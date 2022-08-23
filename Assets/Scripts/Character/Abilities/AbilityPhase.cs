using UnityEngine;

namespace Character.Abilities {
    public abstract class AbilityPhase {
        protected readonly Ability ability;
        private readonly float manaCostPerSecond;
        private readonly float maxDuration;
        private readonly bool useUserDelta;
        private readonly bool requireReactivation;

        private bool reactivated;
        private bool started;
        private float secondCounter;
        private float duration;

        private float Coefficient => Mathf.Clamp01(duration / maxDuration);
        private float DeltaTime => useUserDelta ? ability.user.DeltaTime : Time.deltaTime;
        internal bool Finished => duration > maxDuration;

        protected AbilityPhase(
            Ability ability,
            float manaCostPerSecond = 0,
            float maxDuration = float.MaxValue,
            bool useUserDelta = true,
            bool requireReactivation = false
        ) {
            this.ability = ability;
            this.manaCostPerSecond = manaCostPerSecond;
            this.maxDuration = maxDuration;
            this.useUserDelta = useUserDelta;
            this.requireReactivation = requireReactivation;
        }

        public void Tick() {
            if (requireReactivation && !reactivated)
                return;

            // if this is the first tick of the phase, start the ability
            if (!started) {
                started = true;
                OnStart();
            }

            // test if the ability should end
            duration += DeltaTime;
            if (Finished) {
                OnEnd();
                return; // duration exceeded
            }

            // consume the mana cost per second
            secondCounter += DeltaTime;
            if (secondCounter > 1) {
                // consume mana and reset the counter
                secondCounter -= 1;

                if (!ability.user.UseMana(manaCostPerSecond)) {
                    OnManaDepletion();
                    return; // not enough mana
                }
            }

            // the ability may receive another tick of update
            OnUpdate();
        }

        protected virtual void OnStart() {
            // intentionally blank
        }

        protected virtual void OnUpdate() {
            // intentionally blank
        }

        protected virtual void OnEnd() {
            // intentionally blank
        }

        protected virtual void OnManaDepletion() {
            // intentionally blank
        }

        internal virtual void Reset() {
            reactivated = false;
            started = false;
            secondCounter = 0;
            duration = 0;
        }

        protected void AbortAbility() {
            ability.Abort();
        }

        public virtual void OnReactivation() {
            reactivated = true;
        }
    }
}