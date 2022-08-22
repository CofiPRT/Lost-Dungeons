using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public Rigidbody RigidBody { get; set; }
        public Vector2 LookDirection { get; set; }
        public Vector2 MovementApplication { get; set; }
        public bool MovementRun { get; set; }
        public float Velocity { get; set; }
        public float Acceleration { get; }
        public float Deceleration { get; }
        public float RotationSpeed { get; }
        public float MovementSpeedFactor => 1;
        public bool CanApplyMovement => IsAlive && !IsStunned && !CastBlocksMovement;

        public void ApplyMovement(Vector2 direction, bool run, bool syncLookDirection) {
            if (!CanApplyMovement || direction.magnitude == 0)
                StopMoving();

            if (syncLookDirection)
                LookDirection = direction;

            // compute the signed angle difference between the movement direction and the look direction
            var angleDiff = Vector2.SignedAngle(LookDirection, direction);

            // the animator will be instructed to move according to this relative direction
            var relativeMovementDirection = new Vector2(Mathf.Cos(angleDiff), -Mathf.Sin(angleDiff));

            MovementApplication = relativeMovementDirection;
            MovementRun = run;
        }

        public void UpdateMovement() {
            Animator.SetFloat(AnimatorHash.MovementTickSpeed, MovementSpeedFactor * TickSpeed);

            if (MovementApplication == Vector2.zero) {
                // apply deceleration
                Velocity = Mathf.Clamp(Velocity - Deceleration * DeltaTime, 0, Velocity);
                return;
            }

            // else, instruct the animator to match the movement, and reset the movement application
            var maxVelocity = MovementRun ? 2.0f : 1.0f;

            Velocity = Velocity > maxVelocity
                ? Mathf.Clamp(Velocity - Deceleration * DeltaTime, 0, Velocity)
                : Mathf.Clamp(Velocity + Acceleration * DeltaTime, 0, maxVelocity);

            // the velocity defines the magnitude of the movement
            var movement = MovementApplication * Velocity;
            SetAnimatorMovementSpeed(movement.x, movement.y);

            MovementApplication = Vector2.zero;
        }

        public void StopMoving() {
            Velocity = 0;
            SetAnimatorMovementSpeed(0, 0);
        }

        private void SetAnimatorMovementSpeed(float forward, float side) {
            Animator.SetFloat(AnimatorHash.ForwardSpeed, forward);
            Animator.SetFloat(AnimatorHash.SideSpeed, side);
        }

        public void UpdateLookDirection() {
            if (!IsAlive)
                return;

            // face target direction
            var rotation = Quaternion.LookRotation(
                Vector3.Normalize(new Vector3(LookDirection.x, 0, LookDirection.y))
            );

            // smooth lerp
            var speed = RotationSpeed * DeltaTime;
            RigidBody.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, speed));

            // update the animator
            Animator.SetFloat(AnimatorHash.AnimationTickSpeed, TickSpeed);
        }
    }
}