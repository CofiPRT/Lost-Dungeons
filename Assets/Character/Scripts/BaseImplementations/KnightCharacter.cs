using Character.Scripts.Properties;
using UnityEngine;

namespace Character.Scripts.BaseImplementations {
    public class KnightCharacter : SwordCharacter, IHasShield {
        private const float DefaultShieldAngle = Mathf.PI / 4;
        private const float DefaultShieldRechargeTime = 5; // seconds
        private const BlockStrength DefaultBlockStrength = BlockStrength.Weak;

        private const float FailedBlockStunDuration = 2; // seconds

        public KnightCharacter(
            TeamMembership teamMembership,
            float maxHealth = DefaultMaxHealth,
            float attackDamage = DefaultAttackDamage,
            float attackSpeed = DefaultAttackSpeed,
            float attackRange = DefaultAttackRange,
            float attackAngle = DefaultAttackAngle,
            float shieldAngle = DefaultShieldAngle,
            float shieldRechargeTime = DefaultShieldRechargeTime,
            BlockStrength blockStrength = DefaultBlockStrength
        ) : base(teamMembership, maxHealth, attackDamage, attackSpeed, attackRange, attackAngle) {
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

        public void StartBlocking() {
            if (IsBlocking || IsStunned || ShieldCooldown > 0)
                return;

            IsBlocking = true;
        }

        public void StopBlocking(bool force = false) {
            if (!IsBlocking)
                return;

            IsBlocking = false;
            if (force)
                ShieldCooldown = ShieldRechargeTime;
        }

        /* Parent */

        public override bool AttemptStun(float stunDuration, Character source) {
            if (!base.AttemptStun(stunDuration, source))
                return false;

            StopBlocking(true);
            return true;
        }

        /* Unity */

        protected override void Update() {
            base.Update();

            ShieldCooldown = Mathf.Max(0, ShieldCooldown - Time.deltaTime);
        }
    }
}