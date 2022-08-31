using Properties;
using UnityEngine;

namespace Character.Implementation.Base {
    public class CharacterBuilder {
        /* IHasTeam */

        private const Team DefaultTeam = Team.Enemy;
        private const string DefaultName = "Unnamed";

        public Team team = DefaultTeam;
        public string name = DefaultName;

        /* IHasHealth */

        private const float DefaultMaxHealth = 100;

        public float maxHealth = DefaultMaxHealth;

        /* IHasWeapon */

        private const float DefaultAttackDamage = 10;
        private const float DefaultAttackSpeed = 1; // in attacks per second
        private const float DefaultAttackRange = 2;
        private const float DefaultAttackAngle = Mathf.PI / 4; // in radians
        private const AttackStrength DefaultAttackStrength = AttackStrength.Medium;

        public float attackDamage = DefaultAttackDamage;
        public float attackSpeed = DefaultAttackSpeed;
        public float attackRange = DefaultAttackRange;
        public float attackAngle = DefaultAttackAngle;
        public AttackStrength attackStrength = DefaultAttackStrength;

        /* IHasShield */

        private const float DefaultShieldAngle = Mathf.PI / 4;
        private const float DefaultShieldRechargeTime = 5; // seconds
        private const BlockStrength DefaultBlockStrength = BlockStrength.Medium;

        public float shieldAngle = DefaultShieldAngle;
        public float shieldRechargeTime = DefaultShieldRechargeTime;
        public BlockStrength blockStrength = DefaultBlockStrength;

        /* IHasMana */

        private const float DefaultMaxMana = 100;

        public float maxMana = DefaultMaxMana;
    }
}