using Character.Implementation.Base;
using Character.Implementation.Base.AIActions;

namespace Character.Implementation.Ally.AIChecks {
    public class AllyBlockCheck : GenericCharacter.BaseAICheck {
        private new readonly GenericAlly instance;

        public AllyBlockCheck(GenericAlly instance) : base(instance, 2, 3) {
            this.instance = instance;
        }

        protected override void Perform() {
            if (!instance.FairFight.InFight)
                return; // not in a fight

            var direction = instance.FairFight.GetRandomFightingEnemy().Pos2D - instance.Pos2D;

            instance.AIAction = new AIBlockAction(instance, direction);
        }
    }
}