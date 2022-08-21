using Character.Scripts.Attributes;
using Character.Scripts.Properties;
using UnityEngine;

namespace Character.Scripts.Generic {
    public abstract class KnightCharacter : SwordCharacter, IHasShield {
        protected const float DefaultShieldAngle = Mathf.PI / 4;
        protected const float DefaultShieldRechargeTime = 5; // seconds
        protected const BlockStrength DefaultBlockStrength = BlockStrength.Weak;

        private const float FailedBlockStunDuration = 2; // seconds

        protected KnightCharacter(
            Team team,
            float maxHealth = DefaultMaxHealth,
            float attackDamage = DefaultAttackDamage,
            float attackSpeed = DefaultAttackSpeed,
            float attackRange = DefaultAttackRange,
            float attackAngle = DefaultAttackAngle,
            float shieldAngle = DefaultShieldAngle,
            float shieldRechargeTime = DefaultShieldRechargeTime,
            BlockStrength blockStrength = DefaultBlockStrength
        ) : base(team, maxHealth, attackDamage, attackSpeed, attackRange, attackAngle) {
            ShieldAngle = shieldAngle;
            ShieldRechargeTime = shieldRechargeTime;
            BlockStrength = blockStrength;
        }

        /* IHasShield */

        public bool IsBlocking { get; set; }
        public float ShieldAngle { get; }
        public float ShieldRechargeTime { get; }
        public float ShieldCooldown { get; set; }
        public BlockStrength BlockStrength { get; }

        public float AttemptBlock(float damage, AttackStrength attackStrength, Character source) {
            if (!IsBlocking || IsStunned)
                return damage;

            // compute how strong the attack is compared to the blocking power
            var attackScore = AttackScore.Of(attackStrength);
            var blockScore = BlockScore.Of(BlockStrength);

            var score = attackScore - blockScore;

            switch (score) {
                case 2:
                    // block only half the damage, the blocker is stunned, shield is put on cooldown
                    AttemptStun(FailedBlockStunDuration, source);
                    StopBlocking(true);

                    return TakeDamage(damage / 2, source);
                case 1:
                    // damage blocked completely, the blocker is stunned, shield is put on cooldown
                    AttemptStun(FailedBlockStunDuration, source);
                    StopBlocking(true);

                    return 0;
                case 0:
                    // damage blocked completely, the shield is put on cooldown
                    StopBlocking(true);

                    return 0;
                case -1:
                    // damage blocked completely, the shield is still up
                    return 0;
                case -2:
                    // damage blocked completely, the attacker is stunned
                    source.AttemptStun(FailedBlockStunDuration, this);

                    return 0;
            }

            // should not reach
            Debug.LogError($"Invalid score {score}");
            return 0;
        }

        public bool CanStartBlocking => IsAlive && !IsBlocking && !IsStunned && !IsAttacking && ShieldCooldown == 0;

        public void StartBlocking(Vector2 direction) {
            if (!CanStartBlocking || direction.magnitude == 0) return;

            IsBlocking = true;
            LookDirection = direction;
            StopMoving();

            Animator.SetBool(AnimatorHash.Blocking, true);
        }

        public void StopBlocking(bool force = false) {
            if (!IsBlocking)
                return;

            IsBlocking = false;
            if (force)
                ShieldCooldown = ShieldRechargeTime;

            Animator.SetBool(AnimatorHash.Blocking, false);
        }

        /* Parent */

        public override void OnDeath() {
            StopBlocking();
            base.OnDeath();
        }

        public override bool CanApplyMovement => base.CanApplyMovement && !IsBlocking;

        public override bool AttemptStun(float stunDuration, Character source) {
            if (!base.AttemptStun(stunDuration, source))
                return false;

            StopBlocking(true);
            return true;
        }

        public override void StartAttack(Vector2 direction) {
            base.StartAttack(direction);

            // attacking can override blocking
            if (IsAttacking)
                StopBlocking();
        }

        /* Unity */

        protected override void Update() {
            base.Update();

            ShieldCooldown = Mathf.Max(0, ShieldCooldown - DeltaTime);
        }
    }
}