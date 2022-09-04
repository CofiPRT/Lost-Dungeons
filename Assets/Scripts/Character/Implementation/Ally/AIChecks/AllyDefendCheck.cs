using Character.Implementation.Base;
using UnityEngine;

namespace Character.Implementation.Ally.AIChecks {
    public class AllyDefendCheck : GenericCharacter.BaseAICheck {
        private readonly GenericAlly instance;

        public AllyDefendCheck(GenericAlly instance) : base(instance, 2, 3) {
            this.instance = instance;
        }

        protected override void Perform() {
            if (instance.DefendPosition == null)
                return;

            // if the distance to the defend position is too far, force move to it
            var defend2D = instance.DefendPosition2D ?? instance.DefendPosition.Value;
            var distance = Vector3.Distance(instance.Pos2D, defend2D);

            if (distance > 10)
                instance.ForceMoveTo(defend2D);
        }
    }
}