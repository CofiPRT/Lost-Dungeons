using Character.Implementation.Base;
using Character.Implementation.Base.AIActions;
using UnityEngine;

namespace Character.Implementation.Enemy.AIChecks {
    public class EnemyBlockCheck : GenericCharacter.BaseAICheck {
        private new readonly GenericEnemy instance;

        public EnemyBlockCheck(GenericEnemy instance) : base(instance, 2, 3) {
            this.instance = instance;
        }

        protected override void Perform() {
            if (instance.FairFight == null || instance.FairFight.IsWaiting(instance))
                return; // not in a fight

            // must be in fighting range
            var distance = Vector2.Distance(instance.FairFight.Owner.Pos2D, instance.Pos2D);
            if (distance > 2.5)
                return;

            var direction = instance.FairFight.Owner.Pos2D - instance.Pos2D;

            instance.AIAction = new AIBlockAction(instance, direction);
        }
    }
}