using System;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public bool IsAlive => Health > 0;
        public float DeathTime { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; }

        public float TakeDamage(float damage, GenericCharacter source = null) {
            if (!IsAlive)
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

        public virtual void OnDamageTaken(float damageTaken, GenericCharacter source) {
            // intentionally left blank
        }

        public void Heal(float healAmount) {
            if (!IsAlive)
                return;

            Health = Math.Min(MaxHealth, Health + healAmount);
        }

        public virtual void OnDeath() {
            EndAttack();
            EndStun();
            StopMoving();
            StopBlocking();
            SetAI(false);
            Animator.SetBool(AnimatorHash.Dead, true);
        }

        public void UpdateDeathTime() {
            if (IsAlive)
                return;

            DeathTime += DeltaTime;

            if (DeathTime > DefaultDecayTime)
                Destroy(this);
        }
    }
}