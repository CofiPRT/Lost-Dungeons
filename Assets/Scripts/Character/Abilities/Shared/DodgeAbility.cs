using System.Linq;
using Camera;
using Character.Implementation.Player;
using Game;
using UnityEngine;

namespace Character.Abilities.Shared {
    public class DodgeAbility : Ability {
        private const float Cooldown = 5f;

        private const float GameTickSpeed = 0.1f;
        private const float PlayerTickFactor = 8.0f;

        private Vector2 dashDirection;
        private bool dodged;

        public DodgeAbility(GenericPlayer user) : base(user, Cooldown) {
            phases = new AbilityPhase[] {
                new Phase1(this),
                new Phase2(this),
                new Phase3(this)
            };
            finalPhase = new FinalPhase(this);
        }

        public bool Use(Vector2 direction) {
            if (!base.Use())
                return false;

            dashDirection = direction;
            dodged = false;

            return true;
        }

        private class Phase1 : AbilityPhase {
            private new readonly DodgeAbility ability;

            public Phase1(DodgeAbility ability) : base(ability, 0.25f) {
                this.ability = ability;
            }

            protected override void OnStart() {
                // compute the dash direction relative to body orientation
                var relativeDashDirection = ability.user.RelativizeToForwardDirection(ability.dashDirection);

                // run the animation
                ability.user.Animator.SetBool(AnimatorHash.Dodging, true);
                ability.user.Animator.SetFloat(AnimatorHash.ForwardSpeedDodge, relativeDashDirection.y);
                ability.user.Animator.SetFloat(AnimatorHash.SideSpeedDodge, relativeDashDirection.x);

                ability.user.CanTakeDamage = false;
                ability.user.UseFairFightLookDirection = false;
                ability.user.CastBlocksAbilityUsage = true;
                ability.user.CastBlocksMovement = true;
                ability.user.IgnoreCollisions = true;

                // test if the player dodged an attack
                ability.dodged = ability.user.GetOpponentsInAttackRange()
                    .Any(
                        opponent => opponent.IsPreparingToAttack &&
                                    opponent.OpponentInAttackArea(ability.user)
                    );
            }
        }

        private class Phase2 : AbilityPhase {
            private new readonly DodgeAbility ability;

            public Phase2(DodgeAbility ability) : base(ability, 0.75f) {
                this.ability = ability;
            }

            protected override void OnStart() {
                ability.user.StopMoving();

                if (!ability.dodged)
                    return;

                // prepare the visual effects
                EffectsController.Prepare();
            }

            protected override void OnUpdate() {
                if (!ability.dodged)
                    return;

                // lerp the game speed and the player speed
                GameController.GameTickSpeed = Mathf.Lerp(1.0f, GameTickSpeed, Coefficient);
                GameController.PlayerTickFactor =
                    Mathf.Lerp(1.0f, GameTickSpeed * PlayerTickFactor, Coefficient) / GameController.GameTickSpeed;

                // lerp effects
                EffectsController.Lerp(Coefficient);
            }

            protected override void OnEnd() {
                // stop the animation
                ability.user.Animator.SetBool(AnimatorHash.Dodging, false);
                ability.user.CanTakeDamage = true;
                ability.user.UseFairFightLookDirection = true;
                ability.user.CastBlocksMovement = false;
                ability.user.IgnoreCollisions = false;
                ability.user.RestoreCollisions();

                // if the player dodged an attack, ensure the lerps are finished
                if (ability.dodged) {
                    GameController.GameTickSpeed = GameTickSpeed;
                    GameController.PlayerTickFactor = PlayerTickFactor;

                    // also ensure camera effect lerps are finished
                    EffectsController.Lerp(1.0f);
                } else {
                    // skip Phase3 if the player didn't dodge an attack
                    ability.Abort();
                }
            }
        }

        private class Phase3 : AbilityPhase {
            public Phase3(Ability ability) : base(ability, 5f, false) { }
        }

        private class FinalPhase : DefaultFinalPhase {
            public FinalPhase(Ability ability) : base(ability, 1f, false) { }

            protected override void OnUpdate() {
                // lerp the game speed and the player speed back
                GameController.GameTickSpeed = Mathf.Lerp(GameTickSpeed, 1.0f, Coefficient);
                GameController.PlayerTickFactor =
                    Mathf.Lerp(GameTickSpeed * PlayerTickFactor, 1.0f, Coefficient) / GameController.GameTickSpeed;

                // lerp the camera effects back to normal
                EffectsController.Lerp(1 - Coefficient);
            }

            protected override void OnEnd() {
                // ensure the lerps are finished
                GameController.GameTickSpeed = 1.0f;
                GameController.PlayerTickFactor = 1.0f;

                // stop the visual effects
                EffectsController.ResetEffects();

                // allow the player to use abilities again
                ability.user.CastBlocksAbilityUsage = false;

                // also start the cooldown of the partner's ability
                GameController.OtherPlayer.AbilityDodge.StartCooldown();

                base.OnEnd();
            }
        }
    }
}