using Game;
using UnityEngine;
using UnityEngine.UI;

namespace Camera.HUD {
    public partial class HUDController : MonoBehaviour {
        // singleton
        public static HUDController Instance { get; set; }

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;

            AwakeHealthBars();
            AwakeCooldown();
        }

        private void Update() {
            UpdateHealthBars();
            UpdateCooldown();
        }
    }
}