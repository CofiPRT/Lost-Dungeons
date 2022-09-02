using Character.Implementation.Base;
using Character.Implementation.Base.AIActions;
using UnityEngine;

namespace Character.Implementation.Ally.AIChecks {
    public class AllyFollowCheck : GenericCharacter.BaseAICheck {
        private readonly GenericAlly instance;

        public AllyFollowCheck(GenericAlly instance) : base(instance) {
            this.instance = instance;
        }

        protected override void Perform() {
            if (instance.FairFight.InFight)
                return; // only follow when not in a fight

            var distToLeader = Vector2.Distance(instance.Leader.Pos2D, instance.Pos2D);
            if (distToLeader < 7.5)
                return; // this should be handled by AllyWanderCheck

            var deviatedRange = Random.Range(2.5f, 5f);

            var leaderPos = instance.Leader.Pos2D;
            var ownPos = instance.Pos2D;

            var direction = (leaderPos - ownPos).normalized;
            var desiredAngle = Mathf.Atan2(direction.y, direction.x);
            var deviatedAngle = desiredAngle + Random.Range(-Mathf.PI / 4, Mathf.PI / 4);

            var destination = leaderPos + new Vector2(
                deviatedRange * Mathf.Cos(deviatedAngle),
                deviatedRange * Mathf.Sin(deviatedAngle)
            );

            instance.AIAction = new AIMoveAction(instance, destination);
        }
    }
}