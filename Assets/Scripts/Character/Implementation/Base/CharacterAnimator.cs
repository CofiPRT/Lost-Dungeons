using Character.Misc;
using Game;
using Properties;
using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        private const float FootstepYThresholdLower = 0f;
        private const float FootstepYThresholdUpper = 7e-8f;

        public Animator Animator { get; private set; }
        public float DeltaTime => Time.deltaTime * TickSpeed;
        private float FixedDeltaTime => Time.fixedDeltaTime * TickSpeed;

        private float TickSpeed => Team switch {
            Team.Player => GameController.GameTickSpeed * GameController.PlayerTickFactor,
            Team.Ally => GameController.GameTickSpeed,
            Team.Enemy => GameController.GameTickSpeed,
            _ => 1
        };

        private void UpdateTickSpeeds() {
            Animator.SetFloat(AnimatorHash.AnimationTickSpeed, TickSpeed);
            Animator.SetFloat(AnimatorHash.MovementTickSpeed, MovementSpeedFactor * TickSpeed);
            Animator.SetFloat(AnimatorHash.AttackTickSpeed, AttackSpeed * TickSpeed);
        }

        private Transform leftFoot;
        private Transform rightFoot;

        private bool leftFootLifted;
        private bool rightFootLifted;

        private void AwakeAnimator() {
            Animator = GetComponent<Animator>();

            leftFoot = Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            rightFoot = Animator.GetBoneTransform(HumanBodyBones.RightFoot);
        }

        private float maxLeftY;
        private float minLeftY;

        private float minRightY;
        private float maxRightY;

        private void UpdateFootsteps() {
            // if (leftFoot.localPosition.y < FootstepYThresholdLower && !leftFootLifted) {
            //     leftFootLifted = true;
            //     // Debug.Log("Left foot lifted");
            // } else if (leftFoot.localPosition.y > FootstepYThresholdUpper && leftFootLifted) {
            //     leftFootLifted = false;
            //     PlaySound(stepSound);
            //     // Debug.Log("Left foot put down");
            // }
            //
            // if (rightFoot.localPosition.y < FootstepYThresholdLower && !rightFootLifted) {
            //     rightFootLifted = true;
            //     // Debug.Log("Right foot lifted");
            // } else if (rightFoot.localPosition.y > FootstepYThresholdUpper && rightFootLifted) {
            //     rightFootLifted = false;
            //     PlaySound(stepSound);
            //     // Debug.Log("Right foot put down");
            // }
        }
    }
}