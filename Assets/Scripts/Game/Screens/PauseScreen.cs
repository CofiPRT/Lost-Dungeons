using CameraScript.HUD;
using Menu;
using UnityEngine;

namespace Game.Screens {
    public class PauseScreen : GameScreen {
        public override void Update() {
            if (Input.GetKeyDown(KeyCode.Escape))
                PauseMenuController.OnEscapePressed();
        }
    }
}