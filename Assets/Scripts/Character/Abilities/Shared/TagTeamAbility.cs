using CameraScript;
using CameraScript.HUD;
using Character.Implementation.Player;
using Game;
using UnityEngine;

namespace Character.Abilities.Shared {
    public class TagTeamAbility : Ability<TagTeamAbility> {
        private const float Cooldown = 15f;
        private const float SharedManaCost = 25f;

        private const float GameTickSpeed = 0.1f;

        private bool changePlayers;
        private GenericPlayer otherUser;

        public TagTeamAbility(GenericPlayer user) : base(user, Cooldown, () => user.iconTagTeam) {
            phases = new AbilityPhase<TagTeamAbility>[] {
                new Phase1(this),
                new Phase2(this),
                new Phase3(this),
                new Phase4(this),
                new Phase5(this),
                new Phase6(this)
            };
            finalPhase = new FinalPhase(this);
        }

        public bool Use(bool performPlayerChange) {
            if (!base.Use())
                return false;

            if (GameController.OtherPlayer == null) {
                Reset();
                return false;
            }

            // both characters need to have enough mana
            if (!User.HasMana(SharedManaCost) || !GameController.OtherPlayer.HasMana(SharedManaCost)) {
                Reset();
                return false;
            }

            // consume the mana for both characters, and allow the ability to execute
            User.UseMana(SharedManaCost);
            GameController.OtherPlayer.UseMana(SharedManaCost);

            changePlayers = performPlayerChange;
            otherUser = GameController.OtherPlayer;

            return true;
        }

        private class Phase1 : AbilityPhase<TagTeamAbility> {
            public Phase1(TagTeamAbility ability) : base(ability, 0.5f, false) { }

            protected override void OnStart() {
                var startColor = ability.User.SignatureColor;
                var endColor = ability.changePlayers ? GameController.OtherPlayer.SignatureColor : startColor;

                // prepare the visual effects
                EffectsController.Prepare(startColor, endColor, 0.3f);

                // perform ability logic
                ability.User.StartTransparency();
                ability.User.CastBlocksAbilityUsage = true;
                ability.User.CastBlocksAttack = true;
                ability.User.CastBlocksBlock = true;
                ability.otherUser.StartTransparency();
                ability.otherUser.CastBlocksAbilityUsage = true;
                ability.otherUser.CastBlocksAttack = true;
                ability.otherUser.CastBlocksBlock = true;
            }

            protected override void OnUpdate() {
                // lerp game time and effects
                GameController.GameTickSpeed = Mathf.Lerp(1.0f, GameTickSpeed, Coefficient);
                EffectsController.Lerp(Coefficient, true, false);

                // lerp HUD if necessary
                if (ability.changePlayers) {
                    HUDController.LerpOtherSizeUp(Coefficient);
                    HUDController.LerpTransparency(1 - Coefficient);
                }
            }

            protected override void OnEnd() {
                // ensure lerps are finished
                GameController.GameTickSpeed = GameTickSpeed;
                EffectsController.Lerp(1.0f, true, false);

                if (ability.changePlayers) {
                    HUDController.LerpOtherSizeUp(1.0f);
                    HUDController.LerpTransparency(0.0f);
                }
            }
        }

        private class Phase2 : AbilityPhase<TagTeamAbility> {
            public Phase2(TagTeamAbility ability) : base(ability, 0.5f, false) { }

            protected override void OnUpdate() {
                // lerp transparency
                ability.User.LerpTransparency(1 - Coefficient);
                ability.otherUser.LerpTransparency(1 - Coefficient);
            }

            protected override void OnEnd() {
                // ensure lerps are finished
                ability.User.LerpTransparency(0);
                ability.otherUser.LerpTransparency(0);
            }
        }

        private class Phase3 : AbilityPhase<TagTeamAbility> {
            public Phase3(TagTeamAbility ability) : base(ability, 0.25f, false) { }

            protected override void OnEnd() {
                // swap positions and fair fights
                ability.User.SwapPositions(ability.otherUser);
                ability.User.SwapFairFights(ability.otherUser);

                // switch characters if necessary
                if (ability.changePlayers)
                    GameController.ChangePlayers();
            }
        }

        private class Phase4 : AbilityPhase<TagTeamAbility> {
            public Phase4(TagTeamAbility ability) : base(ability, 0.25f, false) { }

            protected override void OnUpdate() {
                // lerp colors and hud positions if necessary
                if (!ability.changePlayers) return;

                EffectsController.Lerp(Coefficient, false);
                HUDController.LerpPositions(Coefficient);
            }

            protected override void OnEnd() {
                if (!ability.changePlayers) return;

                // ensure lerps are finished
                EffectsController.Lerp(1.0f, false);
                HUDController.LerpPositions(1.0f);
            }
        }

        private class Phase5 : AbilityPhase<TagTeamAbility> {
            public Phase5(TagTeamAbility ability) : base(ability, 0.5f, false) { }

            protected override void OnUpdate() {
                // lerp transparency back in
                ability.User.LerpTransparency(Coefficient);
                ability.otherUser.LerpTransparency(Coefficient);
            }

            protected override void OnEnd() {
                // ensure lerps are finished
                ability.User.LerpTransparency(1);
                ability.otherUser.LerpTransparency(1);
            }
        }

        private class Phase6 : AbilityPhase<TagTeamAbility> {
            public Phase6(TagTeamAbility ability) : base(ability, 0.25f, false) { }

            protected override void OnEnd() {
                // ensure players are visible
                ability.User.StopTransparency();
                ability.otherUser.StopTransparency();
            }
        }

        private class FinalPhase : DefaultFinalPhase<TagTeamAbility> {
            public FinalPhase(TagTeamAbility ability) : base(ability, 0.5f, false) { }

            protected override void OnUpdate() {
                // lerp game time and effects back
                GameController.GameTickSpeed = Mathf.Lerp(GameTickSpeed, 1.0f, Coefficient);
                EffectsController.Lerp(1 - Coefficient, true, false);

                // lerp HUD back if necessary
                if (ability.changePlayers) {
                    HUDController.LerpOtherSizeDown(Coefficient);
                    HUDController.LerpTransparency(Coefficient);
                }
            }

            protected override void OnEnd() {
                // ensure lerps are finished
                GameController.GameTickSpeed = 1.0f;
                EffectsController.ResetEffects();

                if (ability.changePlayers) {
                    HUDController.LerpOtherSizeDown(1.0f);
                    HUDController.LerpTransparency(1.0f);
                }

                // also start the cooldown of the partner's ability
                ability.otherUser.AbilityTagTeam.StartCooldown();

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