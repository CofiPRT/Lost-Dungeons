using CameraScript;
using UnityEngine;

namespace Game.Screens {
    public class PlayScreen : GameScreen {
        public override void LateUpdate() {
            // update camera rotation
            var horizontalMovement = Input.GetAxis("Mouse X");
            var verticalMovement = Input.GetAxis("Mouse Y");
            CameraController.Instance.ApplyInput(horizontalMovement, verticalMovement);
        }

        private float spaceCooldown;
        private int spaceTaps;
        private float spaceHeldFor;
        private bool spaceHeldDown;

        public override void Update() {
            // handle pause
            if (Input.GetKeyDown(KeyCode.Escape)) {
                GameController.PauseGame();
                return;
            }

            if (!GameController.ControlAllowed)
                return;

            // handle topdown view
            if (Input.GetKeyDown(KeyCode.E) && TopdownViewController.Activate())
                return;

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
                    return;
                }

                spaceHeldDown = true;
                spaceHeldFor = 0;
            }

            if (Input.GetKeyUp(KeyCode.Space))
                spaceHeldDown = false;

            if (spaceHeldDown)
                spaceHeldFor += Time.deltaTime;

            if (spaceHeldDown && spaceHeldFor > 0.5f) {
                GameController.ControlledPlayer.StartTagTeam(false);
                return;
            }

            if (!spaceHeldDown && spaceCooldown == 0 && spaceTaps == 1) {
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

            // handle dodge
            if (dodging && direction.magnitude > 0)
                if (GameController.ControlledPlayer.StartDodge(direction))
                    return;

            // handle abilities
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                GameController.ControlledPlayer.Ability1.Use();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                GameController.ControlledPlayer.Ability2.Use();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                GameController.ControlledPlayer.Ultimate.Use();
                return;
            }

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

            // handle debug spawning
            if (Input.GetKeyDown(KeyCode.P))
                GameController.SpawnDebugEnemy();

            if (Input.GetKeyDown(KeyCode.L))
                GameController.SpawnDebugProp();

            if (Input.GetKeyDown(KeyCode.M))
                GameController.SpawnDebugAlly();

            if (Input.GetKeyDown(KeyCode.O))
                GameController.DebugKillAllEnemies();

            if (Input.GetKeyDown(KeyCode.I))
                GameController.DebugToggleTurboGameTick();
        }
    }
}