using UnityEngine;

namespace Character.Scripts {
    public class PlayerAnimator : MonoBehaviour {
        private Animator animator;

        // save hashes for faster parameter acquisition
        private static readonly int IsBlockingHash = Animator.StringToHash("isBlocking");
        private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
        private static readonly int IsRunningHash = Animator.StringToHash("isRunning");
        private static readonly int AttackHash = Animator.StringToHash("attack");

        private static readonly int AnimationSpeedHash = Animator.StringToHash("animationSpeed");
        private static readonly int MovementSpeedHash = Animator.StringToHash("movementSpeed");
        private static readonly int AttackSpeedHash = Animator.StringToHash("attackSpeed");

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        public bool IsAttacking() {
            return animator.GetInteger(AttackHash) != 0;
        }

        public void StartAttacking() {
            animator.SetInteger(AttackHash, Random.Range(1, 3));
        }

        public bool IsBlocking() {
            return animator.GetBool(IsBlockingHash);
        }

        public void SetBlocking(bool value) {
            animator.SetBool(IsBlockingHash, value);
        }

        public bool IsWalking() {
            return animator.GetBool(IsWalkingHash);
        }

        public void SetWalking(bool value) {
            animator.SetBool(IsWalkingHash, value);
        }

        public bool IsRunning() {
            return animator.GetBool(IsRunningHash);
        }

        public void SetRunning(bool value) {
            animator.SetBool(IsRunningHash, value);
        }

        public void SetAnimationSpeed(float value) {
            animator.SetFloat(AnimationSpeedHash, value);
        }

        public void SetMovementSpeed(float value) {
            animator.SetFloat(MovementSpeedHash, value);
        }

        public void SetAttackSpeed(float value) {
            animator.SetFloat(AttackSpeedHash, value);
        }
    }
}