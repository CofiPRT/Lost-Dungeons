using Character.Implementation.Player;
using UnityEngine;

namespace Game.Objects {
    public class LevelFinisher : MonoBehaviour {
        private bool finished;

        private void OnTriggerStay(Collider other) {
            if (finished) return;

            // if the controlled player is in the trigger, test if the game should finish
            var player = other.GetComponent<GenericPlayer>();

            if (player != GameController.ControlledPlayer)
                return;

            finished = GameController.AttemptFinish();

            if (finished)
                GameController.StartFinish();
        }
    }
}