using Camera.HUD;
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

            defaultInstances = GetComponent<Instances>();
            spawnContainer = transform.Find("SpawnContainer").transform;
        }

        private Instances defaultInstances;
        private Transform spawnContainer;

        public static Instances DefaultInstances => Instance.defaultInstances;
        public static Transform SpawnContainer => Instance.spawnContainer;

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
            player1 = Instantiate(defaultInstances.tristian, new Vector3(0, 10, 0), Quaternion.identity);
            player2 = Instantiate(defaultInstances.reinald, new Vector3(1, 10, 0), Quaternion.identity);

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

            HUDController.RefreshIcons();
        }

        public static void SpawnDebugEnemy() {
            Instantiate(
                DefaultInstances.enemyWhite,
                ControlledPlayer.Pos + ControlledPlayer.Forward * 5,
                Quaternion.identity,
                SpawnContainer
            );
        }

        public static void SpawnDebugProp() {
            Instantiate(
                DefaultInstances.bottle,
                ControlledPlayer.Pos + ControlledPlayer.Forward * 5,
                Quaternion.identity,
                SpawnContainer
            );
        }

        public static void SpawnDebugAlly() {
            Instantiate(
                DefaultInstances.ally,
                ControlledPlayer.Pos + ControlledPlayer.Forward * 5,
                Quaternion.identity,
                SpawnContainer
            );
        }
    }
}