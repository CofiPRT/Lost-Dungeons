using System;
using Character.Scripts.Abilities;
using Character.Scripts.Attributes;
using Character.Scripts.Properties;

namespace Character.Scripts.Generic {
    public abstract class CasterCharacter : KnightCharacter, IHasMana, IHasAbilities {
        private const float DefaultMaxMana = 100;
        private const Ability[] DefaultAbilities = null;

        protected CasterCharacter(
            Team team,
            float maxHealth = DefaultMaxHealth,
            float attackDamage = DefaultAttackDamage,
            float attackSpeed = DefaultAttackSpeed,
            float attackRange = DefaultAttackRange,
            float attackAngle = DefaultAttackAngle,
            float shieldAngle = DefaultShieldAngle,
            float shieldRechargeTime = DefaultShieldRechargeTime,
            BlockStrength blockStrength = DefaultBlockStrength,
            float maxMana = DefaultMaxMana,
            Ability[] abilities = DefaultAbilities
        ) : base(
            team,
            maxHealth,
            attackDamage,
            attackSpeed,
            attackRange,
            attackAngle,
            shieldAngle,
            shieldRechargeTime,
            blockStrength
        ) {
            Mana = maxMana;
            MaxMana = maxMana;
            Abilities = abilities ?? Array.Empty<Ability>();
        }

        /* IHasMana */

        public float Mana { get; set; }
        public float MaxMana { get; }

        public bool UseMana(float manaCost) {
            if (Mana < manaCost)
                return false; // not enough mana to complete action

            Mana -= manaCost;
            return true;
        }

        public void ReplenishMana(float manaAmount) {
            Mana = Math.Min(Mana + manaAmount, MaxMana);
        }

        /* IHasAbilities */

        public Ability[] Abilities { get; }

        public bool UseAbility(int index) {
            return Abilities[index].Use();
        }

        public void UpdateAbilities() {
            foreach (var ability in Abilities)
                ability.Update();
        }
    }
}