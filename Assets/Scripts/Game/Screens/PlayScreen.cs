using Camera;
using UnityEngine;

namespace Game.Screens {
    public class PlayScreen : GameScreen {
        public override void LateUpdate() {
            // update camera rotation
            var horizontalMovement = Input.GetAxis("Mouse X");
            var verticalMovement = Input.GetAxis("Mouse Y");
            CameraController.Instance.ApplyInput(horizontalMovement, verticalMovement);
        }

        private bool prevPressedP;

        public override void Update() {
            // handle switch ability
            if (Input.GetKey(KeyCode.Space)) {
                GameController.ControlledPlayer.AbilitySwitch.Use();
                return;
            }

            // handle player movement
            var direction = Vector2.zero;

            if (Input.GetKey(KeyCode.W)) direction += CameraController.Forward2D;

            if (Input.GetKey(KeyCode.S)) direction -= CameraController.Forward2D;

            if (Input.GetKey(KeyCode.D)) direction += CameraController.Right2D;

            if (Input.GetKey(KeyCode.A)) direction -= CameraController.Right2D;

            var dodging = Input.GetKey(KeyCode.Z);
            var startedDodge = false;

            // dodging has priority over other actions
            if (dodging && direction.magnitude > 0)
                startedDodge = GameController.ControlledPlayer.StartDodge(direction);

            if (!startedDodge) {
                var running = Input.GetKey(KeyCode.LeftShift);

                if (direction.magnitude > 0)
                    GameController.ControlledPlayer.ApplyMovement(
                        direction.normalized,
                        running,
                        true
                    );

                // handle player attack
                if (Input.GetKey(KeyCode.Mouse0)) {
                    var attackDirection =
                        direction.magnitude > 0 ? direction : GameController.ControlledPlayer.Forward2D;
                    GameController.ControlledPlayer.StartAttack(attackDirection);
                }

                // handle player block
                if (Input.GetKey(KeyCode.Mouse1))
                    GameController.ControlledPlayer.StartBlocking(CameraController.Forward2D);
                else
                    GameController.ControlledPlayer.StopBlocking();
            }

            // handle debug spawning
            if (Input.GetKeyDown(KeyCode.P) && !prevPressedP)
                GameController.SpawnDebugEnemy();

            prevPressedP = Input.GetKey(KeyCode.P);
        }
    }
}