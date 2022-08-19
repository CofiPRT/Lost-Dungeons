using UnityEngine;

namespace Character.Scripts.Properties {
    public interface IHasAnimator {
        Rigidbody RigidBody { get; set; }
        Animator Animator { get; set; }
        Vector2 LookDirection { get; set; }
        float RotationSpeed { get; }
        void ApplyMovement(Vector2 direction, bool run);
    }

    public static class AnimatorHash {
        // save hashes for faster parameter acquisition
        public static readonly int IsBlocking = Animator.StringToHash("isBlocking");
        public static readonly int IsWalking = Animator.StringToHash("isWalking");
        public static readonly int IsRunning = Animator.StringToHash("isRunning");
        public static readonly int Attack = Animator.StringToHash("attack");

        public static readonly int AnimationSpeed = Animator.StringToHash("animationSpeed");
        public static readonly int MovementSpeed = Animator.StringToHash("movementSpeed");
        public static readonly int AttackSpeed = Animator.StringToHash("attackSpeed");
    }
}