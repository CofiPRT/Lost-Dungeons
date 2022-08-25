using Character.Implementation.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game {
    public class GameController : MonoBehaviour {
        // singleton
        public static GameController Instance { get; private set; }

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        /* game data */

        private GenericPlayer controllerPlayer;

        // public to allow the Unity inspector to set it
        public GenericPlayer player1;
        public GenericPlayer player2;

        private float gameTickSpeed;
        private float playerTickFactor;

        /* static properties */

        public static GenericPlayer Player1 => Instance.player1;
        public static GenericPlayer Player2 => Instance.player2;
        public static GenericPlayer ControllerPlayer => Instance.controllerPlayer;

        public static float GameTickSpeed {
            get => Instance.gameTickSpeed;
            set => Instance.gameTickSpeed = value;
        }

        public static float PlayerTickFactor {
            get => Instance.playerTickFactor;
            set => Instance.playerTickFactor = value;
        }


        private void Start() {
            player1 = Instantiate(player1, new Vector3(0, 1, 0), Quaternion.identity);
            player2 = Instantiate(player2, new Vector3(1, 0, 0), Quaternion.identity);

            controllerPlayer = player1;
        }
    }
}