using CameraScript.HUD;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game {
    public partial class GameController {
        private bool finished;
        private bool dead;
        private float endTime;

        private void UpdateGameEnd() {
            if (!finished && !dead)
                return;

            // lerp overlays
            if (endTime < 2) {
                var coefficient = endTime / 2;

                if (finished)
                    HUDController.LerpFinishTransparency(coefficient);
                else
                    HUDController.LerpDeathTransparency(coefficient);
            } else if (endTime < 7) {
                // finish previous lerp
                if (finished)
                    HUDController.LerpFinishTransparency(1);
                else
                    HUDController.LerpDeathTransparency(1);
            } else if (endTime < 10) {
                var coefficient = (endTime - 7) / 3;

                HUDController.LerpDarkTransparency(coefficient);
            } else {
                // switch back to level select
                SceneManager.LoadScene(0);
            }

            endTime += Time.deltaTime;
        }

        public static void StartFinish() {
            Instance.finished = true;
            PlayerPrefs.SetInt("level." + Instance.levelID + ".finished", 1);
        }

        public static void StartDeath(string deadPlayerName) {
            Instance.dead = true;
            HUDController.SetDeadPlayerName(deadPlayerName);
        }
    }
}