using Character.Implementation.Enemy;
using Character.Implementation.Player;
using UnityEngine;

namespace Game {
    public class GameController : MonoBehaviour {
        // singleton
        private static GameController Instance { get; set; }

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        /* default instances - to be set in inspector */
        public GenericPlayer defaultPlayer1;
        public GenericPlayer defaultPlayer2;
        public GenericEnemy defaultEnemyWhite;
        public Canvas defaultHealthBar;

        /* game data */

        private GenericPlayer controlledPlayer;
        private GenericPlayer player1;
        private GenericPlayer player2;

        private float gameTickSpeed = 1;
        private float playerTickFactor = 1;

        /* static properties */

        public static GenericPlayer Player1 => Instance.player1;
        public static GenericPlayer Player2 => Instance.player2;
        public static GenericPlayer ControlledPlayer => Instance.controlledPlayer;

        public static GenericPlayer OtherPlayer =>
            Instance.controlledPlayer == Instance.player1 ? Instance.player2 : Instance.player1;

        public static float GameTickSpeed {
            get => Instance.gameTickSpeed;
            set => Instance.gameTickSpeed = value;
        }

        public static float PlayerTickFactor {
            get => Instance.playerTickFactor;
            set => Instance.playerTickFactor = value;
        }

        private void Start() {
            player1 = Instantiate(defaultPlayer1, new Vector3(0, 1, 0), Quaternion.identity);
            player2 = Instantiate(defaultPlayer2, new Vector3(1, 0, 0), Quaternion.identity);

            controlledPlayer = player1;
            controlledPlayer.SetAI(false);

            player1.HideHealthBar();
            player2.HideHealthBar();
        }

        public static void ChangePlayers() {
            Instance.controlledPlayer.SetAI(true);
            Instance.controlledPlayer =
                Instance.controlledPlayer == Instance.player1 ? Instance.player2 : Instance.player1;
            Instance.controlledPlayer.SetAI(false);
        }

        public static Canvas InstantiateHealthBar(Transform parent) {
            return Instantiate(Instance.defaultHealthBar, parent);
        }

        public static void SpawnDebugEnemy() {
            Instantiate(
                Instance.defaultEnemyWhite,
                ControlledPlayer.Pos + ControlledPlayer.Forward * 5,
                Quaternion.identity
            );
        }
    }
}