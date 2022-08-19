using System;
using Character.Scripts.Properties;
using UnityEngine;

namespace Character.Scripts.BaseImplementations {
    public class Character : MonoBehaviour, IHasTeam, IHasAnimator, IHasHealth, IStunnable {
        protected const float DefaultMaxHealth = 100;
        private static readonly Vector2 DefaultLookDirection = Vector2.up;
        private const float DefaultRotationSpeed = 5.0f;

        public Character(TeamMembership teamMembership, float maxHealth = DefaultMaxHealth) {
            TeamMembership = teamMembership;

            IsAlive = true;
            Health = maxHealth;
            MaxHealth = maxHealth;

            LookDirection = DefaultLookDirection;
            RotationSpeed = DefaultRotationSpeed;
        }

        /* IHasTeam */

        public TeamMembership TeamMembership { get; }

        public void OnKill(Character target) {
            // intentionally left blank
        }

        /* IHasAnimator */

        public Rigidbody RigidBody { get; set; }
        public Animator Animator { get; set; }
        public Vector2 LookDirection { get; set; }
        public float RotationSpeed { get; }

        public void ApplyMovement(Vector2 direction, bool run) {
            if (IsStunned || direction.magnitude == 0)
                StopMoving();

            // assure the direction is normalized
            direction.Normalize();

            LookDirection = direction;

            // update the animator
            Animator.SetBool(AnimatorHash.IsWalking, true);
            Animator.SetBool(AnimatorHash.IsRunning, run);
        }

        protected void StopMoving() {
            Animator.SetBool(AnimatorHash.IsWalking, false);
            Animator.SetBool(AnimatorHash.IsRunning, false);
        }

        /* IHasHealth */

        public bool IsAlive { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; }

        public float TakeDamage(float damage, Character source = null) {
            var prevHealth = Health;
            Health = Math.Max(0, Health - damage);

            var damageDealt = prevHealth - Health;

            // nothing extra to do if the character is still alive
            if (Health != 0) return damageDealt;

            IsAlive = false;
            OnDeath();

            // notify the killer
            if (source)
                source.OnKill(this);

            return damageDealt;
        }

        public void Heal(float healAmount) {
            Health = Math.Min(MaxHealth, Health + healAmount);
        }

        public void OnDeath() {
            // intentionally left blank
        }

        /* IStunnable */

        public float StunDuration { get; set; }
        public bool IsStunned => StunDuration > 0;

        public virtual bool AttemptStun(float stunDuration, Character source) {
            StunDuration = stunDuration;

            return true;
        }

        /* Unity */

        protected virtual void Awake() {
            Animator = GetComponent<Animator>();
            RigidBody = GetComponent<Rigidbody>();
        }

        protected virtual void Update() {
            StunDuration = Mathf.Max(0, StunDuration - Time.deltaTime);

            // face target direction
            var rotation = Quaternion.LookRotation(
                Vector3.Normalize(
                    new Vector3(
                        LookDirection.x,
                        0,
                        LookDirection.y
                    )
                )
            );

            // smooth lerp
            var speed = RotationSpeed * Time.deltaTime;
            RigidBody.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, speed));
        }
    }
}