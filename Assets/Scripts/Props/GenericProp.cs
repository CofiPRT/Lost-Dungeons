using System;
using Character.Implementation.Base;
using Game;
using UnityEngine;

namespace Props {
    public class GenericProp : MonoBehaviour {
        private const float Speed = 5f;
        private GenericCharacter propTarget;

        public void LaunchTowards(GenericCharacter target) {
            propTarget = target;
        }

        private void Update() {
            if (!propTarget)
                return;

            // lerp position towards target
            transform.position = Vector3.Lerp(
                transform.position,
                propTarget.transform.position,
                GameController.GameTickSpeed * Speed
            );
        }
    }
}