using Character.Implementation.Enemy;
using Character.Implementation.Player;
using Character.Misc;
using Props;
using UnityEngine;

namespace Character.Abilities.Tristian {
    public class NaturesMightAbility : Ability<NaturesMightAbility> {
        private const float Cooldown = 10f;
        private const float ManaCost = 5f;

        private GenericProp prop;
        private GenericEnemy target;

        public NaturesMightAbility(GenericPlayer user) : base(user, Cooldown) {
            phases = new AbilityPhase<NaturesMightAbility>[] {
                new Phase1(this),
                new Phase2(this),
                new Phase3(this)
            };
            finalPhase = new DefaultFinalPhase<NaturesMightAbility>(this);
        }

        public override bool Use() {
            if (!base.Use())
                return false;

            // look for a prop in the direction of the camera
            var foundProp = User.FindObjectInCone<GenericProp>("Prop", x => x.Pos);
            if (foundProp == null) {
                Reset();
                return false;
            }

            // find an enemy close to the player
            var foundEnemy = GenericPlayer.FindEnemyInPropDistance(User.Pos);
            if (foundEnemy == null) {
                Reset();
                return false;
            }

            // check for mana
            if (!User.UseMana(ManaCost)) {
                Reset();
                return false;
            }

            prop = foundProp;
            target = foundEnemy;

            return true;
        }

        private class Phase1 : AbilityPhase<NaturesMightAbility> {
            public Phase1(NaturesMightAbility ability) : base(ability, 0.5f) { }

            protected override void OnStart() {
                // ability logic
                ability.User.UseFairFightLookDirection = false;
                ability.User.CastBlocksAbilityUsage = true;
                ability.User.CastBlocksMovementLookDirectionSync = true;
                ability.User.CastBlocksAttack = true;
                ability.User.CastBlocksBlock = true;

                // start the casting animation
                ability.User.Animator.SetInteger(AnimatorHash.CastID, 1);

                // start the prop animation
                ability.prop.StartCast();
            }

            protected override void OnUpdate() {
                // look towards prop
                ability.User.LookDirection = ability.prop.Pos2D - ability.User.Pos2D;

                // lerp object scale
                ability.prop.CastLerpSize(1 - Coefficient);
            }

            protected override void OnEnd() {
                // ensure the lerp has finished
                ability.prop.CastLerpSize(0);
            }
        }

        private class Phase2 : AbilityPhase<NaturesMightAbility> {
            public Phase2(NaturesMightAbility ability) : base(ability, 0.25f) { }

            protected override void OnUpdate() {
                // lerp object scale back
                ability.prop.CastLerpSize(Coefficient);

                // keep object in front of player
                ability.prop.transform.position = ability.User.Pos + ability.User.Forward * 1.5f + Vector3.up * 2f;
            }

            protected override void OnEnd() {
                // ensure the lerp has finished
                ability.prop.CastLerpSize(1);
            }
        }

        private class Phase3 : AbilityPhase<NaturesMightAbility> {
            public Phase3(NaturesMightAbility ability) : base(ability, 0.25f) { }

            protected override void OnUpdate() {
                // keep object in front of player
                ability.prop.transform.position = ability.User.Pos + ability.User.Forward * 1.5f + Vector3.up * 2f;
            }

            protected override void OnEnd() {
                // launch object
                ability.prop.LaunchTowards(ability.target, ability.User);

                // stop casting
                ability.User.Animator.SetInteger(AnimatorHash.CastID, 0);

                // stop ability logic
                ability.User.UseFairFightLookDirection = true;
                ability.User.CastBlocksAbilityUsage = false;
                ability.User.CastBlocksMovementLookDirectionSync = false;
                ability.User.CastBlocksAttack = false;
                ability.User.CastBlocksBlock = false;
            }
        }
    }
}