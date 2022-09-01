using Camera;
using Character.Implementation.Player;
using Game;
using UnityEngine;

namespace Character.Abilities.Shared {
    public class SwitchAbility : Ability {
        private const float Cooldown = 1f;

        private const float GameTickSpeed = 0.1f;

        public SwitchAbility(GenericPlayer user) : base(user, Cooldown) {
            phases = new AbilityPhase[] {
                new Phase1(this),
                new Phase2(this),
                new Phase3(this),
                new Phase4(this)
            };
            finalPhase = new FinalPhase(this);
        }

        private class Phase1 : AbilityPhase {
            public Phase1(Ability ability) : base(ability, 0.5f, false) { }

            protected override void OnStart() {
                var startColor = ability.user.SignatureColor;
                var endColor = GameController.OtherPlayer.SignatureColor;

                // prepare the visual effects
                EffectsController.Prepare(startColor, endColor, 0.3f);

                // perform ability logic
                ability.user.CastBlocksAbilityUsage = true;
                ability.user.CastBlocksAttack = true;
                ability.user.CastBlocksBlock = true;
                GameController.OtherPlayer.CastBlocksAbilityUsage = true;
                GameController.OtherPlayer.CastBlocksAttack = true;
                GameController.OtherPlayer.CastBlocksBlock = true;
            }

            protected override void OnUpdate() {
                // lerp game time, effects, and HUD
                GameController.GameTickSpeed = Mathf.Lerp(1.0f, GameTickSpeed, Coefficient);
                EffectsController.Lerp(Coefficient, true, false);
                HUDController.LerpOtherSizeUp(Coefficient);
            }

            protected override void OnEnd() {
                // ensure lerps are finished
                GameController.GameTickSpeed = GameTickSpeed;
                EffectsController.Lerp(1.0f, true, false);
                HUDController.LerpOtherSizeUp(1.0f);
            }
        }

        private class Phase2 : AbilityPhase {
            public Phase2(Ability ability) : base(ability, 0.5f, false) { }

            protected override void OnEnd() {
                GameController.ChangePlayers();
            }
        }

        private class Phase3 : AbilityPhase {
            public Phase3(Ability ability) : base(ability, 0.5f, false) { }

            protected override void OnUpdate() {
                // lerp color
                EffectsController.Lerp(Coefficient, false);

                // lerp HUD positions
                HUDController.LerpPositions(Coefficient);
            }
        }

        private class Phase4 : AbilityPhase {
            public Phase4(Ability ability) : base(ability, 0.5f, false) { }
        }

        private class FinalPhase : DefaultFinalPhase {
            public FinalPhase(Ability ability) : base(ability, 0.5f, false) { }

            protected override void OnUpdate() {
                // lerp game time, effects, and HUD back
                GameController.GameTickSpeed = Mathf.Lerp(GameTickSpeed, 1.0f, Coefficient);
                EffectsController.Lerp(1 - Coefficient, true, false);
                HUDController.LerpOtherSizeDown(Coefficient);
            }

            protected override void OnEnd() {
                // ensure lerps are finished, stop the visual effects
                GameController.GameTickSpeed = 1.0f;
                EffectsController.ResetEffects();
                HUDController.LerpOtherSizeDown(1.0f);

                // also start the cooldown of the partner's ability
                GameController.OtherPlayer.AbilitySwitch.StartCooldown();

                // end ability logic
                ability.user.CastBlocksAbilityUsage = false;
                ability.user.CastBlocksAttack = false;
                ability.user.CastBlocksBlock = false;
                ability.user.StopBlocking();
                GameController.ControlledPlayer.CastBlocksAbilityUsage = false;
                GameController.ControlledPlayer.CastBlocksAttack = false;
                GameController.ControlledPlayer.CastBlocksBlock = false;

                base.OnEnd();
            }
        }
    }
}