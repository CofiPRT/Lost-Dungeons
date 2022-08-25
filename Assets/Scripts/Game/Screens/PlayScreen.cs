using Camera;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Game.Screens {
    public class PlayScreen : GameScreen {
        public override void LateUpdate() {
            // update camera rotation
            var horizontalMovement = Input.GetAxis("Mouse X");
            var verticalMovement = Input.GetAxis("Mouse Y");
            CameraController.Instance.ApplyInput(horizontalMovement, verticalMovement);
        }

        public override void Update() {
            // handle player movement
            var direction = Vector2.zero;

            if (Input.GetKey(KeyCode.W)) direction.y += 1;

            if (Input.GetKey(KeyCode.S)) direction.y -= 1;

            if (Input.GetKey(KeyCode.D)) direction.x += 1;

            if (Input.GetKey(KeyCode.A)) direction.x -= 1;

            var running = Input.GetKey(KeyCode.LeftShift);

            if (direction.magnitude > 0)
                GameController.ControllerPlayer.ApplyMovement(direction.normalized, running, false);
            
            // handle player attack
            if (Input.GetKey(KeyCode.Mouse0)) {
                var attackDirection = direction.magnitude > 0 ? direction : CameraController.Forward2D;
                GameController.ControllerPlayer.StartAttack(attackDirection);
            }
            
            // handle player block
            if (Input.GetKey(KeyCode.Mouse1)) {
                var blockDirection = direction.magnitude > 0 ? direction : CameraController.Forward2D;
                GameController.ControllerPlayer.StartBlocking(blockDirection);
            }
        }
    }
}