using System;
using System.Linq;
using Camera;
using Character.Implementation.Player;
using Game;
using UnityEngine;

namespace Character.Abilities.Shared {
    public class DodgeAbility : Ability {
        private const float Cooldown = 5f;
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

            protected override void OnUpdate() {
                if (!ability.dodged)
                    return;

                // lerp the game speed and the player speed
                GameController.Instance.gameTickSpeed = Mathf.Lerp(1.0f, 0.1f, Coefficient);
                GameController.Instance.playerTickFactor = Mathf.Lerp(1.0f, 5.0f, Coefficient);
            }

            protected override void OnEnd() {
                // stop the animation
                ability.user.Animator.SetBool(ability.Hash, false);

                // if the player dodged an attack, ensure the lerps are finished
                if (ability.dodged) {
                    GameController.Instance.gameTickSpeed = 0.1f;
                    GameController.Instance.playerTickFactor = 5.0f;
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
                GameController.Instance.gameTickSpeed = Mathf.Lerp(0.1f, 1.0f, Coefficient);
                GameController.Instance.playerTickFactor = Mathf.Lerp(5.0f, 1.0f, Coefficient);
            }

            protected override void OnEnd() {
                // ensure the lerps are finished
                GameController.Instance.gameTickSpeed = 1.0f;
                GameController.Instance.playerTickFactor = 1.0f;
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