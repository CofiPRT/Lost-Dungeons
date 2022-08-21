using System;
using Scripts.Character.Abilities;
using Scripts.Character.Attributes;

namespace Scripts.Character.Implementation.Base {
    public abstract class CasterCharacter : KnightCharacter, IHasMana, IHasAbilities {
        protected CasterCharacter(CharacterData data) : base(data) {
            Mana = data.maxMana;
            MaxMana = data.maxMana;
            Abilities = data.abilities ?? Array.Empty<Ability>();
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
            if (!IsAlive)
                return;

            foreach (var ability in Abilities)
                ability.Update();
        }

        /* Unity */

        protected override UpdateDelegate UpdateActions => base.UpdateActions + UpdateAbilities;
    }
}