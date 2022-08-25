using System;
using Character.Implementation.Base;
using Character.Implementation.Player;
using UnityEngine;

namespace Character.Abilities {
    public abstract class Ability {
        internal readonly GenericPlayer user;
        private readonly float cooldown;

        protected AbilityPhase[] phases = Array.Empty<AbilityPhase>();
        protected AbilityPhase finalPhase;

        private int currentPhase;
        private float currentCooldown;
        private bool active;
        private bool aborted;

        protected Ability(GenericPlayer user, float cooldown) {
            this.user = user;
            this.cooldown = cooldown;

            finalPhase = new DefaultFinalPhase(this);
        }

        public bool Use() {
            // if on cooldown, or another ability blocks this cast, don't start
            if (currentCooldown > 0 || user.CastBlocksAbilityUsage)
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

            // if we've run through all the phases, or the ability has been aborted, end it
            if (currentPhase >= phases.Length || aborted) {
                finalPhase.Tick(); // at its end, the end phase should deactivate this ability
                return;
            }

            // offer a tick to the current phase
            var phase = phases[currentPhase];
            phase.Tick();

            // advance to the next phase if the current one is done
            if (phase.Finished)
                currentPhase++;
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