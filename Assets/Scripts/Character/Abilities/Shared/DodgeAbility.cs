using System.Linq;
using CameraScript;
using Character.Implementation.Player;
using Character.Misc;
using Game;
using UnityEngine;

namespace Character.Abilities.Shared {
    public class DodgeAbility : Ability<DodgeAbility> {
        private const float Cooldown = 5f;

        private const float GameTickSpeed = 0.1f;
        private const float PlayerTickFactor = 8.0f;

        private Vector2 dashDirection;
        private bool dodged;

        public DodgeAbility(GenericPlayer user) : base(user, Cooldown, () => user.iconDodge) {
            phases = new AbilityPhase<DodgeAbility>[] {
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

        private class Phase1 : AbilityPhase<DodgeAbility> {
            public Phase1(DodgeAbility ability) : base(ability, 0.25f) { }

            protected override void OnStart() {
                // compute the dash direction relative to body orientation
                var relativeDashDirection = ability.User.RelativizeToForwardDirection(ability.dashDirection);

                // run the animation
                ability.User.Animator.SetBool(AnimatorHash.Dodging, true);
                ability.User.Animator.SetFloat(AnimatorHash.ForwardSpeedDodge, relativeDashDirection.y);
                ability.User.Animator.SetFloat(AnimatorHash.SideSpeedDodge, relativeDashDirection.x);

                ability.User.CanTakeDamage = false;
                ability.User.UseFairFightLookDirection = false;
                ability.User.CastBlocksAbilityUsage = true;
                ability.User.CastBlocksMovement = true;
                ability.User.IgnoreCollisions = true;

                // test if the player dodged an attack
                ability.dodged = ability.User.GetOpponentsInAttackRange()
                    .Any(
                        opponent => opponent.IsPreparingToAttack &&
                                    opponent.OpponentInAttackArea(ability.User)
                    );

                // play sound
                ability.User.PlaySound(ability.User.dodgeSound);
            }
        }

        private class Phase2 : AbilityPhase<DodgeAbility> {
            public Phase2(DodgeAbility ability) : base(ability, 0.75f) { }

            protected override void OnStart() {
                ability.User.StopMoving();

                if (!ability.dodged)
                    return;

                // prepare the visual effects
                EffectsController.Prepare();

                // play sound
                ability.User.PlaySound(ability.User.slowMoInSound);
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
                ability.User.Animator.SetBool(AnimatorHash.Dodging, false);
                ability.User.CanTakeDamage = true;
                ability.User.UseFairFightLookDirection = true;
                ability.User.CastBlocksMovement = false;
                ability.User.IgnoreCollisions = false;
                ability.User.RestoreCollisions();

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

        private class Phase3 : AbilityPhase<DodgeAbility> {
            public Phase3(DodgeAbility ability) : base(ability, 5f, false) { }
        }

        private class FinalPhase : DefaultFinalPhase<DodgeAbility> {
            public FinalPhase(DodgeAbility ability) : base(ability, 1f, false) { }

            protected override void OnUpdate() {
                if (!ability.dodged)
                    return;

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
                ability.User.CastBlocksAbilityUsage = false;

                // also start the cooldown of the partner's ability
                if (GameController.OtherPlayer != null)
                    GameController.OtherPlayer.AbilityDodge.StartCooldown();

                base.OnEnd();
            }
        }
    }
}