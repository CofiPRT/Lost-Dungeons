using System;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public bool IsAlive => Health > 0;
        private float DeathTime { get; set; }
        public float Health { get; private set; }
        public float MaxHealth { get; }
        public bool CanTakeDamage { get; set; } = true;

        private float TakeDamage(float damage, GenericCharacter source = null) {
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
            if (source)
                source.OnKill(this);

            return damageDealt;
        }

        protected virtual void OnDamageTaken(float damageTaken, GenericCharacter source) {
            Animator.SetBool(AnimatorHash.Hurt, true);
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
    }
}