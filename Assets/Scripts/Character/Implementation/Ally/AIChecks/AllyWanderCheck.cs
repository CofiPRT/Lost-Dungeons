using Character.Implementation.Base;
using Character.Implementation.Base.AIActions;
using UnityEngine;

namespace Character.Implementation.Ally.AIChecks {
    public class AllyWanderCheck : GenericCharacter.BaseAICheck {
        private new readonly GenericAlly instance;

        public AllyWanderCheck(GenericAlly instance) : base(instance, 5, 10) {
            this.instance = instance;
        }

        protected override void Perform() {
            if (instance.FairFight.InFight)
                return; // only wander when not in a fight

            var distToLeader = Vector2.Distance(instance.Leader.Pos2D, instance.Pos2D);
            if (distToLeader > 5)
                return; // this should be handled by AllyFollowCheck

            var randomAngle = Random.Range(0, Mathf.PI * 2);
            var randomDistance = Random.Range(3, 5);

            var destination = instance.Leader.Pos2D + new Vector2(
                randomDistance * Mathf.Cos(randomAngle),
                randomDistance * Mathf.Sin(randomAngle)
            );

            instance.AIAction = new AIMoveAction(instance, destination, false);
        }
    }
}