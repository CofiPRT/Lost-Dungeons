using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public Vector2 LookDirection { get; set; }
        public Vector2 MovementApplication { get; set; }
        public bool IsRunning { get; set; }
        public Vector2 Velocity { get; set; }
        public float Acceleration { get; }
        public float Deceleration { get; }
        public float RotationSpeed { get; }
        public float MovementSpeedFactor => 1;
        protected virtual bool CanApplyMovement => IsAlive && !IsStunned && !AttackBlocksMovement;

        public void ApplyMovement(Vector2 direction, bool run, bool syncLookDirection) {
            if (!CanApplyMovement || direction.magnitude == 0)
                return;

            if ((run || syncLookDirection) && !IsBlocking)
                LookDirection = direction;

            // compute the signed angle difference between the movement direction and the character's orientation
            var relativeMovementDirection = RelativizeToForwardDirection(direction);

            MovementApplication = relativeMovementDirection * (run ? 2 : 1);
            IsRunning = run;
        }

        public Vector2 RelativizeToForwardDirection(Vector2 direction) {
            var angleDiff = Vector2.SignedAngle(Forward2D, direction) * Mathf.Deg2Rad;
            return new Vector2(-Mathf.Sin(angleDiff), Mathf.Cos(angleDiff));
        }

        private void ApplyDeceleration() {
            if (Velocity.magnitude == 0)
                return; // no deceleration if we're not moving

            var deceleration = Deceleration * FixedDeltaTime;
            var newMagnitude = Mathf.Clamp(Velocity.magnitude - deceleration, 0, Velocity.magnitude);
            Velocity = Velocity.normalized * newMagnitude;
        }

        private void ApplyAcceleration() {
            if (MovementApplication.magnitude == 0)
                return; // no acceleration if we're not moving

            var maxVelocity = MovementApplication.magnitude;
            var initialMagnitude = Velocity.magnitude;

            // don't overshoot the acceleration vector, otherwise a terrible jitter may happen
            var accelerationDirection = (MovementApplication - Velocity).normalized;
            var acceleration = Mathf.Min(
                Acceleration * FixedDeltaTime,
                Vector2.Distance(Velocity, MovementApplication)
            );

            var newVelocity = Velocity + accelerationDirection * acceleration;

            // if the initial magnitude is faster than max velocity, don't allow it to increase
            // if it is slower, don't allow it to go past max velocity
            Velocity = Vector2.ClampMagnitude(newVelocity, Mathf.Max(initialMagnitude, maxVelocity));

            // reset movement application - if no new movement is applied, we'll decelerate
            MovementApplication = Vector2.zero;
        }

        public void UpdateMovement() {
            ApplyDeceleration();
            ApplyAcceleration();

            // instruct the animator to match this velocity
            Animator.SetFloat(AnimatorHash.ForwardSpeed, Velocity.y);
            Animator.SetFloat(AnimatorHash.SideSpeed, Velocity.x);
            Animator.SetFloat(AnimatorHash.SpeedMagnitude, Velocity.magnitude);
        }

        public void StopMoving() {
            Velocity = Vector2.zero;
        }

        public virtual void UpdateLookDirection() {
            if (!IsAlive)
                return;

            var direction = LookDirection == Vector2.zero ? Forward2D : LookDirection;

            // face target direction
            var rotation = Quaternion.LookRotation(
                Vector3.Normalize(new Vector3(direction.x, Forward.y, direction.y))
            );

            // smooth lerp
            var speed = RotationSpeed * FixedDeltaTime;
            RigidBody.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, speed));

            IsRunning = false;
        }
    }
}