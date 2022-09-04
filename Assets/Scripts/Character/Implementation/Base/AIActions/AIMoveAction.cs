using UnityEngine;

namespace Character.Implementation.Base.AIActions {
    public class AIMoveAction : GenericCharacter.BaseAIAction {
        private readonly Vector2 destination;
        private readonly bool run;
        private readonly bool syncLookDirection;

        public AIMoveAction(
            GenericCharacter instance,
            Vector2 destination,
            bool run = true,
            bool syncLookDirection = true,
            float maxDuration = 5,
            int priority = 2
        ) : base(instance, priority, maxDuration) {
            this.destination = destination;
            this.run = run;
            this.syncLookDirection = syncLookDirection;
        }

        protected override void OnUpdate() {
            var ownPos = instance.Pos2D;
            var direction = (destination - ownPos).normalized;

            if (Vector2.Distance(ownPos, destination) < 0.5f) {
                OnEnd();
                return; // reached destination
            }

            instance.ApplyMovement(direction, run, syncLookDirection);
        }
    }
}