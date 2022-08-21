using Character.Scripts.Generic;
using UnityEngine;

namespace Character.Scripts.Abilities {
    public abstract class Ability {
        internal readonly CasterCharacter user;
        private readonly AbilityPhase[] phases;
        private readonly bool useUserDeltaTime;
        private readonly float cooldown;

        private int currentPhase;
        private float currentCooldown;
        private bool active;

        internal float DeltaTime => useUserDeltaTime ? user.DeltaTime : Time.deltaTime;

        protected Ability(AbilityPhase[] phases, CasterCharacter user, float cooldown, bool useUserDeltaTime = true) {
            this.phases = phases;
            this.user = user;
            this.cooldown = cooldown;
            this.useUserDeltaTime = useUserDeltaTime;
        }

        public bool Use() {
            // if on cooldown, don't start
            if (currentCooldown > 0)
                return false;

            currentPhase = 0;
            active = true;

            return true;
        }

        public void Update() {
            currentCooldown = Mathf.Max(0, currentCooldown - DeltaTime);

            // only update while active
            if (!active)
                return;

            // a true result indicates the ability can continue running
            if (phases[currentPhase].Update()) return;
            
            // otherwise, the ability is stopped
            EndAbility();
        }

        public void AdvancePhase() {
            currentPhase++;
        }

        private void EndAbility() {
            active = false;
            currentCooldown = cooldown;
        }

        public void Reset() {
            foreach (var phase in phases)
                phase.Reset();
        }
    }

    public abstract class AbilityPhase {
        private readonly Ability ability;
        private readonly float initialManaCost;
        private readonly float manaCostPerSecond;
        private readonly float maxDuration;

        private bool started;
        private float secondCounter;
        private float duration;

        protected AbilityPhase(
            Ability ability,
            float initialManaCost = 0,
            float manaCostPerSecond = 0,
            float maxDuration = float.MaxValue
        ) {
            this.ability = ability;
            this.initialManaCost = initialManaCost;
            this.manaCostPerSecond = manaCostPerSecond;
            this.maxDuration = maxDuration;
        }

        public bool Update() {
            // if this is the first tick of the ability, attempt to consume the initial amount of mana
            if (!started) {
                if (!ability.user.UseMana(initialManaCost))
                    return false; // not enough mana
                
                // the ability has been successfully started
                started = true;
                
                OnStart();
                return true;
            }
            
            // test if the ability should end
            duration += ability.DeltaTime;
            if (duration > maxDuration) {
                OnEnd();
                return false; // duration exceeded
            }

            // consume the mana cost per second
            secondCounter += ability.DeltaTime;
            if (secondCounter > 1) {
                // consume mana and reset the counter
                secondCounter -= 1;
                if (!ability.user.UseMana(manaCostPerSecond)) {
                    OnEnd();
                    return false; // not enough mana
                }
            }
            
            // the ability may receive another tick of update
            OnTick();
            return true;
        }

        protected virtual void OnStart() {
            OnTick(); // by default, offer one tick of update
        }
        
        protected virtual void OnTick() {
            // intentionally blank
        }
        
        protected virtual void OnEnd() {
            ability.AdvancePhase();
        }

        internal virtual void Reset() {
            started = false;
            secondCounter = 0;
            duration = 0;
        }
    }
}