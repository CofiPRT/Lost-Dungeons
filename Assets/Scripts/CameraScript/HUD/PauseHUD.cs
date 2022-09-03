using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CameraScript.HUD {
    public partial class HUDController {
        private const float PauseLerpSpeed = 10f;
        private const float LerpEpsilon = 0.001f;

        private CanvasGroup pauseMenuRoot;
        private Transform mainPauseButtons;
        private Transform exitConfirmationButtons;

        private GraphicRaycaster raycaster;

        private Transform activeButtons;

        private bool isPaused;

        private void AwakePauseMenu() {
            pauseMenuRoot = transform.Find("PauseMenu").GetComponent<CanvasGroup>();
            mainPauseButtons = pauseMenuRoot.transform.Find("MainPauseButtons");
            exitConfirmationButtons = pauseMenuRoot.transform.Find("ExitConfirmationButtons");
            raycaster = pauseMenuRoot.transform.GetComponent<GraphicRaycaster>();

            activeButtons = mainPauseButtons;
            mainPauseButtons.gameObject.SetActive(true);
            exitConfirmationButtons.gameObject.SetActive(false);
            raycaster.enabled = false;
        }

        private void ChangeActiveButtons(Transform newActiveButtons) {
            activeButtons.gameObject.SetActive(false);
            activeButtons = newActiveButtons;
            activeButtons.gameObject.SetActive(true);
        }

        public void ToMainPauseButtons() {
            ChangeActiveButtons(mainPauseButtons);
        }

        public void OpenExitConfirmationButtons() {
            ChangeActiveButtons(exitConfirmationButtons);
        }

        public static void OnEscapePressed() {
            if (Instance.activeButtons == Instance.exitConfirmationButtons) {
                Instance.ToMainPauseButtons();
                return;
            }

            Unpause();
        }

        public void OnExitConfirmation() {
            SceneManager.LoadScene(0);
        }

        public static void Pause() {
            Instance.activeButtons = Instance.mainPauseButtons;
            Instance.isPaused = true;
        }

        public static void Unpause() {
            Instance.isPaused = false;
            Instance.raycaster.enabled = false;
            GameController.ResumeGame();
        }

        private void UpdatePauseMenu() {
            var lerpTarget = isPaused ? 1f : 0f;
            pauseMenuRoot.alpha = Mathf.Lerp(pauseMenuRoot.alpha, lerpTarget, PauseLerpSpeed * Time.deltaTime);

            if (Mathf.Abs(pauseMenuRoot.alpha - lerpTarget) < LerpEpsilon) {
                pauseMenuRoot.alpha = lerpTarget;

                if (isPaused)
                    raycaster.enabled = true;
            }
        }
    }
}