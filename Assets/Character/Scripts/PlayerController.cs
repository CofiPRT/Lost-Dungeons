using Game;
using UnityEngine;

namespace Character.Scripts {
    public class PlayerController : MonoBehaviour {
        public float rotateSpeed = 5.0f;
        public float movementSpeedFactor = 1.0f;
        public float attackSpeedFactor = 1.0f;

        public Transform ally;
        public bool automated;
        public bool updateInputs = true;

        private UnityEngine.Camera cam;
        private PlayerAnimator animator;
        private Vector3 targetDirection;
        private Rigidbody rb;

        // actions
        public bool attackPressed;
        public bool blockPressed;
        public bool runPressed;
        public Vector3 movementInput;

        /*void Awake()
    {
        StartCoroutine(PostSimulationUpdate());
    }*/

        private void Start() {
            cam = UnityEngine.Camera.main;
            animator = GetComponent<PlayerAnimator>();
            rb = GetComponent<Rigidbody>();
        }

        private void Update() {
            if (automated || !updateInputs)
                return;

            attackPressed = InputController.AttackPressed();
            blockPressed = InputController.BlockPressed();
            runPressed = InputController.RunPressed();
            movementInput = GetMovementInput();
        }

        private void FixedUpdate() {
            var cascadeReset = UpdateAttack();
            cascadeReset = UpdateBlock(cascadeReset);
            UpdateMovement(cascadeReset);

            FaceTargetDirection();
            UpdateAnimationSpeed();
        }

        private bool UpdateAttack() {
            var isAttacking = animator.IsAttacking();

            // finish an existing attack before attempting to start another one
            if (isAttacking)
                return true;

            // we want the animation to start when the attack key is pressed
            if (!attackPressed)
                return false;

            animator.StartAttacking();
            SetTargetDirection();

            return true;
        }

        private bool UpdateBlock(bool reset = false) {
            if (reset) {
                animator.SetBlocking(false);

                return true;
            }

            var isBlocking = animator.IsBlocking();

            // we want the animation to run while the block key is pressed
            if (isBlocking != blockPressed) {
                animator.SetBlocking(!isBlocking);

                // if we just started blocking, rotate the player in the direction of the camera
                if (blockPressed)
                    SetTargetDirection();
            }

            return blockPressed;
        }

        private void UpdateMovement(bool reset = false) {
            if (reset) {
                animator.SetWalking(false);
                animator.SetRunning(false);

                return;
            }

            var isWalking = animator.IsWalking();
            var isRunning = animator.IsRunning();

            // if the resultant is zero (e.g.: pressing all movement keys at once, or opposite keys like W and S), we should not move
            var movementPressed = movementInput != Vector3.zero;

            // we want the animation to run while a movement key is pressed
            if (isWalking != movementPressed)
                animator.SetWalking(!isWalking);

            if (isRunning != (movementPressed && runPressed))
                animator.SetRunning(!isRunning);

            // rotate the player relative to the direction of movement
            SetTargetDirection();

            // set the speed of this animation relative to the player's movement speed
            animator.SetMovementSpeed(movementSpeedFactor);
        }

        private void UpdateAnimationSpeed() {
            var tickSpeed = GameController.Instance.GetPlayerTickSpeed();
            animator.SetAnimationSpeed(tickSpeed);
            animator.SetMovementSpeed(tickSpeed * movementSpeedFactor);
            animator.SetAttackSpeed(tickSpeed * attackSpeedFactor);
        }

        private Vector3 GetMovementInput() {
            // the movement is relative to the camera's orientation, disregarding any pitch (thus the cross products)
            var camT = cam.transform;
            var movement = Vector3.zero;

            if (InputController.ForwardPressed())
                movement += Vector3.Cross(camT.right, Vector3.up);

            if (InputController.LeftPressed())
                movement += Vector3.Cross(camT.forward, Vector3.up);

            if (InputController.BackPressed())
                movement -= Vector3.Cross(camT.right, Vector3.up);

            if (InputController.RightPressed())
                movement -= Vector3.Cross(camT.forward, Vector3.up);

            return Vector3.Normalize(movement);
        }

        private void FaceTargetDirection() {
            // make the direction parallel to the ground - remove the y component
            var rotation =
                Quaternion.LookRotation(Vector3.Normalize(new Vector3(targetDirection.x, 0, targetDirection.z)));

            // smooth lerp
            var speed = rotateSpeed * Time.deltaTime * GameController.Instance.GetPlayerTickSpeed();
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, speed));

            /* // make the direction parallel to the ground - remove the y component
        Vector3 direction = new Vector3(targetDirection.x, 0, targetDirection.z);

        float eulerAngle = Vector3.SignedAngle(Vector3.forward, targetDirection, Vector3.up);
        float desiredYRotation = transform.eulerAngles.y + Mathf.DeltaAngle(transform.eulerAngles.y, eulerAngle);

        float targetAngle = Mathf.SmoothDampAngle(
            rb.rotation.eulerAngles.y,
            desiredYRotation,
            ref rotateSpeed,
            0.1f,
            float.MaxValue,
            100 * Time.fixedDeltaTime
        );

        Quaternion targetRotation = Quaternion.Euler(0.0f, targetAngle, 0.0f);
        rb.MoveRotation(targetRotation);*/
        }

        private void SetTargetDirection() {
            targetDirection = movementInput == Vector3.zero ? transform.forward : movementInput;
        }
    }
}