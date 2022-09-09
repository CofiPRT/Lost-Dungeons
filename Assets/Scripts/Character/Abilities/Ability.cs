using System;
using Character.Implementation.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Character.Abilities {
    public interface IAbility {
        SpriteGetter IconGetter { get; }
        GenericPlayer User { get; }
        float CooldownTime { get; }
        float CurrentCooldown { get; }
        bool Active { get; }

        void StartCooldown();
        void Update();
        bool Use();
        void Reset();
        void OnAttack();

        public delegate Sprite SpriteGetter();
    }

    public abstract class Ability<T> : IAbility where T : IAbility {
        public IAbility.SpriteGetter IconGetter { get; }
        public GenericPlayer User { get; }

        public float CooldownTime { get; }
        public float CurrentCooldown { get; private set; }
        public bool Active { get; private set; }

        private readonly bool checkIfBlocked;

        protected AbilityPhase<T>[] phases = Array.Empty<AbilityPhase<T>>();
        protected AbilityPhase<T> finalPhase;

        private int currentPhase;
        private bool aborted;

        protected Ability(
            GenericPlayer user,
            float cooldown,
            IAbility.SpriteGetter iconGetter,
            bool checkIfBlocked = true
        ) {
            User = user;
            IconGetter = iconGetter;
            CooldownTime = cooldown;
            this.checkIfBlocked = checkIfBlocked;
        }

        public virtual bool Use() {
            // if on cooldown, or another ability blocks this cast, don't start
            if (CurrentCooldown > 0 || (checkIfBlocked && User.CastBlocksAbilityUsage))
                return false;

            // if already active, offer a reactivation to the current phase
            if (Active) {
                phases[currentPhase].OnReactivation();
            } else {
                if (User.IsAttacking)
                    return false;

                Reset(); // prepare to run the ability again
                Active = true; // keep it active
            }

            return true;
        }

        public void Update() {
            CurrentCooldown = Mathf.Max(0, CurrentCooldown - User.DeltaTime);

            // only update while active
            if (!Active)
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
            Active = false;
            CurrentCooldown = CooldownTime;
        }

        internal void Abort() {
            aborted = true;
        }

        public void Reset() {
            CurrentCooldown = 0;
            currentPhase = 0;
            aborted = false;
            Active = false;

            foreach (var phase in phases)
                phase.Reset();

            finalPhase.Reset();
        }

        public void OnAttack() {
            if (Active)
                phases[currentPhase].OnAttack();
        }
    }
}