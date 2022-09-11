using Character.Implementation.Base;
using Properties;

namespace Character.Implementation.Enemy {
    public class EnemyViolet : GenericEnemy {
        public EnemyViolet() : base(CreateData()) { }

        private static CharacterBuilder CreateData() {
            return new CharacterBuilder {
                name = "Violet Enemy",

                maxHealth = 150,
                attackDamage = 15,

                attackStrength = AttackStrength.Strong,
                blockStrength = BlockStrength.Strong
            };
        }
    }
}