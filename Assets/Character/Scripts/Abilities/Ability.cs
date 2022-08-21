using Character.Scripts.Generic;
using UnityEngine;

namespace Character.Scripts.Abilities {
    public abstract class Ability {
        internal readonly CasterCharacter user;
        private readonly AbilityPhase[] phases;
        private readonly AbilityPhase finalPhase;
        private readonly float cooldown;

        private int currentPhase;
        private float currentCooldown;
        private bool active;
        private bool aborted;

        protected Ability(AbilityPhase[] phases, CasterCharacter user, float cooldown, AbilityPhase finalPhase = null) {
            this.phases = phases;
            this.user = user;
            this.cooldown = cooldown;

            this.finalPhase = finalPhase ?? new DefaultFinalPhase(this);
        }

        public bool Use() {
            // if on cooldown, don't start
            if (currentCooldown > 0)
                return false;

            // if already active, offer a reactivation to the current phase
            if (active)
                phases[currentPhase].OnReactivation();
            else
                Reset();

            return true;
        }

        public void Update() {
            currentCooldown = Mathf.Max(0, currentCooldown - user.DeltaTime);

            // only update while active
            if (!active)
                return;

            // offer a tick to the current phase
            var phase = phases[currentPhase];
            phase.Tick();

            // advance to the next phase if the current one is done
            if (phase.Finished)
                currentPhase++;

            // if we've run through all the phases, or the ability has been aborted, end it
            if (currentPhase >= phases.Length || aborted)
                finalPhase.Tick(); // at its end, the end phase should deactivate this ability
        }

        internal void StartCooldown() {
            active = false;
            currentCooldown = cooldown;
        }

        internal void Abort() {
            aborted = true;
        }

        private void Reset() {
            currentCooldown = 0;
            currentPhase = 0;
            aborted = false;
            active = true;

            foreach (var phase in phases)
                phase.Reset();
        }
    }
}