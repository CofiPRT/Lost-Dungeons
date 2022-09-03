using Camera;
using Character.Implementation.Player;
using Character.Misc;

namespace Character.Abilities.Tristian {
    public class BerserkAbility : Ability<BerserkAbility> {
        private const float Cooldown = 30f;
        private const float ManaCost = 25f;

        public BerserkAbility(GenericPlayer user) : base(user, Cooldown, () => user.iconUltimate, false) {
            phases = new AbilityPhase<BerserkAbility>[] {
                new Phase1(this),
                new Phase2(this),
                new Phase3(this)
            };
            finalPhase = new FinalPhase(this);
        }

        public override bool Use() {
            var recast = Active;

            if (!base.Use())
                return false;

            if (recast)
                return true;

            // check for mana
            if (!User.UseMana(ManaCost)) {
                Reset();
                return false;
            }

            return true;
        }

        private class Phase1 : AbilityPhase<BerserkAbility> {
            public Phase1(BerserkAbility ability) : base(ability, 1f) { }

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
                ability.User.Animator.SetInteger(AnimatorHash.CastID, 2);
            }
        }

        private class Phase2 : AbilityPhase<BerserkAbility> {
            public Phase2(BerserkAbility ability) : base(ability, 1f) { }

            protected override void OnStart() {
                // prepare effects
                EffectsController.Prepare(
                    ability.User.SignatureColor,
                    filmGrainIntensity: 0f,
                    lensDistortionIntensity: 0f
                );
            }

            protected override void OnUpdate() {
                // lerp effects
                EffectsController.Lerp(Coefficient);
            }

            protected override void OnEnd() {
                // ensure lerps are finished
                EffectsController.Lerp(1f);

                // restore ability logic
                ability.User.UseFairFightLookDirection = true;
                ability.User.CanTakeDamage = true;
                ability.User.CastBlocksMovementLookDirectionSync = false;
                ability.User.CastBlocksAttack = false;
                ability.User.CastBlocksBlock = false;
                ability.User.CastBlocksMovement = false;

                // stop the casting animation
                ability.User.Animator.SetInteger(AnimatorHash.CastID, 0);

                ability.User.StartUltimate();
            }
        }

        private class Phase3 : AbilityPhase<BerserkAbility> {
            public Phase3(BerserkAbility ability) : base(ability, manaCostPerSecond: 5) { }

            protected override void OnManaDepletion() {
                ability.Abort();
            }

            public override void OnReactivation() {
                ability.Abort();
            }
        }

        private class FinalPhase : DefaultFinalPhase<BerserkAbility> {
            public FinalPhase(BerserkAbility ability) : base(ability, 0.5f) { }

            protected override void OnStart() {
                ability.User.StopUltimate();
            }

            protected override void OnUpdate() {
                // lerp effects out
                EffectsController.Lerp(1f - Coefficient);
            }

            protected override void OnEnd() {
                // ensure lerps are finished
                EffectsController.ResetEffects();

                // restore ability logic
                ability.User.CastBlocksAbilityUsage = false;

                base.OnEnd();
            }
        }
    }
}