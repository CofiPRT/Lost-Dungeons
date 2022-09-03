using System.Collections.Generic;
using Game.Screens;
using UnityEngine;

namespace Game {
    public class ScreenController : MonoBehaviour {
        // singleton
        private static ScreenController Instance { get; set; }

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;

            screens.Push(new PlayScreen());
        }

        private readonly Stack<GameScreen> screens = new Stack<GameScreen>();

        private void Update() {
            screens.Peek()?.Update();
        }

        private void LateUpdate() {
            screens.Peek()?.LateUpdate();
        }

        public static void AddScreen(GameScreen screen) {
            Instance.screens.Push(screen);
        }

        public static void RemoveScreen() {
            Instance.screens.Pop();
        }
    }
}