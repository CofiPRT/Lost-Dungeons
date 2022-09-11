using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu {
    public class PauseMenuController : MonoBehaviour {
        // singleton
        private static PauseMenuController Instance { get; set; }

        private CanvasGroup canvasGroup;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;

            canvasGroup = GetComponent<CanvasGroup>();
            mainPauseButtons = transform.Find("MainPauseButtons");
            exitConfirmationButtons = transform.Find("ExitConfirmationButtons");
            raycaster = transform.GetComponent<GraphicRaycaster>();

            activeButtons = mainPauseButtons;
            mainPauseButtons.gameObject.SetActive(true);
            exitConfirmationButtons.gameObject.SetActive(false);
            raycaster.enabled = false;
        }

        private const float PauseLerpSpeed = 10f;
        private const float LerpEpsilon = 0.001f;

        private Transform mainPauseButtons;
        private Transform exitConfirmationButtons;

        private GraphicRaycaster raycaster;

        private Transform activeButtons;

        private bool isPaused;

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

        private void Update() {
            var lerpTarget = isPaused ? 1f : 0f;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, lerpTarget, PauseLerpSpeed * Time.deltaTime);

            if (Mathf.Abs(canvasGroup.alpha - lerpTarget) < LerpEpsilon) {
                canvasGroup.alpha = lerpTarget;

                if (isPaused)
                    raycaster.enabled = true;
            }
        }
    }
}