using UnityEngine;

namespace Character.Attributes {
    public interface IHasAnimator {
        Animator Animator { get; set; }
        float DeltaTime { get; }
        float TickSpeed { get; }
    }

    public static class AnimatorHash {
        // save hashes for faster parameter acquisition
        public static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");
        public static readonly int SideSpeed = Animator.StringToHash("sideSpeed");

        public static readonly int Blocking = Animator.StringToHash("blocking");
        public static readonly int Attacking = Animator.StringToHash("attacking");

        public static readonly int Stunned = Animator.StringToHash("stunned");
        public static readonly int Dead = Animator.StringToHash("dead");

        public static readonly int AnimationTickSpeed = Animator.StringToHash("animationTickSpeed");
        public static readonly int MovementTickSpeed = Animator.StringToHash("movementTickSpeed");
        public static readonly int AttackTickSpeed = Animator.StringToHash("attackTickSpeed");
    }
}