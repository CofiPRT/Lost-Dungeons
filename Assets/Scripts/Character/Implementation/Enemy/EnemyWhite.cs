using Character.Implementation.Base;
using Properties;

namespace Character.Implementation.Enemy {
    public class EnemyWhite : GenericEnemy {
        public EnemyWhite() : base(CreateData()) { }

        private static CharacterBuilder CreateData() {
            return new CharacterBuilder {
                name = "White Enemy",

                maxHealth = 25,
                attackDamage = 5,

                attackStrength = AttackStrength.Weak,
                blockStrength = BlockStrength.Weak
            };
        }
    }
}