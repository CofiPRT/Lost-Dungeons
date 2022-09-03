using CameraScript.HUD;
using UnityEngine;

namespace Game.Screens {
    public class PauseScreen : GameScreen {
        public override void Update() {
            if (Input.GetKeyDown(KeyCode.Escape))
                HUDController.OnEscapePressed();
        }
    }
}