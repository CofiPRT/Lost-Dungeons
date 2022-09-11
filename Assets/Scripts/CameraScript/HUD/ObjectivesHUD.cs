using Game;
using TMPro;
using UnityEngine;

namespace CameraScript.HUD {
    public partial class HUDController {
        private TextMeshProUGUI objectivesText;
        private Transform objectivesRoot;

        private void AwakeObjectives() {
            objectivesRoot = transform.Find("Objectives");
            objectivesText = objectivesRoot.Find("ObjectivesText").GetComponent<TextMeshProUGUI>();
        }

        private void UpdateObjectives() {
            var message = "";

            if (GameController.AliveCollectibles.Count > 0)
                message += "Find collectibles: " + GameController.AliveCollectibles.Count + "\n";

            if (GameController.AliveEnemies.Count > 0)
                message += "Kill enemies: " + GameController.AliveEnemies.Count + "\n";

            if (message == "" && !GameController.IsFinished)
                message += "Find the well";

            objectivesRoot.gameObject.SetActive(message != "");
            objectivesText.text = message;
        }
    }
}