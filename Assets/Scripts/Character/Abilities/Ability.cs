using System;
using Character.Implementation.Player;
using UnityEngine;

namespace Character.Abilities {
    public interface IAbility {
        GenericPlayer User { get; }
        void StartCooldown();
        void Update();
        bool Use();
        void Reset();
        void OnAttack();
    }

    public abstract class Ability<T> : IAbility where T : IAbility {
        public GenericPlayer User { get; }
        private readonly float cooldown;

        private readonly bool checkIfBlocked;

        protected AbilityPhase<T>[] phases = Array.Empty<AbilityPhase<T>>();
        protected AbilityPhase<T> finalPhase;

        private int currentPhase;
        private float currentCooldown;
        protected bool active;
        private bool aborted;

        protected Ability(GenericPlayer user, float cooldown, bool checkIfBlocked = true) {
            User = user;
            this.cooldown = cooldown;
            this.checkIfBlocked = checkIfBlocked;
        }

        public virtual bool Use() {
            // if on cooldown, or another ability blocks this cast, don't start
            if (currentCooldown > 0 || (checkIfBlocked && User.CastBlocksAbilityUsage))
                return false;

            // if already active, offer a reactivation to the current phase
            if (active) {
                phases[currentPhase].OnReactivation();
            } else {
                Reset();
                active = true; // keep it active
            }

            return true;
        }

        public void Update() {
            currentCooldown = Mathf.Max(0, currentCooldown - User.DeltaTime);

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

        public void StartCooldown() {
            active = false;
            currentCooldown = cooldown;
        }

        internal void Abort() {
            aborted = true;
        }

        public void Reset() {
            currentCooldown = 0;
            currentPhase = 0;
            aborted = false;
            active = false;

            foreach (var phase in phases)
                phase.Reset();

            finalPhase.Reset();
        }

        public void OnAttack() {
            if (active)
                phases[currentPhase].OnAttack();
        }
    }
}