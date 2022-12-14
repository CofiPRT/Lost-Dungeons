using System.Collections.Generic;
using System.Linq;
using Character.Implementation.Ally;
using Character.Implementation.Base;
using Character.Implementation.Enemy.AIChecks;
using Character.Misc;
using Game;
using Properties;
using UnityEngine;

namespace Character.Implementation.Enemy {
    public abstract class GenericEnemy : GenericCharacter {
        private const float SearchRadius = 10f;

        protected GenericEnemy(CharacterBuilder data) : base(data) {
            AIChecks = new BaseAICheck[] {
                new EnemyFightCheck(this),
                new EnemyBetterFightCheck(this),
                new EnemyRangeCheck(this),
                new EnemyAttackCheck(this),
                new EnemyBlockCheck(this),
                new EnemyWanderCheck(this),
                new EnemyVisionCheck(this)
            };
        }

        public FairFight FairFight { get; set; }

        public List<GenericAlly> FindOpponents() {
            // ReSharper disable once Unity.PreferNonAllocApi
            return Physics.OverlapSphere(
                    Pos,
                    SearchRadius,
                    LayerMask.GetMask(AttackableTeams.Select(TeamUtils.ToLayer).ToArray())
                )
                .Select(x => x.GetComponent<GenericAlly>())
                .Where(x => x != null && CanSee(x) && x.IsAlive && x.IsDetectable)
                .ToList();
        }

        /* Parent */

        protected override void OnDeath() {
            base.OnDeath();
            FairFight?.Unsubscribe(this);
            GameController.AliveEnemies.Remove(this);
        }

        protected override void UpdateLookDirection() {
            // override the look direction if instructed
            if (FairFight != null)
                LookDirection = FairFight.Owner.Pos2D - Pos2D;

            base.UpdateLookDirection();
        }

        protected override void Awake() {
            base.Awake();
            LoadAudioClips();
        }

        private void LoadAudioClips() {
            const string rootPath = "Audio/Character/Enemy/";

            hurtSound = Resources.Load<AudioClip>(rootPath + "enemy_hurt");
            deathSound = Resources.Load<AudioClip>(rootPath + "enemy_death");
            stepSound = Resources.Load<AudioClip>(rootPath + "enemy_step");
        }
    }
}