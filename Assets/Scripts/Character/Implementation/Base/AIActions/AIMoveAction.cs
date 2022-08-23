using Character.Implementation.Enemy;
using UnityEngine;

namespace Character.Implementation.Base.AIActions {
    public class AIMoveAction : GenericCharacter.BaseAIAction {
        private new readonly GenericCharacter instance;
        private readonly Vector2 destination;
        private readonly bool run;
        private readonly bool attackOnReach;
        private readonly bool syncLookDirection;

        public AIMoveAction(
            GenericCharacter instance,
            Vector2 destination,
            bool run = true,
            bool attackOnReach = false,
            bool syncLookDirection = true,
            float maxDuration = 5
        )
            : base(instance, maxDuration) {
            this.instance = instance;
            this.destination = destination;
            this.run = run;
            this.attackOnReach = attackOnReach;
            this.syncLookDirection = syncLookDirection;
        }

        protected override void OnUpdate() {
            var ownPos = instance.Pos2D;
            var direction = (destination - ownPos).normalized;

            if (Vector2.Distance(ownPos, destination) < 0.5f) {
                if (attackOnReach)
                    instance.StartAttack(direction);

                OnEnd();
                return; // reached destination
            }

            instance.ApplyMovement(direction, run, syncLookDirection);
        }
    }
}