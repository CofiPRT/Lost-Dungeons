using System;
using Scripts.Character.Attributes;
using Scripts.Game;
using Scripts.Properties;
using UnityEngine;

namespace Scripts.Character.Implementation.Base {
    public abstract class GenericCharacter : MonoBehaviour,
        IHasTeam,
        IHasAI,
        IHasMovement,
        IHasAnimator,
        IHasHealth,
        IStunnable {
        private static readonly Vector2 DefaultLookDirection = Vector2.up;
        private const float DefaultRotationSpeed = 5.0f;
        private const float DefaultDecayTime = 10; // after this many seconds, the character will be destroyed

        protected GenericCharacter(CharacterData data) {
            Team = data.team;

            Health = data.maxHealth;
            MaxHealth = data.maxHealth;

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

        public bool UseAI { get; set; }

        public void SetAI(bool useAI) {
            UseAI = useAI;
        }

        public virtual void RunAI() {
            // intentionally left blank
        }

        /* IHasMovement */

        public Rigidbody RigidBody { get; set; }
        public Vector2 LookDirection { get; set; }
        public float RotationSpeed { get; }
        public float MovementSpeedFactor => 1;
        public virtual bool CanApplyMovement => IsAlive && !IsStunned;

        public virtual void ApplyMovement(Vector2 direction, bool run) {
            if (!CanApplyMovement || direction.magnitude == 0)
                StopMoving();

            // assure the direction is normalized
            LookDirection = direction;

            // update the animator
            Animator.SetBool(AnimatorHash.Walking, true);
            Animator.SetBool(AnimatorHash.Running, run);
            Animator.SetFloat(AnimatorHash.MovementSpeed, MovementSpeedFactor * TickSpeed);
        }

        public void StopMoving() {
            Animator.SetBool(AnimatorHash.Walking, false);
            Animator.SetBool(AnimatorHash.Running, false);
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
            UpdateLookDirection();
        };

        protected virtual void Update() {
            UpdateActions();
        }
    }
}