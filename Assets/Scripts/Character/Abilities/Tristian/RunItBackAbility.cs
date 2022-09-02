using Character.Implementation.Player;
using Character.Misc;
using Game;
using UnityEngine;

namespace Character.Abilities.Tristian {
    public class RunItBackAbility : Ability<RunItBackAbility> {
        private const float Cooldown = 20f;
        private const float ManaCost = 10f;

        private GenericDecoy decoy;

        public RunItBackAbility(GenericPlayer user) : base(user, Cooldown) {
            phases = new AbilityPhase<RunItBackAbility>[] {
                new Phase1(this),
                new Phase2(this),
                new Phase3(this)
            };
            finalPhase = new DefaultFinalPhase<RunItBackAbility>(this);
        }

        public override bool Use() {
            var recast = active;

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

        private class Phase1 : AbilityPhase<RunItBackAbility> {
            public Phase1(RunItBackAbility ability) : base(ability, 1.0f) { }

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

                // spawn a decoy at the user's location
                var userTransform = ability.User.transform;

                ability.decoy = Object.Instantiate(
                    GameController.DefaultInstances.decoy,
                    ability.User.Pos,
                    userTransform.rotation,
                    userTransform.parent
                );

                // ensure the decoy spawns invisible
                ability.decoy.LerpTransparency(0f);
            }

            protected override void OnUpdate() {
                // lerp decoy transparency
                ability.decoy.LerpTransparency(Coefficient);
            }

            protected override void OnEnd() {
                // ensure decoy is fully visible
                ability.decoy.LerpTransparency(1f);

                // restore ability logic
                ability.User.UseFairFightLookDirection = true;
                ability.User.CanTakeDamage = true;
                ability.User.CastBlocksAbilityUsage = false;
                ability.User.CastBlocksMovementLookDirectionSync = false;
                ability.User.CastBlocksAttack = false;
                ability.User.CastBlocksBlock = false;
                ability.User.CastBlocksMovement = false;

                // stop casting
                ability.User.Animator.SetInteger(AnimatorHash.CastID, 0);
            }
        }

        private class Phase2 : AbilityPhase<RunItBackAbility> {
            public Phase2(RunItBackAbility ability) : base(ability, 0.5f, true, true) { }

            protected override void OnStart() {
                // prepare user transparency
                ability.User.StartTransparency();

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
            }

            protected override void OnUpdate() {
                // lerp decoy back to invisible
                ability.decoy.LerpTransparency(1f - Coefficient);

                // lerp user to invisible
                ability.User.LerpTransparency(1f - Coefficient);
            }

            protected override void OnEnd() {
                // teleport the user to the decoy's location
                ability.User.transform.position = ability.decoy.transform.position;

                // destroy the decoy
                Object.Destroy(ability.decoy.gameObject);
            }
        }

        private class Phase3 : AbilityPhase<RunItBackAbility> {
            public Phase3(RunItBackAbility ability) : base(ability, 0.5f) { }

            protected override void OnUpdate() {
                // lerp user back to visible
                ability.User.LerpTransparency(Coefficient);
            }

            protected override void OnEnd() {
                // stop user transparency
                ability.User.StopTransparency();

                // restore ability logic
                ability.User.UseFairFightLookDirection = true;
                ability.User.CanTakeDamage = true;
                ability.User.CastBlocksAbilityUsage = false;
                ability.User.CastBlocksMovementLookDirectionSync = false;
                ability.User.CastBlocksAttack = false;
                ability.User.CastBlocksBlock = false;
                ability.User.CastBlocksMovement = false;

                // stop casting
                ability.User.Animator.SetInteger(AnimatorHash.CastID, 0);
            }
        }
    }
}