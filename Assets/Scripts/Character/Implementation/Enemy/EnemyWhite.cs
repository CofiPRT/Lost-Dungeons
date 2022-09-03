﻿using Character.Implementation.Base;

namespace Character.Implementation.Enemy {
    public class EnemyWhite : GenericEnemy {
        public EnemyWhite() : base(CreateData()) { }

        private static CharacterBuilder CreateData() {
            return new CharacterBuilder {
                name = "White Enemy",

                maxHealth = 25,
                attackDamage = 5,

                attackStrength = Properties.AttackStrength.Weak,
                blockStrength = Properties.BlockStrength.Weak
            };
        }
    }
}