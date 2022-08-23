using UnityEngine;
using Random = UnityEngine.Random;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public BaseAIAction AIAction { get; set; }
        protected BaseAICheck[] AIChecks { get; set; }
        private bool UseAI { get; set; }

        public void SetAI(bool useAI) {
            var prevAI = UseAI;
            UseAI = useAI;

            // if the AI has been toggled, remove the action
            if (prevAI != UseAI)
                AIAction = null;
        }

        public void UpdateAI() {
            if (!UseAI)
                return;

            // perform the AI checks
            foreach (var check in AIChecks)
                check.Update();

            // offer a game tick to the current action if any
            AIAction?.Tick();
        }

        public abstract class BaseAICheck {
            protected readonly GenericCharacter instance;
            private readonly float cooldownMin;
            private readonly float cooldownMax;
            private float cooldown;

            public BaseAICheck(GenericCharacter instance, float cooldownMin = 0f, float cooldownMax = 1f) {
                this.instance = instance;
                this.cooldownMin = cooldownMin;
                this.cooldownMax = cooldownMax;
            }

            public void Update() {
                cooldown = Mathf.Max(0f, cooldown - instance.DeltaTime);
                if (cooldown > 0)
                    return;

                cooldown = Random.Range(cooldownMin, cooldownMax);
                Perform();
            }

            protected abstract void Perform();
        }

        public abstract class BaseAIAction {
            protected readonly GenericCharacter instance;
            private readonly float maxDuration;

            private float duration;
            private bool started;

            public BaseAIAction(GenericCharacter instance, float maxDuration = Mathf.Infinity) {
                this.instance = instance;
                this.maxDuration = maxDuration;
            }

            public void Tick() {
                if (!started) {
                    OnStart();
                    started = true;
                }

                OnUpdate();

                duration += instance.DeltaTime;
                if (duration >= maxDuration)
                    OnEnd();
            }

            protected virtual void OnStart() {
                // intentionally left blank
            }

            protected virtual void OnUpdate() {
                // intentionally left blank
            }

            protected virtual void OnEnd() {
                instance.AIAction = null;
            }
        }
    }
}