using UnityEngine;

namespace Character.Scripts {
    public class PlayerAnimator : MonoBehaviour {
        private Animator animator;


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