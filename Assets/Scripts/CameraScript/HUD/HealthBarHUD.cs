using Character.Implementation.Player;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace CameraScript.HUD {
    public partial class HUDController {
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

        private static GenericPlayer Player => GameController.ControlledPlayer;

        private RectTransform ControlledHUD => Player == GameController.Player1
            ? player1HUD
            : player2HUD;

        private RectTransform OtherHUD => ControlledHUD == player1HUD ? player2HUD : player1HUD;

        private void StartHealthBars() {
            // handle player1
            player1HUD = transform.Find("Player1").GetComponent<RectTransform>();
            player1HealthBar = player1HUD.transform.Find("Bars/HealthBar").GetComponent<Image>();
            player1ManaBar = player1HUD.transform.Find("Bars/ManaBar").GetComponent<Image>();
            player1HUD.gameObject.SetActive(false);

            // handle player2
            player2HUD = transform.Find("Player2").GetComponent<RectTransform>();
            player2HealthBar = player2HUD.transform.Find("Bars/HealthBar").GetComponent<Image>();
            player2ManaBar = player2HUD.transform.Find("Bars/ManaBar").GetComponent<Image>();
            player2HUD.gameObject.SetActive(false);
        }

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

        private bool initialized;

        private void UpdateHealthBars() {
            Instance.player1HUD.gameObject.SetActive(GameController.Player1 != null);
            Instance.player2HUD.gameObject.SetActive(GameController.Player2 != null);

            // ensure the HUD of the controlled player is at the top
            if (!initialized && GameController.ControlledPlayer == GameController.Player2) {
                Instance.player2HUD.anchoredPosition = Position1;
                Instance.player2HUD.localScale = new Vector3(Scale1, Scale1, 1);

                Instance.player1HUD.anchoredPosition = Position2;
                Instance.player1HUD.localScale = new Vector3(Scale2, Scale2, 1);

                initialized = true;
            }

            // lerp players' health and mana bars
            if (GameController.Player1 != null) {
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
            }

            if (GameController.Player2 != null) {
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
}