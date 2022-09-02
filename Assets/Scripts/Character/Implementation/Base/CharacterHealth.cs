using System;
using Character.Misc;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public bool IsAlive => Health > 0;
        private float DeathTime { get; set; }
        public float Health { get; private set; }
        public float MaxHealth { get; }
        public bool CanTakeDamage { get; set; } = true;
        protected internal virtual bool IsDetectable => true;
        private float LastDamageTime { get; set; }

        protected virtual float TakeDamage(float damage, GenericCharacter source = null) {
            if (!IsAlive || !CanTakeDamage)
                return 0;

            var prevHealth = Health;
            Health = Math.Max(0, Health - damage);

            var damageDealt = prevHealth - Health;

            if (damageDealt > 0)
                OnDamageTaken(damageDealt, source);

            // nothing extra to do if the character is still alive
            if (IsAlive) return damageDealt;

            OnDeath();

            // notify the killer
            if (source != null)
                source.OnKill(this);

            return damageDealt;
        }

        protected virtual void OnDamageTaken(float damageTaken, GenericCharacter source) {
            // consume stun
            EndStun();

            Animator.SetBool(AnimatorHash.Hurt, true);
            LastDamageTime = 0;
            secondCounter = 0;
            secondCounterMana = 0;
        }

        public void StopHurtAnimation() {
            Animator.SetBool(AnimatorHash.Hurt, false);
        }

        public void Heal(float healAmount) {
            if (!IsAlive)
                return;

            Health = Math.Min(MaxHealth, Health + healAmount);
        }

        protected virtual void OnDeath() {
            EndAttack();
            EndStun();
            StopMoving();
            StopBlocking();
            SetAI(false);

            IgnoreCollisions = true;
            Animator.SetBool(AnimatorHash.Dead, true);
        }

        private void UpdateDeathTime() {
            if (IsAlive)
                return;

            DeathTime += DeltaTime;

            if (DeathTime < DefaultDecayTime) return;

            Destroy(gameObject);
            Destroy(healthBarCanvas.gameObject);
        }

        private float secondCounter;

        private void UpdateAutoHeal() {
            LastDamageTime += DeltaTime;

            if (LastDamageTime < 5f || Health >= 25f)
                return;

            // heal 5 health every second
            secondCounter += DeltaTime;

            if (secondCounter < 1f)
                return;

            secondCounter = 0;
            Heal(5);
        }
    }
}