using System.Linq;
using Character.Implementation.Base;

namespace Character.Implementation.Enemy.AIChecks {
    public class EnemyBetterFightCheck : GenericCharacter.BaseAICheck {
        private readonly GenericEnemy instance;

        public EnemyBetterFightCheck(GenericEnemy instance) : base(instance, 2, 3) {
            this.instance = instance;
        }

        protected override void Perform() {
            // if we're in a fight, and waiting, attempt to search for a more available opponent
            if (instance.FairFight == null || instance.FairFight.IsFighting(instance))
                return;

            var opponents = instance.FindOpponents();
            opponents.Sort();

            var validOpponents = opponents.Where(
                    opponent => !opponent.FairFight.MaxFightingEnemiesReached
                )
                .ToList();

            var opponent = validOpponents.Any() ? validOpponents.First() : null;

            // if there's no such opponent, there's nothing to do
            if (opponent == null)
                return;

            // unsubscribe from the current fight and subscribe to the new one
            instance.FairFight.Unsubscribe(instance);
            instance.FairFight = opponent.FairFight;
            instance.FairFight.Subscribe(instance);
        }
    }
}