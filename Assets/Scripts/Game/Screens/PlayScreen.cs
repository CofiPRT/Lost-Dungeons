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

        private float spaceCooldown;
        private int spaceTaps;
        private float spaceHeldFor;
        private bool spaceHeldDown;

        public override void Update() {
            // handle switch ability
            if (spaceCooldown == 0)
                spaceTaps = 0;

            spaceCooldown = Mathf.Max(0f, spaceCooldown - Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space)) {
                if (spaceTaps == 0) {
                    spaceTaps = 1;
                    spaceCooldown = 0.35f;
                } else if (spaceTaps == 1 && spaceCooldown > 0) {
                    GameController.ControlledPlayer.StartTagTeam(true);
                }

                spaceHeldDown = true;
                spaceHeldFor = 0;
            }

            if (Input.GetKeyUp(KeyCode.Space))
                spaceHeldDown = false;

            if (spaceHeldDown)
                spaceHeldFor += Time.deltaTime;

            if (spaceHeldDown && spaceHeldFor > 0.5f)
                GameController.ControlledPlayer.StartTagTeam(false);
            else if (!spaceHeldDown && spaceCooldown == 0 && spaceTaps == 1)
                GameController.ControlledPlayer.AbilitySwitch.Use();

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