using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public float Mana { get; private set; }
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

        protected void ReplenishMana(float amount) {
            Mana = Mathf.Min(Mana + amount, MaxMana);
        }

        private float secondCounterMana;
        
        private void UpdateAutoManaGain() {
            LastDamageTime += DeltaTime;

            if (LastDamageTime < 5f || Mana >= 50f)
                return;

            // heal 1 mana every half second
            secondCounterMana += DeltaTime;

            if (secondCounterMana < 0.5f)
                return;

            secondCounterMana = 0;
            ReplenishMana(1);
        }
    }
}