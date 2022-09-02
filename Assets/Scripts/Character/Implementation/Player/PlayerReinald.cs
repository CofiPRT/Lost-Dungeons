using Character.Abilities.Reinald;
using Character.Implementation.Base;
using UnityEngine;

namespace Character.Implementation.Player {
    public class PlayerReinald : GenericPlayer {
        public PlayerReinald() : base("Reinald", Color.blue) {
            Ability1 = new NaturesTricksAbility(this);
            Ability2 = new BehindYouAbility(this);
            Ultimate = new SpiritFormAbility(this);
        }

        /* Parent */

        protected override float MovementSpeedFactor => base.MovementSpeedFactor * (UltimateActive ? 2f : 1f);
        protected internal override bool IsDetectable => !UltimateActive;

        public override void StartUltimate() {
            base.StartUltimate();

            // unsubscribe every enemy from the fight
            FairFight.UnsubscribeAll();
        }

        protected override void OnKill(GenericCharacter target) {
            // reset ultimate cooldown
            Ultimate.Reset();
        }
    }
}