using System.Collections.Generic;
using System.Linq;
using Character.Implementation.Ally;
using Character.Implementation.Base;
using Properties;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Character.Implementation.Enemy {
    public abstract class GenericEnemy : GenericCharacter {
        private const float SearchRadius = 10f;

        protected GenericEnemy(CharacterBuilder data) : base(SetTeam(data)) {
            AIChecks = new BaseAICheck[] {
                new FightCheck(this),
                new BetterFightCheck(this),
                new RangeCheck(this)
            };
        }

        private static CharacterBuilder SetTeam(CharacterBuilder data) {
            data.team = Team.Enemy;
            return data;
        }

        private FairFight fairFight;

        private class FightCheck : BaseAICheck {
            private new readonly GenericEnemy instance;

            public FightCheck(GenericEnemy instance) : base(instance, 2, 3) {
                this.instance = instance;
            }

            protected override void Perform() {
                if (instance.fairFight != null)
                    return; // already in a fight

                // look for a fight - search for an opponent in a radius
                var opponents = instance.FindOpponents();

                // sort opponents and choose the first
                opponents.Sort();

                // if there are no opponents, there's nothing to do
                if (opponents.Count == 0)
                    return;

                var opponent = opponents[0];

                // subscribe to this fight
                instance.fairFight = opponent.FairFight;
                instance.fairFight.Subscribe(instance);
            }
        }

        private class BetterFightCheck : BaseAICheck {
            private new readonly GenericEnemy instance;

            public BetterFightCheck(GenericEnemy instance) : base(instance, 2, 3) {
                this.instance = instance;
            }

            protected override void Perform() {
                // if we're in a fight, and waiting, attempt to search for a more available opponent
                if (instance.fairFight == null || instance.fairFight.IsFighting(instance))
                    return;

                var opponents = instance.FindOpponents();
                opponents.Sort();

                var opponent = opponents.Where(
                        opponent => !opponent.FairFight.MaxFightingEnemiesReached
                    )
                    .FirstOrDefault(null!);

                // if there's no such opponent, there's nothing to do
                if (opponent == null)
                    return;

                // unsubscribe from the current fight and subscribe to the new one
                instance.fairFight.Unsubscribe(instance);
                instance.fairFight = opponent.FairFight;
                instance.fairFight.Subscribe(instance);
            }
        }

        private class RangeCheck : BaseAICheck {
            private new readonly GenericEnemy instance;

            public RangeCheck(GenericEnemy instance) : base(instance, 2, 3) {
                this.instance = instance;
            }

            protected override void Perform() {
                if (instance.fairFight == null)
                    return; // not in a fight or already moving into range

                var desiredRange = instance.fairFight.IsFighting(instance) ? 2f : 5f;
                var deviatedRange = desiredRange + Random.Range(-0.5f, 0.5f);

                var opponentPos = instance.fairFight.Owner.Pos2D;
                var ownPos = instance.Pos2D;

                var direction = (opponentPos - ownPos).normalized;
                var desiredAngle = Mathf.Atan2(direction.y, direction.x);
                var angleDeviation = instance.fairFight.IsFighting(instance) ? 0f : Mathf.PI / 4;
                var deviatedAngle = desiredAngle + Random.Range(-angleDeviation, angleDeviation);

                var destination = opponentPos + new Vector2(
                    deviatedRange * Mathf.Cos(deviatedAngle),
                    deviatedRange * Mathf.Sin(deviatedAngle)
                );

                var run = Vector2.Distance(ownPos, opponentPos) > 5.5f;

                instance.AIAction = new AIMoveAction(instance, destination, run);
            }
        }

        // public override void RunAITick() {
        //     performAttackCooldown = Mathf.Max(0, performAttackCooldown - DeltaTime);
        //     if (performAttackCooldown == 0) {
        //         performAttackCooldown = Random.Range(2, 3);
        //         PerformAttack();
        //     }
        // }

        private class AIMoveAction : BaseAIAction {
            private new readonly GenericEnemy instance;
            private readonly Vector2 destination;
            private readonly bool run;

            public AIMoveAction(GenericEnemy instance, Vector2 destination, bool run = true, float maxDuration = 5)
                : base(instance, maxDuration) {
                this.instance = instance;
                this.destination = destination;
                this.run = run;
            }

            protected override void RunTick() {
                var ownPos = instance.Pos2D;

                if (Vector2.Distance(ownPos, destination) < 0.5f) {
                    End(); // reached destination
                    return;
                }

                var direction = (destination - ownPos).normalized;
                var syncLookDirection = instance.fairFight == null;

                instance.ApplyMovement(direction, run, syncLookDirection);
            }
        }

        private List<GenericAlly> FindOpponents() {
            // ReSharper disable once Unity.PreferNonAllocApi
            return Physics.OverlapSphere(
                    Pos,
                    SearchRadius,
                    LayerMask.GetMask(AttackableTeams.Select(TeamUtils.ToLayer).ToArray())
                )
                .Select(x => x.GetComponent<GenericAlly>())
                .Where(x => x != null)
                .ToList();
        }
    }
}