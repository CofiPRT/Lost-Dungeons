using Game;
using UnityEngine;
using UnityEngine.UI;

namespace Camera {
    public class HUDController : MonoBehaviour {
        // singleton
        private static HUDController Instance { get; set; }

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;

            // handle player1
            player1HUD = (RectTransform)transform.Find("Player1");
            player1HealthBar = player1HUD.transform.Find("Bars/HealthBar").GetComponent<Image>();
            player1ManaBar = player1HUD.transform.Find("Bars/ManaBar").GetComponent<Image>();

            // handle player2
            player2HUD = (RectTransform)transform.Find("Player2");
            player2HealthBar = player2HUD.transform.Find("Bars/HealthBar").GetComponent<Image>();
            player2ManaBar = player2HUD.transform.Find("Bars/ManaBar").GetComponent<Image>();
        }

        private const float LerpSpeed = 5f;

        private static readonly Vector2 Position1 = new Vector2(0, -140);
        private static readonly Vector2 Position2 = new Vector2(0, -230);
        private static readonly Vector2 PositionIntermediary = new Vector2(0, -280);

        private const float Scale1 = 0.5f;
        private const float Scale2 = 0.35f;

        private RectTransform player1HUD;
        private RectTransform player2HUD;

        private Image player1HealthBar;
        private Image player2HealthBar;

        private Image player1ManaBar;
        private Image player2ManaBar;

        private RectTransform ControlledHUD => GameController.ControlledPlayer == GameController.Player1
            ? player1HUD
            : player2HUD;

        private RectTransform OtherHUD => ControlledHUD == player1HUD ? player2HUD : player1HUD;

        public static void LerpOtherSizeUp(float coefficient) {
            var scale = Mathf.Lerp(Scale2, Scale1, coefficient);
            Instance.OtherHUD.localScale = new Vector3(scale, scale, 1);

            // the other HUD must be brought towards the intermediary position
            Instance.OtherHUD.anchoredPosition = Vector2.Lerp(Position2, PositionIntermediary, coefficient);
        }

        public static void LerpPositions(float coefficient) {
            // the currently controlled HUD must be brought towards position 1
            Instance.ControlledHUD.anchoredPosition = Vector2.Lerp(PositionIntermediary, Position1, coefficient);

            // the other HUD must be brought towards the intermediary position
            Instance.OtherHUD.anchoredPosition = Vector2.Lerp(Position1, PositionIntermediary, coefficient);
        }

        public static void LerpOtherSizeDown(float coefficient) {
            var scale = Mathf.Lerp(Scale1, Scale2, coefficient);
            Instance.OtherHUD.localScale = new Vector3(scale, scale, 1);

            // the other HUD must be brought towards position 2
            Instance.OtherHUD.anchoredPosition = Vector2.Lerp(PositionIntermediary, Position2, coefficient);
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