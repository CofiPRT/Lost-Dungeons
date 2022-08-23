using Character.Implementation.Base;

namespace Character.Implementation.Enemy.AIChecks {
    public class EnemyFightCheck : GenericCharacter.BaseAICheck {
        private new readonly GenericEnemy instance;

        public EnemyFightCheck(GenericEnemy instance) : base(instance, 2, 3) {
            this.instance = instance;
        }

        protected override void Perform() {
            if (instance.FairFight != null)
                return; // already in a fight

            // look for a fight - search for an opponent in a radius
            var opponents = instance.FindOpponents();

            // sort opponents and choose the first
            opponents.Sort();

            // if there are no opponents, there's nothing to do
            if (opponents.Count == 0)
                return;

            var opponent = opponents[0];

            // subscribe to this fight
            instance.FairFight = opponent.FairFight;
            instance.FairFight.Subscribe(instance);
        }
    }
}