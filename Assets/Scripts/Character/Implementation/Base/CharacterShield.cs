using Character.Misc;
using Properties;
using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        private const float FailedBlockStunDuration = 2; // seconds

        public bool IsBlocking { get; private set; }
        private float ShieldAngle { get; }
        public float ShieldRechargeTime { get; }
        public float ShieldCooldown { get; private set; }
        private BlockStrength BlockStrength { get; }

        private float AttemptBlock(float damage, AttackStrength attackStrength, GenericCharacter source) {
            if (!IsBlocking || IsStunned)
                return TakeDamage(damage, source);

            // if the attack is not within shield angle, block is not successful
            var attackDirection = source.Pos2D - Pos2D;
            var blockDirection = Forward2D;

            var angle = Vector2.Angle(attackDirection, blockDirection) * Mathf.Deg2Rad;
            if (angle > ShieldAngle)
                return TakeDamage(damage, source);

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
                    PlaySound(blockSound);
                    return 0;
                case -2:
                    // damage blocked completely, the attacker is stunned
                    source.AttemptStun(FailedBlockStunDuration, this);
                    PlaySound(blockSound);

                    return 0;
            }

            // should not reach
            Debug.LogError($"Invalid score {score}");
            return 0;
        }

        protected virtual bool CanStartBlocking => IsAlive && !IsStunned && !IsAttacking && ShieldCooldown == 0;

        public void StartBlocking(Vector2 direction) {
            if (!CanStartBlocking || direction.magnitude == 0) return;

            // start blocking towards this direction - changing the look direction by other means should be prevented
            LookDirection = direction;
            IsBlocking = true;
            Animator.SetBool(AnimatorHash.Blocking, true);
        }

        public void StopBlocking(bool force = false) {
            if (!IsBlocking)
                return;

            IsBlocking = false;
            if (force) {
                ShieldCooldown = ShieldRechargeTime;
                PlaySound(blockBreakSound);
            }

            Animator.SetBool(AnimatorHash.Blocking, false);
        }

        private void UpdateBlock() {
            // update cooldown
            ShieldCooldown = Mathf.Max(0, ShieldCooldown - DeltaTime);
        }
    }
}