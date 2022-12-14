using Character.Implementation.Base;
using Properties;

namespace Character.Implementation.Enemy {
    public class EnemyOrange : GenericEnemy {
        public EnemyOrange() : base(CreateData()) { }

        private static CharacterBuilder CreateData() {
            return new CharacterBuilder {
                name = "Orange Enemy",

                maxHealth = 50,
                attackDamage = 7.5f,

                attackStrength = AttackStrength.Medium,
                blockStrength = BlockStrength.Weak
            };
        }
    }
}