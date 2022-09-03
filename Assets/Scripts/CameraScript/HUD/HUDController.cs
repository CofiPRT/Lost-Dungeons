using UnityEngine;

namespace CameraScript.HUD {
    public partial class HUDController : MonoBehaviour {
        // singleton
        public static HUDController Instance { get; set; }

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;

            AwakeCooldown();
            AwakeGameEndOverlay();
            AwakePauseMenu();
        }

        private void Start() {
            StartHealthBars();
        }

        private void Update() {
            UpdateHealthBars();
            UpdateCooldown();
            UpdatePauseMenu();
        }
    }
}