using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter : MonoBehaviour {
        private const float DefaultAcceleration = 3.0f;
        private const float DefaultDeceleration = 1.0f;
        private const float DefaultRotationSpeed = 5.0f;
        private const float DefaultDecayTime = 10; // after this many seconds, the character will be destroyed

        protected GenericCharacter(CharacterBuilder data) {
            Name = data.name;
            Team = data.team;

            Health = data.maxHealth;
            MaxHealth = data.maxHealth;

            Acceleration = DefaultAcceleration;
            Deceleration = DefaultDeceleration;
            RotationSpeed = DefaultRotationSpeed;

            AttackDamage = data.attackDamage;
            AttackSpeed = data.attackSpeed;
            AttackRange = data.attackRange;
            AttackAngle = data.attackAngle;
            AttackStrength = data.attackStrength;

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

        public Vector3 Right => transform.right;
        public Vector2 Right2D => new Vector2(Right.x, Right.z).normalized;

        protected virtual void Awake() {
            AwakeAnimator();
            AwakeCollider();
            AwakeHealthBar();
            AwakeTransparency();
        }

        protected delegate void UpdateDelegate();

        protected virtual UpdateDelegate UpdateActions => delegate {
            UpdateTickSpeeds();
            UpdateDeathTime();
            UpdateStunDuration();
            UpdateAI();
            UpdateBlock();
            UpdateHealthBar();
            UpdateAutoHeal();
            UpdateAutoManaGain();
        };

        protected UpdateDelegate FixedUpdateActions => delegate {
            UpdateMovement();
            UpdateLookDirection();
        };

        protected virtual void Update() {
            UpdateActions();
        }

        protected void FixedUpdate() {
            FixedUpdateActions();
        }
    }
}