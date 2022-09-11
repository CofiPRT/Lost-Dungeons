using UnityEngine;

namespace CameraScript.HUD {
    public partial class HUDController : MonoBehaviour {
        // singleton
        private static HUDController Instance { get; set; }

        private CanvasGroup canvasGroup;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;

            canvasGroup = GetComponent<CanvasGroup>();

            AwakeCooldown();
            AwakeGameEndOverlay();
            AwakeObjectives();
        }

        private void Start() {
            StartHealthBars();
        }

        private const float AlphaLerpSpeed = 0.4f;
        private float targetAlpha;

        private void Update() {
            UpdateHealthBars();
            UpdateCooldown();
            UpdateObjectives();
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * AlphaLerpSpeed);

            // apply epsilon
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.01f)
                canvasGroup.alpha = targetAlpha;
        }

        public static void ShowHUD() {
            Instance.targetAlpha = 1f;
        }

        public static void HideHUD(bool instant = true) {
            Instance.targetAlpha = 0f;

            if (instant)
                Instance.canvasGroup.alpha = 0f;
        }
    }
}