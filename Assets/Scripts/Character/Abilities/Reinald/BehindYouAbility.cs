using Character.Implementation.Enemy;
using Character.Implementation.Player;
using Character.Misc;

namespace Character.Abilities.Reinald {
    public class BehindYouAbility : Ability<BehindYouAbility> {
        private const float Cooldown = 20f;
        private const float ManaCost = 10f;

        private GenericEnemy target;

        public BehindYouAbility(GenericPlayer user) : base(user, Cooldown, () => user.iconAbility2) {
            phases = new AbilityPhase<BehindYouAbility>[] {
                new Phase1(this),
                new Phase2(this),
                new Phase3(this),
                new Phase4(this)
            };
            finalPhase = new DefaultFinalPhase<BehindYouAbility>(this);
        }

        public override bool Use() {
            if (!base.Use())
                return false;

            // look for an enemy in the direction of the camera
            var foundEnemy = User.FindObjectInCone<GenericEnemy>(
                "Enemy",
                x => x.Pos,
                x => x.IsAlive
            );
            if (foundEnemy == null) {
                Reset();
                return false;
            }

            // check for mana
            if (!User.UseMana(ManaCost)) {
                Reset();
                return false;
            }

            target = foundEnemy;

            return true;
        }

        private class Phase1 : AbilityPhase<BehindYouAbility> {
            public Phase1(BehindYouAbility ability) : base(ability, 0.5f) { }

            protected override void OnStart() {
                // ability logic
                ability.User.UseFairFightLookDirection = false;
                ability.User.CanTakeDamage = false;
                ability.User.CastBlocksAbilityUsage = true;
                ability.User.CastBlocksMovementLookDirectionSync = true;
                ability.User.CastBlocksAttack = true;
                ability.User.CastBlocksBlock = true;
                ability.User.CastBlocksMovement = true;

                // start the casting animation
                ability.User.Animator.SetInteger(AnimatorHash.CastID, 1);

                // apply stun on target
                ability.target.AttemptStun(5f, ability.User);

                // play sound
                ability.User.PlaySound(ability.User.castSound);
            }
        }

        private class Phase2 : AbilityPhase<BehindYouAbility> {
            public Phase2(BehindYouAbility ability) : base(ability, 0.5f) { }

            protected override void OnStart() {
                // prepare the character to teleport
                ability.User.StartTransparency();
            }

            protected override void OnUpdate() {
                // lerp character transparency
                ability.User.LerpTransparency(1 - Coefficient);
            }

            protected override void OnEnd() {
                // ensure the lerp is complete
                ability.User.LerpTransparency(0);

                // teleport behind the target
                var direction = (ability.target.Pos - ability.User.Pos).normalized;
                var destination = ability.target.Pos + direction * ability.User.AttackRange * 0.5f;

                ability.User.transform.position = destination;
            }
        }

        private class Phase3 : AbilityPhase<BehindYouAbility> {
            public Phase3(BehindYouAbility ability) : base(ability, 0.25f) { }
        }

        private class Phase4 : AbilityPhase<BehindYouAbility> {
            public Phase4(BehindYouAbility ability) : base(ability, 0.5f) { }

            protected override void OnStart() {
                // stop channeling
                ability.User.Animator.SetInteger(AnimatorHash.CastID, 0);

                // start attack animation
                ability.User.CastBlocksAttack = false;
                ability.User.StartAttack(ability.target.Pos2D - ability.User.Pos2D);
            }

            protected override void OnUpdate() {
                // lerp the user's transparency back
                ability.User.LerpTransparency(Coefficient);
            }

            protected override void OnEnd() {
                // ensure the lerp is complete
                ability.User.LerpTransparency(0);
                ability.User.StopTransparency();

                // restore ability logic
                ability.User.UseFairFightLookDirection = true;
                ability.User.CanTakeDamage = true;
                ability.User.CastBlocksAbilityUsage = false;
                ability.User.CastBlocksMovementLookDirectionSync = false;
                ability.User.CastBlocksBlock = false;
                ability.User.CastBlocksMovement = false;
            }
        }
    }
}