using System.Collections.Generic;
using Game.Screens;
using UnityEngine;

namespace Game {
    public class ScreenController : MonoBehaviour {
        private readonly Stack<GameScreen> screens = new Stack<GameScreen>();

        private void Awake() {
            screens.Push(new PlayScreen());
        }

        private void Update() {
            screens.Peek()?.Update();
        }

        private void LateUpdate() {
            screens.Peek()?.LateUpdate();
        }
    }
}