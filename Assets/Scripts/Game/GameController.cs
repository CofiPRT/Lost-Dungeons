using System.Collections.Generic;
using CameraScript.HUD;
using Character.Implementation.Enemy;
using Character.Implementation.Player;
using Game.Screens;
using UnityEngine;

namespace Game {
    public partial class GameController : MonoBehaviour {
        // singleton
        private static GameController Instance { get; set; }

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;

            defaultInstances = GetComponent<Instances>();
            spawnContainer = transform.Find("SpawnContainer").transform;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public string levelID; // to be set in the inspector

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

        private readonly HashSet<GenericEnemy> aliveEnemies = new HashSet<GenericEnemy>();
        private readonly HashSet<LevelCollectible> aliveCollectibles = new HashSet<LevelCollectible>();

        private float prePauseTickSpeed;

        // debug
        public float debugMovementSpeedMultiplier = 1;
        public static float DebugMovementSpeedMultiplier => Instance.debugMovementSpeedMultiplier;

        /* static properties */

        public static GenericPlayer Player1 => Instance.player1;
        public static GenericPlayer Player2 => Instance.player2;
        public static GenericPlayer ControlledPlayer => Instance.controlledPlayer;

        public static GenericPlayer OtherPlayer =>
            Instance.controlledPlayer == Instance.player1 ? Instance.player2 : Instance.player1;

        public static HashSet<GenericEnemy> AliveEnemies => Instance.aliveEnemies;

        public static float GameTickSpeed {
            get => Instance.gameTickSpeed;
            set => Instance.gameTickSpeed = value;
        }

        public static float PlayerTickFactor {
            get => Instance.playerTickFactor;
            set => Instance.playerTickFactor = value;
        }

        public static void SetPlayer1(GenericPlayer player) {
            Instance.player1 = player;
            Instance.player1.HideHealthBar();

            if (Instance.player2 != null) // set a random player as the controlled player
                SetControlledPlayer(Random.value > 0.5f ? Instance.player1 : Instance.player2);
            else
                SetControlledPlayer(Instance.player1);
        }

        public static void SetPlayer2(GenericPlayer player) {
            Instance.player2 = player;
            Instance.player2.SetAI(false);
            Instance.player2.HideHealthBar();

            if (Instance.player1 != null) // set a random player as the controlled player
                SetControlledPlayer(Random.value > 0.5f ? Instance.player1 : Instance.player2);
            else
                SetControlledPlayer(Instance.player2);
        }

        private static void SetControlledPlayer(GenericPlayer player) {
            Instance.controlledPlayer = player;
            Instance.controlledPlayer.SetAI(false);

            if (OtherPlayer != null)
                OtherPlayer.SetAI(true);

            HUDController.RefreshIcons();
        }

        public static void ChangePlayers() {
            if (OtherPlayer == null)
                return;

            SetControlledPlayer(OtherPlayer);
        }

        public static void PauseGame() {
            Instance.prePauseTickSpeed = GameTickSpeed;
            GameTickSpeed = 0;

            ScreenController.AddScreen(new PauseScreen());
            HUDController.Pause();

            ShowCursor();
        }

        public static void ResumeGame() {
            GameTickSpeed = Instance.prePauseTickSpeed;
            ScreenController.RemoveScreen();

            HideCursor();
        }

        public static void ShowCursor() {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public static void HideCursor(bool center = true) {
            Cursor.visible = false;
            Cursor.lockState = center ? CursorLockMode.Locked : CursorLockMode.Confined;
        }

        public static bool AttemptFinish() {
            return Instance.aliveEnemies.Count == 0 && Instance.aliveCollectibles.Count == 0;
        }

        public static void AddCollectible(LevelCollectible collectible) {
            Instance.aliveCollectibles.Add(collectible);
        }

        public static void RemoveCollectible(LevelCollectible collectible) {
            Instance.aliveCollectibles.Remove(collectible);
        }

        private void Update() {
            UpdateGameEnd();
        }

        public static void SpawnDebugEnemy() {
            Instantiate(
                DefaultInstances.enemyWhite,
                ControlledPlayer.Pos + ControlledPlayer.Forward * 5,
                Quaternion.identity,
                SpawnContainer
            );

            Debug.Log("Spawned enemy");
        }

        public static void SpawnDebugProp() {
            Instantiate(
                DefaultInstances.bottle,
                ControlledPlayer.Pos + ControlledPlayer.Forward * 5,
                Quaternion.identity,
                SpawnContainer
            );

            Debug.Log("Spawned prop");
        }

        public static void SpawnDebugAlly() {
            Instantiate(
                DefaultInstances.ally,
                ControlledPlayer.Pos + ControlledPlayer.Forward * 5,
                Quaternion.identity,
                SpawnContainer
            );

            Debug.Log("Spawned ally");
        }

        public static void DebugKillAllEnemies() {
            foreach (var enemy in AliveEnemies)
                enemy.TakeDamage(1000);

            Debug.Log("Killed all enemies");
        }

        public static void DebugToggleTurboGameTick() {
            Instance.debugMovementSpeedMultiplier = Instance.debugMovementSpeedMultiplier == 10 ? 1 : 10;

            Debug.Log("Debug movement speed multiplier: " + Instance.debugMovementSpeedMultiplier);
        }
    }
}