using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public float Mana { get; set; }
        public float MaxMana { get; }

        public bool UseMana(float manaCost) {
            if (Mana < manaCost)
                return false; // not enough mana to complete action

            Mana -= manaCost;
            return true;
        }

        public void ReplenishMana(float manaAmount) {
            Mana = Mathf.Min(Mana + manaAmount, MaxMana);
        }
    }
}