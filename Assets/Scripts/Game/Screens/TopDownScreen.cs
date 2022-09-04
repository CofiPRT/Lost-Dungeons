using CameraScript;
using UnityEngine;

namespace Game.Screens {
    public class TopDownScreen : GameScreen {
        public override void Update() {
            if (Input.GetKeyDown(KeyCode.E)) {
                TopdownViewController.Deactivate();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                TopdownViewController.OnEscapePressed();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                TopdownViewController.OnRightClick();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
                TopdownViewController.OnMouseClick();
        }
    }
}