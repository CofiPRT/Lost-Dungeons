using UnityEngine;

namespace Character.Implementation.Base.AIActions {
    public class AIAttackAction : GenericCharacter.BaseAIAction {
        private readonly GenericCharacter target;
        private readonly bool run;
        private readonly bool syncLookDirection;

        public AIAttackAction(
            GenericCharacter instance,
            GenericCharacter target,
            bool run = true,
            bool syncLookDirection = true,
            float maxDuration = 5
        ) : base(instance, maxDuration) {
            this.target = target;
            this.run = run;
            this.syncLookDirection = syncLookDirection;
        }

        protected override void OnUpdate() {
            var ownPos = instance.Pos2D;
            var targetPos = target.Pos2D;

            var direction = (targetPos - ownPos).normalized;

            if (Vector2.Distance(ownPos, targetPos) < instance.AttackRange * 0.75) {
                instance.StartAttack(direction);
                OnEnd();
                return; // reached destination
            }

            instance.ApplyMovement(direction, run, syncLookDirection);
        }
    }
}