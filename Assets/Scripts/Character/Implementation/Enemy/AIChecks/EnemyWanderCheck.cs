using Character.Implementation.Base;
using Character.Implementation.Base.AIActions;
using UnityEngine;

namespace Character.Implementation.Enemy.AIChecks {
    public class EnemyWanderCheck : GenericCharacter.BaseAICheck {
        private readonly GenericEnemy instance;

        public EnemyWanderCheck(GenericEnemy instance) : base(instance, 5, 10) {
            this.instance = instance;
        }

        protected override void Perform() {
            if (instance.FairFight != null)
                return; // only wander when not in a fight

            var randomAngle = Random.Range(0, Mathf.PI * 2);
            var randomDistance = Random.Range(1, 5);

            var destination = instance.Pos2D + new Vector2(
                randomDistance * Mathf.Cos(randomAngle),
                randomDistance * Mathf.Sin(randomAngle)
            );

            instance.AIAction = new AIMoveAction(instance, destination, false);
        }
    }
}