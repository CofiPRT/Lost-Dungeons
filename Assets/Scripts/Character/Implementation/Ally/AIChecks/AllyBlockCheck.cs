using Character.Implementation.Base;
using Character.Implementation.Base.AIActions;
using UnityEngine;

namespace Character.Implementation.Ally.AIChecks {
    public class AllyBlockCheck : GenericCharacter.BaseAICheck {
        private readonly GenericAlly instance;

        public AllyBlockCheck(GenericAlly instance) : base(instance, 2, 3) {
            this.instance = instance;
        }

        protected override void Perform() {
            if (!instance.FairFight.InFight)
                return; // not in a fight

            var ownPos = instance.Pos2D;
            var opponentPos = instance.FairFight.GetRandomFightingEnemy().Pos2D;

            var distance = Vector2.Distance(ownPos, opponentPos);
            if (distance > 2.5f)
                return; // too far away

            var direction = (opponentPos - ownPos).normalized;

            instance.AIAction = new AIBlockAction(instance, direction);
        }
    }
}