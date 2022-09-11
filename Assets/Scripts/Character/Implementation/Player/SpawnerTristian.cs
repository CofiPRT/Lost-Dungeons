using Game;
using UnityEngine;

namespace Character.Implementation.Player {
    public class SpawnerTristian : MonoBehaviour {
        private void Start() {
            var ownTransform = transform;
            GameController.SetPlayer1(
                Instantiate(
                    GameController.DefaultInstances.tristian,
                    ownTransform.position,
                    ownTransform.rotation,
                    GameController.SpawnContainerPlayers
                )
            );

            Destroy(gameObject);
        }
    }
}