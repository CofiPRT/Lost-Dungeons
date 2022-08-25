using System.Collections.Generic;
using Character.Abilities;
using Character.Abilities.Shared;
using Character.Implementation.Ally;
using Character.Implementation.Base;

namespace Character.Implementation.Player {
    public abstract class GenericPlayer : GenericAlly {
        protected GenericPlayer(CharacterBuilder data) : base(data) {
            AbilityDodge = new DodgeAbility(this);
        }

        /* Shared Abilities */

        public Ability AbilityDodge { get; }
        public Ability AbilitySwitch { get; }
        public Ability AbilityTagTeam { get; }

        /* Specific Abilities */

        public Ability Ability1 { get; }
        public Ability Ability2 { get; }
        public Ability Ultimate { get; }

        /* Ability Logic */

        public bool CastBlocksAbilityUsage { get; set; }
        public bool CastBlocksMovement { get; set; }

        public IEnumerable<Ability> Abilities => new[]
            { AbilityDodge, AbilitySwitch, AbilityTagTeam, Ability1, Ability2, Ultimate };

        public void UpdateAbilities() {
            if (!IsAlive)
                return;

            foreach (var ability in Abilities)
                ability.Update();
        }

        /* Parent */

        public override bool CanApplyMovement => base.CanApplyMovement && !CastBlocksMovement;
        protected override UpdateDelegate UpdateActions => base.UpdateActions + UpdateAbilities;
    }
}