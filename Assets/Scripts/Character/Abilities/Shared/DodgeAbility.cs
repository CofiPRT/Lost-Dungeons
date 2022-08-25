using System.Linq;
using Camera;
using Character.Implementation.Player;
using Game;
using UnityEngine;
using UnityEngine.Rendering;

namespace Character.Abilities.Shared {
    public class DodgeAbility : Ability {
        private const float Cooldown = 5f;

        private static readonly Color VignetteColor = Color.white;
        private const float VignetteIntensity = 0.25f;
        private const float FilmGrainIntensity = 1.0f;
        private const float LensDistortionIntensity = -0.5f;

        private DashDirection dashDirection;
        private bool dodged;

        public DodgeAbility(GenericPlayer user) : base(user, Cooldown) {
            phases = new AbilityPhase[] {
                new Phase1(this),
                new Phase2(this),
                new Phase3(this)
            };
            finalPhase = new FinalPhase(this);
        }

        private int Hash => dashDirection switch {
            DashDirection.Forward => AnimatorHash.DodgingForward,
            DashDirection.Backward => AnimatorHash.DodgingBackward,
            DashDirection.Left => AnimatorHash.DodgingLeft,
            DashDirection.Right => AnimatorHash.DodgingRight,
            _ => AnimatorHash.DodgingBackward
        };

        public void Use(DashDirection direction) {
            if (!base.Use())
                return;

            dashDirection = direction;
            dodged = false;
        }

        private class Phase1 : AbilityPhase {
            private new readonly DodgeAbility ability;

            public Phase1(DodgeAbility ability) : base(ability, 0.5f, false) {
                this.ability = ability;
            }

            protected override void OnStart() {
                // instantly make the player look towards the direction of the camera
                ability.user.RigidBody.MoveRotation(Quaternion.LookRotation(CameraController.Forward2D));

                // run the animation
                ability.user.Animator.SetBool(ability.Hash, true);
                ability.user.CastBlocksAbilityUsage = true;
                ability.user.CastBlocksMovement = true;

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

            public Phase2(DodgeAbility ability) : base(ability, 0.5f, false) {
                this.ability = ability;
            }

            protected override void OnStart() {
                if (!ability.dodged)
                    return;

                // prepare the visual effects
                EffectsController.FilmGrain.active = true;
                EffectsController.Vignette.active = true;
                EffectsController.Vignette.color = new ColorParameter(VignetteColor);
                EffectsController.MotionBlur.active = true;
            }

            protected override void OnUpdate() {
                if (!ability.dodged)
                    return;

                // lerp the game speed and the player speed
                GameController.GameTickSpeed = Mathf.Lerp(1.0f, 0.1f, Coefficient);
                GameController.PlayerTickFactor = Mathf.Lerp(1.0f, 5.0f, Coefficient);

                // lerp effects
                EffectsController.FilmGrain.intensity.value = Mathf.Lerp(0.0f, FilmGrainIntensity, Coefficient);
                EffectsController.Vignette.intensity.value = Mathf.Lerp(0.0f, VignetteIntensity, Coefficient);
                EffectsController.LensDistortion.intensity.value =
                    Mathf.Lerp(0.0f, LensDistortionIntensity, Coefficient);
            }

            protected override void OnEnd() {
                // stop the animation
                ability.user.Animator.SetBool(ability.Hash, false);
                ability.user.CastBlocksMovement = false;

                // if the player dodged an attack, ensure the lerps are finished
                if (ability.dodged) {
                    GameController.GameTickSpeed = 0.1f;
                    GameController.PlayerTickFactor = 5.0f;

                    // also ensure camera effect lerps are finished
                    EffectsController.FilmGrain.intensity.value = FilmGrainIntensity;
                    EffectsController.Vignette.intensity.value = VignetteIntensity;
                    EffectsController.LensDistortion.intensity.value = LensDistortionIntensity;
                } else {
                    // skip Phase3 if the player didn't dodge an attack
                    ability.Abort();
                }
            }
        }

        private class Phase3 : AbilityPhase {
            public Phase3(DodgeAbility ability) : base(ability, 2.5f, false) { }
        }

        private class FinalPhase : AbilityPhase {
            public FinalPhase(DodgeAbility ability) : base(ability, 0.5f, false) { }

            protected override void OnUpdate() {
                // lerp the game speed and the player speed back
                GameController.GameTickSpeed = Mathf.Lerp(0.1f, 1.0f, Coefficient);
                GameController.PlayerTickFactor = Mathf.Lerp(5.0f, 1.0f, Coefficient);

                // lerp the camera effects back to normal
                EffectsController.FilmGrain.intensity.value = Mathf.Lerp(FilmGrainIntensity, 0.0f, Coefficient);
                EffectsController.Vignette.intensity.value = Mathf.Lerp(VignetteIntensity, 0.0f, Coefficient);
                EffectsController.LensDistortion.intensity.value =
                    Mathf.Lerp(LensDistortionIntensity, 0.0f, Coefficient);
            }

            protected override void OnEnd() {
                // ensure the lerps are finished
                GameController.GameTickSpeed = 1.0f;
                GameController.PlayerTickFactor = 1.0f;

                // stop the visual effects
                EffectsController.FilmGrain.active = false;
                EffectsController.Vignette.active = false;
                EffectsController.MotionBlur.active = false;

                // allow the player to use abilities again
                ability.user.CastBlocksAbilityUsage = false;

                base.OnEnd();
            }
        }
    }

    public enum DashDirection {
        Forward,
        Backward,
        Left,
        Right
    }
}