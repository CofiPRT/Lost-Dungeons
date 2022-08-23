using Character.Implementation.Base;
using Character.Implementation.Base.AIActions;

namespace Character.Implementation.Enemy.AIChecks {
    public class EnemyAttackCheck : GenericCharacter.BaseAICheck {
        private new readonly GenericEnemy instance;

        public EnemyAttackCheck(GenericEnemy instance) : base(instance, 2, 3) {
            this.instance = instance;
        }

        protected override void Perform() {
            if (instance.FairFight == null || instance.FairFight.IsWaiting(instance))
                return; // not in a fight

            var destination = instance.FairFight.Owner.Pos2D;

            instance.AIAction = new AIMoveAction(
                instance,
                destination,
                true,
                true,
                instance.FairFight == null
            );
        }
    }
}