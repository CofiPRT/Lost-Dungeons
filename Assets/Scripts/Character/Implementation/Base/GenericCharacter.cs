using System;
using Character.Attributes;
using Game;
using Properties;
using UnityEngine;

namespace Character.Implementation.Base {
    public abstract class GenericCharacter : MonoBehaviour,
        IHasTeam,
        IHasAI,
        IHasMovement,
        IHasAnimator,
        IHasHealth,
        IStunnable {
        private const float DefaultAcceleration = 1.0f;
        private const float DefaultDeceleration = 0.5f;
        private static readonly Vector2 DefaultLookDirection = Vector2.up;
        private const float DefaultRotationSpeed = 5.0f;
        private const float DefaultDecayTime = 10; // after this many seconds, the character will be destroyed

        protected GenericCharacter(CharacterData data) {
            Team = data.team;

            Health = data.maxHealth;
            MaxHealth = data.maxHealth;

            Acceleration = DefaultAcceleration;
            Deceleration = DefaultDeceleration;
            LookDirection = DefaultLookDirection;
            RotationSpeed = DefaultRotationSpeed;

            // by default, the AI is enabled
            UseAI = true;
        }

        /* IHasTeam */

        public Team Team { get; }

        public void OnKill(GenericCharacter target) {
            // intentionally left blank
        }

        /* IHasAI */

        public Func<bool> AIAction { get; set; }
        public bool UseAI { get; set; }

        public void SetAI(bool useAI) {
            UseAI = useAI;
        }

        public virtual void UpdateAI() {
            if (!UseAI)
                return;

            if (AIAction?.Invoke() ?? false)
                AIAction = null;
        }

        /* IHasMovement */

        public Rigidbody RigidBody { get; set; }
        public Vector2 LookDirection { get; set; }
        public Vector2 MovementApplication { get; set; }
        public bool MovementRun { get; set; }
        public float Velocity { get; set; }
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

        /* IHasAnimator */

        public Animator Animator { get; set; }
        public float DeltaTime => Time.deltaTime * TickSpeed;

        public virtual float TickSpeed => Team switch {
            Team.Player => GameController.Instance.gameTickSpeed * GameController.Instance.playerTickFactor,
            Team.Ally => GameController.Instance.gameTickSpeed,
            Team.Enemy => GameController.Instance.gameTickSpeed,
            _ => 1
        };

        /* IHasHealth */

        public bool IsAlive => Health > 0;
        public float DeathTime { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; }

        public float TakeDamage(float damage, GenericCharacter source = null) {
            if (!IsAlive)
                return 0;

            var prevHealth = Health;
            Health = Math.Max(0, Health - damage);

            var damageDealt = prevHealth - Health;

            // nothing extra to do if the character is still alive
            if (IsAlive) return damageDealt;

            OnDeath();

            // notify the killer
            if (source)
                source.OnKill(this);

            return damageDealt;
        }

        public void Heal(float healAmount) {
            if (!IsAlive)
                return;

            Health = Math.Min(MaxHealth, Health + healAmount);
        }

        public virtual void OnDeath() {
            EndStun();
            StopMoving();
            SetAI(false);
            Animator.SetBool(AnimatorHash.Dead, true);
        }

        public void UpdateDeathTime() {
            if (IsAlive)
                return;

            DeathTime += DeltaTime;

            if (DeathTime > DefaultDecayTime)
                Destroy(this);
        }

        /* IStunnable */

        public float StunDuration { get; set; }
        public bool IsStunned => StunDuration > 0;

        public virtual bool AttemptStun(float stunDuration, GenericCharacter source) {
            if (!IsAlive || stunDuration <= 0) return false;

            StunDuration = stunDuration;
            StopMoving();
            Animator.SetBool(AnimatorHash.Stunned, true);

            return true;
        }

        public void UpdateStunDuration() {
            if (!IsAlive)
                return;

            StunDuration = Mathf.Max(0, StunDuration - DeltaTime);
            if (StunDuration == 0)
                EndStun();
        }

        public void EndStun() {
            StunDuration = 0;
            Animator.SetBool(AnimatorHash.Stunned, false);
        }

        /* Unity */

        protected virtual void Awake() {
            Animator = GetComponent<Animator>();
            RigidBody = GetComponent<Rigidbody>();
        }

        protected delegate void UpdateDelegate();

        protected virtual UpdateDelegate UpdateActions => delegate {
            UpdateDeathTime();
            UpdateStunDuration();
            UpdateMovement();
            UpdateLookDirection();
            UpdateAI();
        };

        protected virtual void Update() {
            UpdateActions();
        }
    }
}