using UnityEngine;

namespace Character.Scripts.Attributes {
    public interface IHasAnimator {
        Animator Animator { get; set; }
        float DeltaTime { get; }
        float TickSpeed { get; }
    }

    public static class AnimatorHash {
        // save hashes for faster parameter acquisition
        public static readonly int Blocking = Animator.StringToHash("blocking");
        public static readonly int Walking = Animator.StringToHash("walking");
        public static readonly int Running = Animator.StringToHash("running");
        public static readonly int Attacking = Animator.StringToHash("attacking");
        public static readonly int Stunned = Animator.StringToHash("stunned");
        public static readonly int Dead = Animator.StringToHash("dead");

        public static readonly int AnimationSpeed = Animator.StringToHash("animationSpeed");
        public static readonly int MovementSpeed = Animator.StringToHash("movementSpeed");
        public static readonly int AttackSpeed = Animator.StringToHash("attackSpeed");
    }
}