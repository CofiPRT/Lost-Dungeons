using Character.Abilities.Tristian;
using Character.Implementation.Base;
using UnityEngine;

namespace Character.Implementation.Player {
    public class PlayerTristian : GenericPlayer {
        public PlayerTristian() : base("Tristian", Color.red) {
            Ability1 = new NaturesMightAbility(this);
            Ability2 = new RunItBackAbility(this);
            Ultimate = new BerserkAbility(this);
        }

        /* Parent */

        protected override float AttackSpeed => base.AttackSpeed * (UltimateActive ? 1.5f : 1f);
        protected override bool CanUseRareFinisher => UltimateActive;

        protected override float TakeDamage(float damage, GenericCharacter source = null) {
            // reduce incoming damage when in ultimate
            if (UltimateActive)
                damage *= 0.75f;

            return base.TakeDamage(damage, source);
        }

        public override bool AttemptStun(float stunDuration, GenericCharacter source) {
            // immune to stuns when in ultimate
            return !UltimateActive && base.AttemptStun(stunDuration, source);
        }

        protected override void OnAttackSuccess(GenericCharacter target, float damageDealt) {
            base.OnAttackSuccess(target, damageDealt);

            // restore health when in ultimate
            if (UltimateActive)
                Heal(10f);
        }
    }
}