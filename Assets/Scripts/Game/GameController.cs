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

        /* game data */

        private GenericPlayer controllerPlayer;
        private GenericPlayer player1;
        private GenericPlayer player2;

        private float gameTickSpeed = 1;
        private float playerTickFactor = 1;

        /* static properties */

        public static GenericPlayer Player1 => Instance.player1;
        public static GenericPlayer Player2 => Instance.player2;
        public static GenericPlayer ControlledPlayer => Instance.controllerPlayer;

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

            controllerPlayer = player1;
            controllerPlayer.SetAI(false);
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