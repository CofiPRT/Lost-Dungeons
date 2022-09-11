using Character.Implementation.Enemy;
using Character.Implementation.Player;
using Character.Misc;
using Props;

namespace Character.Abilities.Reinald {
    public class NaturesTricksAbility : Ability<NaturesTricksAbility> {
        private const float Cooldown = 10f;
        private const float ManaCost = 5f;

        private GenericProp prop;
        private GenericEnemy target;

        public NaturesTricksAbility(GenericPlayer user) : base(user, Cooldown, () => user.iconAbility1) {
            phases = new AbilityPhase<NaturesTricksAbility>[] {
                new Phase1(this)
            };
            finalPhase = new DefaultFinalPhase<NaturesTricksAbility>(this);
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
            var foundEnemy = GenericPlayer.FindEnemyInPropDistance(foundProp.Pos);
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

        private class Phase1 : AbilityPhase<NaturesTricksAbility> {
            public Phase1(NaturesTricksAbility ability) : base(ability, 1f) { }

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

                // play sound
                ability.User.PlaySound(ability.User.castSound);
            }

            protected override void OnUpdate() {
                // look towards prop
                ability.User.LookDirection = ability.prop.Pos2D - ability.User.Pos2D;

                // lerp prop position
                ability.prop.CastLift(Coefficient);
            }

            protected override void OnEnd() {
                // ensure the lerp has finished
                ability.prop.CastLift(1f);

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