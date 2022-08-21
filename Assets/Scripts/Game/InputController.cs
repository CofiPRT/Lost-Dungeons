using UnityEngine;

namespace Scripts.Game {
    public class InputController : MonoBehaviour {
        public static bool AttackPressed() {
            return Input.GetKey(KeyCode.Mouse0);
        }

        public static bool BlockPressed() {
            return Input.GetKey(KeyCode.Mouse1);
        }

        public static bool ForwardPressed() {
            return Input.GetKey("w");
        }

        public static bool LeftPressed() {
            return Input.GetKey("a");
        }

        public static bool BackPressed() {
            return Input.GetKey("s");
        }

        public static bool RightPressed() {
            return Input.GetKey("d");
        }

        public static bool RunPressed() {
            return Input.GetKey("left shift");
        }

        public static float HorizontalLook() {
            return Input.GetAxis("Mouse X");
        }

        public static float VerticalLook() {
            return Input.GetAxis("Mouse Y");
        }

        public static bool ChangePlayerPressed() {
            return Input.GetKey("space");
        }
    }
}