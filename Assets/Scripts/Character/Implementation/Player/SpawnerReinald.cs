using Game;
using UnityEngine;

namespace Character.Implementation.Player {
    public class SpawnerReinald : MonoBehaviour {
        private void Start() {
            var ownTransform = transform;
            GameController.SetPlayer2(
                Instantiate(
                    GameController.DefaultInstances.reinald,
                    ownTransform.position,
                    ownTransform.rotation,
                    GameController.SpawnContainer
                )
            );

            Destroy(gameObject);
        }
    }
}