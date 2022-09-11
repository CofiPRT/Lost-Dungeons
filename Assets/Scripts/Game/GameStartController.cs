using CameraScript.HUD;
using CameraScript.Path;
using UnityEngine;

namespace Game {
    public class GameStartController : MonoBehaviour {
        private PathFollower cameraPathFollower;

        private void Awake() {
            cameraPathFollower = transform.Find("CameraPath").GetComponent<PathFollower>();

            // subscribe to the event
            cameraPathFollower.FinishEvents += () => {
                // destroy the camera follower and start the game
                Destroy(cameraPathFollower.gameObject);
                GameController.ControlAllowed = true;

                // show the HUD
                HUDController.ShowHUD();

                // start AI
                if (GameController.OtherPlayer != null)
                    GameController.OtherPlayer.ActivateAI();

                // show collectibles
                foreach (var collectible in GameController.AliveCollectibles)
                    collectible.gameObject.SetActive(true);
            };
        }

        private void Start() {
            // ensure HUD elements are hidden
            HUDController.HideHUD();
        }
    }
}