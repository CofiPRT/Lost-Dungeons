﻿using Character.Implementation.Enemy;
using UnityEngine;

namespace Character.Implementation.Base.AIActions {
    public class AIBlockAction : GenericCharacter.BaseAIAction {
        private new readonly GenericCharacter instance;
        private readonly Vector2 direction;

        public AIBlockAction(GenericCharacter instance, Vector2 direction)
            : base(instance, Random.Range(2, 3)) {
            this.instance = instance;
            this.direction = direction;
        }

        protected override void OnStart() {
            instance.StartBlocking(direction);
        }

        protected override void OnEnd() {
            instance.StopBlocking();
            base.OnEnd();
        }
    }
}