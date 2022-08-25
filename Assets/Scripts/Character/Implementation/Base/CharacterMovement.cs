using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public Rigidbody RigidBody { get; set; }
        public virtual Vector2 LookDirection { get; set; }
        public Vector2 MovementApplication { get; set; }
        public bool MovementRun { get; set; }
        public Vector2 Velocity { get; set; }
        public float Acceleration { get; }
        public float Deceleration { get; }
        public float RotationSpeed { get; }
        public float MovementSpeedFactor => 1;
        public virtual bool CanApplyMovement => IsAlive && !IsStunned;

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

        private void ApplyDeceleration() {
            if (Velocity.magnitude == 0)
                return; // no deceleration if we're not moving

            var deceleration = Deceleration * DeltaTime;
            var newMagnitude = Mathf.Clamp(Velocity.magnitude - deceleration, 0, Velocity.magnitude);
            Velocity = Velocity.normalized * newMagnitude;
        }

        private void ApplyAcceleration() {
            var maxVelocity = MovementRun ? 2.0f : 1.0f;
            var initialMagnitude = Velocity.magnitude;

            var acceleration = Acceleration * DeltaTime;
            var accelerationDirection = (MovementApplication - Velocity).normalized;
            var newVelocity = Velocity + accelerationDirection * acceleration;

            // if the initial magnitude is faster than max velocity, don't allow it to increase
            // if it is slower, don't allow it to go past max velocity
            var maxMagnitude = initialMagnitude > maxVelocity ? initialMagnitude : maxVelocity;
            newVelocity = Vector2.ClampMagnitude(newVelocity, maxMagnitude);

            Velocity = newVelocity;

            // reset movement application - if no new movement is applied, we'll decelerate
            MovementApplication = Vector2.zero;
        }

        public void UpdateMovement() {
            Animator.SetFloat(AnimatorHash.MovementTickSpeed, MovementSpeedFactor * TickSpeed);

            // no movement, just decelerate
            if (MovementApplication == Vector2.zero)
                ApplyDeceleration();
            else
                ApplyAcceleration();

            // instruct the animator to match this velocity
            SetAnimatorMovementSpeed(Velocity.x, Velocity.y);
        }

        public void StopMoving() {
            Velocity = Vector2.zero;
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