using Character.Abilities;
using Properties;
using UnityEngine;

namespace Character.Implementation.Base {
    public class CharacterBuilder {
        /* IHasTeam */

        private const Team DefaultTeam = Team.Enemy;

        public Team team = DefaultTeam;

        /* IHasHealth */

        private const float DefaultMaxHealth = 100;

        public float maxHealth = DefaultMaxHealth;

        /* IHasWeapon */

        private const float DefaultAttackDamage = 10;
        private const float DefaultAttackSpeed = 1; // in attacks per second
        private const float DefaultAttackRange = 1;
        private const float DefaultAttackAngle = Mathf.PI / 4; // in radians

        public float attackDamage = DefaultAttackDamage;
        public float attackSpeed = DefaultAttackSpeed;
        public float attackRange = DefaultAttackRange;
        public float attackAngle = DefaultAttackAngle;

        /* IHasShield */

        private const float DefaultShieldAngle = Mathf.PI / 4;
        private const float DefaultShieldRechargeTime = 5; // seconds
        private const BlockStrength DefaultBlockStrength = BlockStrength.Weak;

        public float shieldAngle = DefaultShieldAngle;
        public float shieldRechargeTime = DefaultShieldRechargeTime;
        public BlockStrength blockStrength = DefaultBlockStrength;

        /* IHasMana */

        private const float DefaultMaxMana = 100;

        public float maxMana = DefaultMaxMana;
    }
}