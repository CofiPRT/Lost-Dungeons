using Game;
using UnityEngine;

namespace Character.Implementation.Ally {
    public class SpawnerAlly : MonoBehaviour {
        private void Start() {
            var ownTransform = transform;
            Instantiate(
                GameController.DefaultInstances.ally,
                ownTransform.position,
                ownTransform.rotation,
                GameController.SpawnContainer
            );

            Destroy(gameObject);
        }
    }
}