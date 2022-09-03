using System;
using UnityEngine;

namespace Character.Implementation.Enemy.Groups {
    public abstract class GenericGroup : MonoBehaviour {
        private void Start() {
            SpawnGroup();
            Destroy(gameObject);
        }

        protected abstract void SpawnGroup();
    }
}