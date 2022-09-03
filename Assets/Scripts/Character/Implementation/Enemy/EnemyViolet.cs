using Character.Implementation.Base;

namespace Character.Implementation.Enemy {
    public class EnemyViolet : GenericEnemy {
        public EnemyViolet() : base(CreateData()) { }

        private static CharacterBuilder CreateData() {
            return new CharacterBuilder {
                name = "Violet Enemy",

                maxHealth = 150,
                attackDamage = 40,

                attackStrength = Properties.AttackStrength.Strong,
                blockStrength = Properties.BlockStrength.Strong
            };
        }
    }
}