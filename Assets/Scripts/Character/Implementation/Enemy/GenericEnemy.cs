using System;
using System.Collections.Generic;
using System.Linq;
using Character.Implementation.Ally;
using Character.Implementation.Base;
using Properties;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Character.Implementation.Enemy {
    public abstract class GenericEnemy : KnightCharacter {
        private const float SearchRadius = 10f;

        protected GenericEnemy(CharacterData data) : base(SetTeam(data)) { }

        private static CharacterData SetTeam(CharacterData data) {
            data.team = Team.Enemy;
            return data;
        }

        private FairFight fairFight;

        /* IHasAI */

        public override void UpdateAI() {
            searchForFightCooldown = Mathf.Max(0, searchForFightCooldown - DeltaTime);
            if (searchForFightCooldown == 0) {
                searchForFightCooldown = Random.Range(2, 3);
                SearchForFight();
            }

            searchForAvailableOpponentCooldown = Mathf.Max(0, searchForAvailableOpponentCooldown - DeltaTime);
            if (searchForAvailableOpponentCooldown == 0) {
                searchForAvailableOpponentCooldown = Random.Range(2, 3);
                SearchForAvailableOpponent();
            }

            moveIntoRangeCooldown = Mathf.Max(0, moveIntoRangeCooldown - DeltaTime);
            if (moveIntoRangeCooldown == 0) {
                moveIntoRangeCooldown = Random.Range(2, 3);
                MoveIntoRange();
            }
            
            performAttackCooldown = Mathf.Max(0, performAttackCooldown - DeltaTime);
            if (performAttackCooldown == 0) {
                performAttackCooldown = Random.Range(2, 3);
                PerformAttack();
            }
        }

        private float searchForFightCooldown;

        private void SearchForFight() {
            if (fairFight != null)
                return; // already in a fight

            // look for a fight - search for an opponent in a radius
            var opponents = FindOpponents();

            // sort opponents and choose the first
            opponents.Sort();

            // if there are no opponents, there's nothing to do
            if (opponents.Count == 0)
                return;

            var opponent = opponents[0];

            // subscribe to this fight
            fairFight = opponent.FairFight;
            fairFight.Subscribe(this);
        }

        private float searchForAvailableOpponentCooldown;

        private void SearchForAvailableOpponent() {
            // if we're in a fight, and waiting, attempt to search for a more available opponent
            if (fairFight == null || fairFight.IsFighting(this))
                return;

            var opponents = FindOpponents();
            opponents.Sort();

            var opponent = opponents.Where(
                    opponent => !opponent.FairFight.MaxFightingEnemiesReached
                )
                .FirstOrDefault(null!);

            // if there's no such opponent, there's nothing to do
            if (opponent == null)
                return;

            // unsubscribe from the current fight and subscribe to the new one
            fairFight.Unsubscribe(this);
            fairFight = opponent.FairFight;
            fairFight.Subscribe(this);
        }

        private float moveIntoRangeCooldown;
        private AIMoveAction moveIntoRangeAction;

        private void MoveIntoRange() {
            if (fairFight == null || moveIntoRangeAction != null)
                return; // not in a fight or already moving into range

            var desiredRange = fairFight.IsFighting(this) ? 2f : 5f;
            var deviatedRange = desiredRange + Random.Range(-0.5f, 0.5f);

            var opponentPos3 = fairFight.Owner.transform.position;
            var opponentPos = new Vector2(opponentPos3.x, opponentPos3.z);
            
            var ownPos3 = transform.position;
            var ownPos = new Vector2(ownPos3.x, ownPos3.z);
            
            var direction = (opponentPos - ownPos).normalized;
            var desiredAngle = Mathf.Atan2(direction.y, direction.x);
            var angleDeviation = fairFight.IsFighting(this) ? 0f : Mathf.PI / 4;
            var deviatedAngle = desiredAngle + Random.Range(-angleDeviation, angleDeviation);

            var destination = opponentPos + new Vector2(
                deviatedRange * Mathf.Cos(deviatedAngle),
                deviatedRange * Mathf.Sin(deviatedAngle)
            );
            
            moveIntoRangeAction = new AIMoveAction(this, destination);
        }

        private class AIMoveAction {
            private readonly GenericEnemy instance;
            private readonly Vector2 destination;
            private readonly bool run;
            private readonly float maxDuration;
            private float duration;

            public AIMoveAction(GenericEnemy instance, Vector2 destination, bool run = true, float maxDuration = 5) {
                this.instance = instance;
                this.destination = destination;
                this.run = run;
                this.maxDuration = maxDuration;
            }

            public void Update() {
                var ownPos3 = instance.transform.position;
                var ownPos = new Vector2(ownPos3.x, ownPos3.z);
                
                if (Vector2.Distance(ownPos, destination) < 0.5f) {
                    instance.moveIntoRangeAction = null; // reached destination
                    return;
                }
                
                var direction = (destination - ownPos).normalized;
                var syncLookDirection = instance.fairFight == null;
                
                instance.ApplyMovement(direction, run, syncLookDirection);
                
                duration += Time.deltaTime;
                if (duration > maxDuration)
                    instance.moveIntoRangeAction = null; // ran for too long
            }
        }

        private void UpdateAIAction() {
            moveIntoRangeAction?.Update();
        }

        private List<GenericAlly> FindOpponents() {
            // ReSharper disable once Unity.PreferNonAllocApi
            return Physics.OverlapSphere(
                    transform.position,
                    SearchRadius,
                    LayerMask.GetMask(AttackableTeams.Select(TeamUtils.ToLayer).ToArray())
                )
                .Select(x => x.GetComponent<GenericAlly>())
                .Where(x => x != null)
                .ToList();
        }
        
        /* Unity */

        protected override UpdateDelegate UpdateActions => base.UpdateActions + UpdateAIAction;
    }
}