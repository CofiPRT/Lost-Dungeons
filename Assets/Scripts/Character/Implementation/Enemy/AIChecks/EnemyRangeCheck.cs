using Character.Implementation.Base;
using Character.Implementation.Base.AIActions;
using UnityEngine;

namespace Character.Implementation.Enemy.AIChecks {
    public class EnemyRangeCheck : GenericCharacter.BaseAICheck {
        private new readonly GenericEnemy instance;

        public EnemyRangeCheck(GenericEnemy instance) : base(instance, 2, 3) {
            this.instance = instance;
        }

        protected override void Perform() {
            if (instance.FairFight == null)
                return; // not in a fight or already moving into range

            var desiredRange = instance.FairFight.IsFighting(instance) ? 2f : 5f;
            var deviatedRange = desiredRange + Random.Range(-0.5f, 0.5f);

            var opponentPos = instance.FairFight.Owner.Pos2D;
            var ownPos = instance.Pos2D;

            var direction = (ownPos - opponentPos).normalized;
            var desiredAngle = Mathf.Atan2(direction.y, direction.x);
            var angleDeviation = instance.FairFight.IsFighting(instance) ? 0f : Mathf.PI / 6;
            var deviatedAngle = desiredAngle + Random.Range(-angleDeviation, angleDeviation);

            var destination = opponentPos + new Vector2(
                deviatedRange * Mathf.Cos(deviatedAngle),
                deviatedRange * Mathf.Sin(deviatedAngle)
            );

            var run = Vector2.Distance(ownPos, opponentPos) > 5.5f;

            instance.AIAction = new AIMoveAction(instance, destination, run, false);
        }
    }
}