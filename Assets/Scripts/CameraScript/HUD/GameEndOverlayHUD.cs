using TMPro;
using UnityEngine;

namespace CameraScript.HUD {
    public partial class HUDController {
        private CanvasGroup finishOverlay;
        private CanvasGroup deathOverlay;
        private CanvasGroup darkOverlay;
        private TextMeshProUGUI deadPlayerText;

        private void AwakeGameEndOverlay() {
            finishOverlay = transform.Find("GameEndOverlays/GameFinishOverlay").GetComponent<CanvasGroup>();
            deathOverlay = transform.Find("GameEndOverlays/GameDeathOverlay").GetComponent<CanvasGroup>();
            darkOverlay = transform.Find("GameEndOverlays/DarkOverlay").GetComponent<CanvasGroup>();

            deadPlayerText = deathOverlay.transform.Find("DeadPlayerText").GetComponent<TextMeshProUGUI>();
        }

        public static void LerpFinishTransparency(float coefficient) {
            Instance.finishOverlay.alpha = coefficient;
        }

        public static void LerpDeathTransparency(float coefficient) {
            Instance.deathOverlay.alpha = coefficient;
        }

        public static void LerpDarkTransparency(float coefficient) {
            Instance.darkOverlay.alpha = coefficient;
        }

        public static void SetDeadPlayerName(string name) {
            Instance.deadPlayerText.text = $"{name} Died";
        }
    }
}