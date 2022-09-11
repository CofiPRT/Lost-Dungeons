using Character.Implementation.Base;

namespace Character.Implementation.Enemy.AIChecks {
    public class EnemyVisionCheck : GenericCharacter.BaseAICheck {
        private readonly GenericEnemy instance;

        public EnemyVisionCheck(GenericEnemy instance) : base(instance, 2, 3) {
            this.instance = instance;
        }

        protected override void Perform() {
            if (instance.FairFight == null)
                return; // not in a fight

            if (!instance.CanSee(instance.FairFight.Owner))
                instance.FairFight.Unsubscribe(instance);
        }
    }
}