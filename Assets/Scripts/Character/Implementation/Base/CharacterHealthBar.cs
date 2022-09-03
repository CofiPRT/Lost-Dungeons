using CameraScript;
using Game;
using Properties;
using UnityEngine;
using UnityEngine.UI;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        private const float LerpSpeed = 5f;

        private Canvas healthBarCanvas;
        private Image healthBarFill;

        private void AwakeHealthBar() {
            var instance = Team == Team.Enemy
                ? GameController.DefaultInstances.redHealthBar
                : GameController.DefaultInstances.greenHealthBar;
            healthBarCanvas = Instantiate(instance, GameController.SpawnContainer);
            healthBarFill = healthBarCanvas.transform.Find("Fill").GetComponent<Image>();
        }

        public void HideHealthBar() {
            healthBarCanvas.gameObject.SetActive(false);
        }

        private void UpdateHealthBar() {
            // update value
            healthBarFill.fillAmount = Mathf.Lerp(
                healthBarFill.fillAmount,
                Health / MaxHealth,
                Time.deltaTime * LerpSpeed
            );

            // hide when it reaches 0
            if (healthBarFill.fillAmount <= 0.001f)
                HideHealthBar();

            // update position - keep it above the character
            healthBarCanvas.transform.position = transform.position + Vector3.up * 1.5f;

            // update rotation - keep it facing the camera
            healthBarCanvas.transform.rotation = Quaternion.LookRotation(CameraController.Forward);
        }
    }
}