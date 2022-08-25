using System.Collections.Generic;
using System.Linq;
using Character.Implementation.Ally;
using Character.Implementation.Base;
using Character.Implementation.Enemy.AIChecks;
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
                new EnemyWanderCheck(this)
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
                .Where(x => x != null && x.IsAlive)
                .ToList();
        }

        /* Parent */

        public override Vector2 LookDirection => FairFight != null ? FairFight.Owner.Pos2D - Pos2D : Forward2D;
        
        public override void OnDeath() {
            base.OnDeath();
            FairFight?.Unsubscribe(this);
        }
    }
}