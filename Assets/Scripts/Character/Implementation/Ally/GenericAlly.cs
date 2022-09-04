using System;
using Character.Implementation.Ally.AIChecks;
using Character.Implementation.Base;
using Character.Implementation.Enemy;
using Character.Implementation.Player;
using Character.Misc;
using Game;
using UnityEngine;

namespace Character.Implementation.Ally {
    public abstract class GenericAlly : GenericCharacter, IComparable<GenericAlly> {
        protected GenericAlly(CharacterBuilder data) : base(data) {
            FairFight = new FairFight(this);
            AIChecks = new BaseAICheck[] {
                new AllyFollowCheck(this),
                new AllyAttackCheck(this),
                new AllyBlockCheck(this),
                new AllyWanderCheck(this),
                new AllyTeamUpCheck(this),
                new AllyDefendCheck(this)
            };
        }

        public FairFight FairFight { get; set; }
        public GenericPlayer Leader => GameController.ControlledPlayer;

        private Vector3? defendPosition;

        public Vector3? DefendPosition {
            get => defendPosition;
            set {
                defendPosition = value;
                if (defendPosition != null)
                    ForceMoveTo(new Vector2(defendPosition.Value.x, defendPosition.Value.z));
            }
        }

        public Vector2? DefendPosition2D => DefendPosition.HasValue
            ? new Vector2(DefendPosition.Value.x, DefendPosition.Value.z)
            : (Vector2?)null;

        public void UpdateFairFight() {
            FairFight.Update();
        }

        /* Parent */

        protected override void OnAIDisable() {
            DefendPosition = null;
        }

        protected override void OnAttackSuccess(GenericCharacter target, float damageDealt) {
            if (!target.IsAlive) {
                if (FairFight.LastFoughtEnemy == target)
                    FairFight.LastFoughtEnemy = null;

                return; // target was killed by this blow, don't subscribe
            }

            if (!(target is GenericEnemy enemy))
                return;

            if (enemy.FairFight != null && enemy.FairFight != FairFight) {
                // unsubscribe from other fights if the character is waiting in one
                if (enemy.FairFight.IsWaiting(enemy)) {
                    enemy.FairFight.Unsubscribe(enemy);

                    // subscribe to this fight
                    FairFight.ForceSubscribe(enemy);
                }
            } else {
                // force subscribe to this fight
                FairFight.ForceSubscribe(enemy);
            }
        }

        protected override void OnDamageTaken(float damageTaken, GenericCharacter source) {
            base.OnDamageTaken(damageTaken, source);

            if (!(source is GenericEnemy enemy)) return;

            FairFight.LastFoughtEnemy = enemy;
        }

        public bool UseFairFightLookDirection { get; set; } = true;

        protected override void UpdateLookDirection() {
            // override the look direction if instructed
            if (UseFairFightLookDirection && FairFight.InFight && !IsBlocking && !IsAttacking && !IsRunning) {
                // ensure last fought enemy is set
                FairFight.LastFoughtEnemy ??= FairFight.GetRandomFightingEnemy();
                if (FairFight.LastFoughtEnemy.IsAlive)
                    LookDirection = FairFight.LastFoughtEnemy.Pos2D - Pos2D;
            }

            base.UpdateLookDirection();
        }

        protected override void OnDeath() {
            base.OnDeath();

            // unsubscribe all the enemies from this fight
            FairFight.UnsubscribeAll();
        }

        protected override UpdateDelegate UpdateActions => base.UpdateActions + UpdateFairFight;

        /* IComparable */

        public int CompareTo(GenericAlly other) {
            return FairFight.CompareTo(other.FairFight);
        }
    }
}