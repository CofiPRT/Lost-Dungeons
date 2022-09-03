using Camera;
using Camera.HUD;
using Character.Implementation.Player;
using Game;
using UnityEngine;

namespace Character.Abilities.Shared {
    public class SwitchAbility : Ability<SwitchAbility> {
        private const float Cooldown = 1f;

        private const float GameTickSpeed = 0.1f;

        private GenericPlayer otherUser;

        public SwitchAbility(GenericPlayer user) : base(user, Cooldown, null) {
            phases = new AbilityPhase<SwitchAbility>[] {
                new Phase1(this),
                new Phase2(this),
                new Phase3(this),
                new Phase4(this)
            };
            finalPhase = new FinalPhase(this);
        }

        public new bool Use() {
            if (!base.Use())
                return false;

            otherUser = GameController.OtherPlayer;

            return true;
        }

        private class Phase1 : AbilityPhase<SwitchAbility> {
            public Phase1(SwitchAbility ability) : base(ability, 0.5f, false) { }

            protected override void OnStart() {
                var startColor = ability.User.SignatureColor;
                var endColor = GameController.OtherPlayer.SignatureColor;

                // prepare the visual effects
                EffectsController.Prepare(startColor, endColor, 0.3f);

                // perform ability logic
                ability.User.CastBlocksAbilityUsage = true;
                ability.User.CastBlocksAttack = true;
                ability.User.CastBlocksBlock = true;
                ability.otherUser.CastBlocksAbilityUsage = true;
                ability.otherUser.CastBlocksAttack = true;
                ability.otherUser.CastBlocksBlock = true;
            }

            protected override void OnUpdate() {
                // lerp game time, effects, and HUD
                GameController.GameTickSpeed = Mathf.Lerp(1.0f, GameTickSpeed, Coefficient);
                EffectsController.Lerp(Coefficient, true, false);
                HUDController.LerpOtherSizeUp(Coefficient);
                HUDController.LerpTransparency(1 - Coefficient);
            }

            protected override void OnEnd() {
                // ensure lerps are finished
                GameController.GameTickSpeed = GameTickSpeed;
                EffectsController.Lerp(1.0f, true, false);
                HUDController.LerpOtherSizeUp(1.0f);
                HUDController.LerpTransparency(0.0f);
            }
        }

        private class Phase2 : AbilityPhase<SwitchAbility> {
            public Phase2(SwitchAbility ability) : base(ability, 0.5f, false) { }

            protected override void OnEnd() {
                GameController.ChangePlayers();
            }
        }

        private class Phase3 : AbilityPhase<SwitchAbility> {
            public Phase3(SwitchAbility ability) : base(ability, 0.5f, false) { }

            protected override void OnUpdate() {
                // lerp color
                EffectsController.Lerp(Coefficient, false);

                // lerp HUD positions
                HUDController.LerpPositions(Coefficient);
            }

            protected override void OnEnd() {
                // ensure lerps are finished
                EffectsController.Lerp(1.0f, false);
                HUDController.LerpPositions(1.0f);
            }
        }

        private class Phase4 : AbilityPhase<SwitchAbility> {
            public Phase4(SwitchAbility ability) : base(ability, 0.5f, false) { }
        }

        private class FinalPhase : DefaultFinalPhase<SwitchAbility> {
            public FinalPhase(SwitchAbility ability) : base(ability, 0.5f, false) { }

            protected override void OnUpdate() {
                // lerp game time, effects, and HUD back
                GameController.GameTickSpeed = Mathf.Lerp(GameTickSpeed, 1.0f, Coefficient);
                EffectsController.Lerp(1 - Coefficient, true, false);
                HUDController.LerpOtherSizeDown(Coefficient);
                HUDController.LerpTransparency(Coefficient);
            }

            protected override void OnEnd() {
                // ensure lerps are finished, stop the visual effects
                GameController.GameTickSpeed = 1.0f;
                EffectsController.ResetEffects();
                HUDController.LerpOtherSizeDown(1.0f);
                HUDController.LerpTransparency(1.0f);

                // also start the cooldown of the partner's ability
                ability.otherUser.AbilitySwitch.StartCooldown();

                // end ability logic
                ability.User.CastBlocksAbilityUsage = false;
                ability.User.CastBlocksAttack = false;
                ability.User.CastBlocksBlock = false;
                ability.User.StopBlocking();
                ability.otherUser.CastBlocksAbilityUsage = false;
                ability.otherUser.CastBlocksAttack = false;
                ability.otherUser.CastBlocksBlock = false;

                base.OnEnd();
            }
        }
    }
}