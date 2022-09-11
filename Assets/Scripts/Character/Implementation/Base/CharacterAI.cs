using Character.Implementation.Base.AIActions;
using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        private BaseAIAction aiAction;

        public BaseAIAction AIAction {
            get => aiAction;
            set {
                if (aiAction == null)
                    aiAction = value;
                else if (value == null || value.priority >= aiAction.priority)
                    aiAction = value;
            }
        }

        protected BaseAICheck[] AIChecks { get; set; }
        private bool UseAI { get; set; } = true;

        public virtual void ActivateAI() {
            UseAI = true;
        }

        public virtual void DeactivateAI() {
            UseAI = false;
            OnAIDisable();
        }

        protected virtual void OnAIDisable() {
            // intentionally left blank
        }

        private void UpdateAI() {
            if (!UseAI)
                return;

            // perform the AI checks
            foreach (var check in AIChecks)
                check.Update();

            // offer a game tick to the current action if any
            AIAction?.Tick();
        }

        public void ForceMoveTo(Vector2 target) {
            AIAction = new AIMoveAction(this, target, priority: 3);
        }

        public abstract class BaseAICheck {
            private readonly GenericCharacter instance;
            private readonly float cooldownMin;
            private readonly float cooldownMax;
            private float cooldown;

            protected BaseAICheck(GenericCharacter instance, float cooldownMin = 0f, float cooldownMax = 1f) {
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
            protected internal readonly int priority;
            private readonly float maxDuration;

            private float duration;
            private bool started;

            protected BaseAIAction(
                GenericCharacter instance,
                int priority,
                float maxDuration = Mathf.Infinity
            ) {
                this.instance = instance;
                this.priority = priority;
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
                instance.aiAction = null;
            }
        }
    }
}