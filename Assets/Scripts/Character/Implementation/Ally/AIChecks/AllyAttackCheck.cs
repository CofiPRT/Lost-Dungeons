using Character.Implementation.Base;
using Character.Implementation.Base.AIActions;

namespace Character.Implementation.Ally.AIChecks {
    public class AllyAttackCheck : GenericCharacter.BaseAICheck {
        private new readonly GenericAlly instance;

        public AllyAttackCheck(GenericAlly instance) : base(instance, 2, 3) {
            this.instance = instance;
        }

        protected override void Perform() {
            if (!instance.FairFight.InFight)
                return; // not in a fight

            var destination = instance.FairFight.GetRandomFightingEnemy().Pos2D;

            instance.AIAction = new AIMoveAction(
                instance,
                destination,
                true,
                true
            );
        }
    }
}