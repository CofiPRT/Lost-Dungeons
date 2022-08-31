using System;
using System.Collections.Generic;
using System.Linq;
using Character.Implementation.Ally.AIChecks;
using Character.Implementation.Base;
using Character.Implementation.Enemy;
using Character.Implementation.Player;
using Game;
using Properties;
using UnityEngine;

namespace Character.Implementation.Ally {
    public abstract class GenericAlly : GenericCharacter, IComparable<GenericAlly> {
        private const float SearchRadius = 10f;

        protected GenericAlly(CharacterBuilder data) : base(data) {
            FairFight = new FairFight(this);
            AIChecks = new BaseAICheck[] {
                new AllyFollowCheck(this),
                new AllyAttackCheck(this),
                new AllyBlockCheck(this),
                new AllyWanderCheck(this),
                new AllyTeamUpCheck(this)
            };
        }

        public FairFight FairFight { get; }
        public GenericPlayer Leader => GameController.ControlledPlayer;

        public void UpdateFairFight() {
            FairFight.Update();
        }

        /* Parent */

        protected override void OnAttackSuccess(GenericCharacter target, float damageDealt) {
            if (!target.IsAlive) return; // target was killed by this blow, don't subscribe

            if (!(target is GenericEnemy enemy)) return;

            // unsubscribe from other fights if the character is waiting in one
            if (enemy.FairFight != null && enemy.FairFight != FairFight && enemy.FairFight.IsWaiting(enemy))
                enemy.FairFight.Unsubscribe(enemy);

            // subscribe to this fight
            FairFight.LastFoughtEnemy = enemy;
            FairFight.ForceSubscribe(enemy);
        }

        public override void OnDamageTaken(float damageTaken, GenericCharacter source) {
            base.OnDamageTaken(damageTaken, source);

            if (!(source is GenericEnemy enemy)) return;

            FairFight.LastFoughtEnemy = enemy;
        }

        public override void UpdateLookDirection() {
            // override the look direction if instructed
            if (FairFight.InFight && !IsBlocking && !IsAttacking && !IsRunning) {
                // ensure last fought enemy is set
                FairFight.LastFoughtEnemy ??= FairFight.GetRandomFightingEnemy();
                LookDirection = FairFight.LastFoughtEnemy.Pos2D - Pos2D;
            }

            base.UpdateLookDirection();
        }

        public override void OnDeath() {
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