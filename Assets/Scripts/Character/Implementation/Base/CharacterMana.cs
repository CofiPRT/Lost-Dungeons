using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public float Mana { get; set; }
        public float MaxMana { get; }

        public bool HasMana(float amount) {
            return Mana >= amount;
        }

        public bool UseMana(float amount) {
            if (!HasMana(amount))
                return false; // not enough mana to complete action

            Mana -= amount;
            return true;
        }

        public void ReplenishMana(float amount) {
            Mana = Mathf.Min(Mana + amount, MaxMana);
        }
    }
}