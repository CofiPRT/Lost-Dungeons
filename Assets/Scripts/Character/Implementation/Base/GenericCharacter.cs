using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter : MonoBehaviour {
        private const float DefaultAcceleration = 1.0f;
        private const float DefaultDeceleration = 0.5f;
        private static readonly Vector2 DefaultLookDirection = Vector2.up;
        private const float DefaultRotationSpeed = 5.0f;
        private const float DefaultDecayTime = 10; // after this many seconds, the character will be destroyed

        protected GenericCharacter(CharacterBuilder data) {
            Team = data.team;

            Health = data.maxHealth;
            MaxHealth = data.maxHealth;

            Acceleration = DefaultAcceleration;
            Deceleration = DefaultDeceleration;
            LookDirection = DefaultLookDirection;
            RotationSpeed = DefaultRotationSpeed;

            AttackDamage = data.attackDamage;
            AttackSpeed = data.attackSpeed;
            AttackRange = data.attackRange;
            AttackAngle = data.attackAngle;

            ShieldAngle = data.shieldAngle;
            ShieldRechargeTime = data.shieldRechargeTime;
            BlockStrength = data.blockStrength;

            Mana = data.maxMana;
            MaxMana = data.maxMana;

            // by default, the AI is enabled
            SetAI(true);
        }

        public Vector3 Pos => transform.position;
        public Vector2 Pos2D => new Vector2(Pos.x, Pos.z);

        public Vector3 Forward => transform.forward;
        public Vector2 Forward2D => new Vector2(Forward.x, Forward.z).normalized;

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
            UpdateAttackCooldown();
            UpdateShieldCooldown();
        };

        protected virtual void Update() {
            UpdateActions();
        }
    }
}