﻿using UnityEngine;

namespace Character {
    public static class AnimatorHash {
        // save hashes for faster parameter acquisition
        public static readonly int AnimationTickSpeed = Animator.StringToHash("animationTickSpeed");
        public static readonly int MovementTickSpeed = Animator.StringToHash("movementTickSpeed");
        public static readonly int AttackTickSpeed = Animator.StringToHash("attackTickSpeed");

        public static readonly int SpeedMagnitude = Animator.StringToHash("speedMagnitude");
        public static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");
        public static readonly int SideSpeed = Animator.StringToHash("sideSpeed");

        public static readonly int AttackID = Animator.StringToHash("attackID");
        public static readonly int CastID = Animator.StringToHash("castID");

        public static readonly int Blocking = Animator.StringToHash("blocking");
        public static readonly int Stunned = Animator.StringToHash("stunned");
        public static readonly int Dead = Animator.StringToHash("dead");
        public static readonly int Hurt = Animator.StringToHash("hurt");

        public static readonly int DodgingForward = Animator.StringToHash("dodgingForward");
        public static readonly int DodgingBackward = Animator.StringToHash("dodgingBackward");
        public static readonly int DodgingLeft = Animator.StringToHash("dodgingLeft");
        public static readonly int DodgingRight = Animator.StringToHash("dodgingRight");
    }
}