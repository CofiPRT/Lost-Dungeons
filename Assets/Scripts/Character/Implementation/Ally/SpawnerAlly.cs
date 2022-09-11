using Game;
using UnityEngine;

namespace Character.Implementation.Ally {
    public class SpawnerAlly : MonoBehaviour {
        private void Start() {
            var ownTransform = transform;
            var spawnedAlly = Instantiate(
                GameController.DefaultInstances.ally,
                ownTransform.position,
                ownTransform.rotation,
                GameController.SpawnContainerAllies
            );

            GameController.AliveAllies.Add(spawnedAlly);
            spawnedAlly.DeactivateAI();

            Destroy(gameObject);
        }
    }
}