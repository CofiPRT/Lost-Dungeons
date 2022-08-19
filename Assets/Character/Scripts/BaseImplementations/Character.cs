using System;
using Character.Scripts.Properties;
using UnityEngine;

namespace Character.Scripts.BaseImplementations {
    public class Character : MonoBehaviour, IHasTeam, IHasHealth, IStunnable {
        protected const float DefaultMaxHealth = 100;

        public Character(TeamMembership teamMembership, float maxHealth = DefaultMaxHealth) {
            TeamMembership = teamMembership;

            IsAlive = true;
            Health = maxHealth;
            MaxHealth = maxHealth;
        }

        public TeamMembership TeamMembership { get; }

        public void OnKill(Character target) {
            // intentionally left blank
        }

        public bool IsAlive { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; }

        public float TakeDamage(float damage, Character source = null) {
            var prevHealth = Health;
            Health = Math.Max(0, Health - damage);

            var damageDealt = prevHealth - Health;

            if (Health != 0) return damageDealt;

            IsAlive = false;
            OnDeath();

            if (source)
                source.OnKill(this);

            return damageDealt;
        }

        public void Heal(float healAmount) {
            Health = Math.Min(MaxHealth, Health + healAmount);
        }

        public void OnDeath() {
            // intentionally left blank
        }

        public float StunDuration { get; set; }
        public bool IsStunned => StunDuration > 0;

        public bool AttemptStun(float stunDuration, Character source) {
            StunDuration = stunDuration;

            return true;
        }

        private void Update() {
            StunDuration = Math.Max(0, StunDuration - Time.deltaTime);
        }
    }
}