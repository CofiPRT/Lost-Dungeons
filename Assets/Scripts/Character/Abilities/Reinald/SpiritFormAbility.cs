using CameraScript;
using Character.Implementation.Player;
using Character.Misc;

namespace Character.Abilities.Reinald {
    public class SpiritFormAbility : Ability<SpiritFormAbility> {
        private const float Cooldown = 30f;
        private const float ManaCost = 25f;

        public SpiritFormAbility(GenericPlayer user) : base(user, Cooldown, () => user.iconUltimate, false) {
            phases = new AbilityPhase<SpiritFormAbility>[] {
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

        private class Phase1 : AbilityPhase<SpiritFormAbility> {
            public Phase1(SpiritFormAbility ability) : base(ability, 1f) { }

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
                ability.User.Animator.SetInteger(AnimatorHash.CastID, 3);
            }
        }

        private class Phase2 : AbilityPhase<SpiritFormAbility> {
            public Phase2(SpiritFormAbility ability) : base(ability, 1f) { }

            protected override void OnStart() {
                // prepare user transparency
                ability.User.StartTransparency();

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

        private class Phase3 : AbilityPhase<SpiritFormAbility> {
            public Phase3(SpiritFormAbility ability) : base(ability, manaCostPerSecond: 5) { }

            protected override void OnManaConsumption() {
                // heal user
                ability.User.Heal(10f);
            }

            protected override void OnManaDepletion() {
                ability.Abort();
            }

            public override void OnReactivation() {
                ability.Abort();
            }

            protected internal override void OnAttack() {
                ability.Abort();
            }
        }

        private class FinalPhase : DefaultFinalPhase<SpiritFormAbility> {
            public FinalPhase(SpiritFormAbility ability) : base(ability, 0.5f) { }

            protected override void OnStart() {
                ability.User.StopUltimate();

                // stop user transparency
                ability.User.StopTransparency();
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