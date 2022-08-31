using Game;
using UnityEngine;
using UnityEngine.UI;

namespace Camera {
    public class HUDController : MonoBehaviour {
        private const float LerpSpeed = 5f;

        private RectTransform player1HUD;
        private RectTransform player2HUD;

        private Image player1HealthBar;
        private Image player2HealthBar;

        private Image player1ManaBar;
        private Image player2ManaBar;

        private void Awake() {
            // handle player1
            player1HUD = (RectTransform)transform.Find("Player1");
            player1HealthBar = player1HUD.transform.Find("Bars/HealthBar").GetComponent<Image>();
            player1ManaBar = player1HUD.transform.Find("Bars/ManaBar").GetComponent<Image>();

            // handle player2
            player2HUD = (RectTransform)transform.Find("Player2");
            player2HealthBar = player2HUD.transform.Find("Bars/HealthBar").GetComponent<Image>();
            player2ManaBar = player2HUD.transform.Find("Bars/ManaBar").GetComponent<Image>();
        }

        private void Update() {
            // lerp players' health and mana bars
            player1HealthBar.fillAmount = Mathf.Lerp(
                player1HealthBar.fillAmount,
                GameController.Player1.Health / GameController.Player1.MaxHealth,
                Time.deltaTime * LerpSpeed
            );

            player1ManaBar.fillAmount = Mathf.Lerp(
                player1ManaBar.fillAmount,
                GameController.Player1.Mana / GameController.Player1.MaxMana,
                Time.deltaTime * LerpSpeed
            );

            player2HealthBar.fillAmount = Mathf.Lerp(
                player2HealthBar.fillAmount,
                GameController.Player2.Health / GameController.Player2.MaxHealth,
                Time.deltaTime * LerpSpeed
            );

            player2ManaBar.fillAmount = Mathf.Lerp(
                player2ManaBar.fillAmount,
                GameController.Player2.Mana / GameController.Player2.MaxMana,
                Time.deltaTime * LerpSpeed
            );
        }
    }
}