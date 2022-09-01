using System.Collections.Generic;
using Character.Abilities;
using Character.Abilities.Shared;
using Character.Implementation.Ally;
using Character.Implementation.Base;
using UnityEngine;

namespace Character.Implementation.Player {
    public abstract class GenericPlayer : GenericAlly {
        protected GenericPlayer(string name, Color signatureColor) : base(CreateData(name)) {
            SignatureColor = signatureColor;
            AbilityDodge = new DodgeAbility(this);
            AbilitySwitch = new SwitchAbility(this);
        }

        private static CharacterBuilder CreateData(string name) {
            return new CharacterBuilder {
                name = name,
                team = Properties.Team.Player
            };
        }

        public Color SignatureColor { get; }

        /* Shared Abilities */

        public DodgeAbility AbilityDodge { get; }
        public Ability AbilitySwitch { get; }
        public Ability AbilityTagTeam { get; }

        /* Specific Abilities */

        public Ability Ability1 { get; }
        public Ability Ability2 { get; }
        public Ability Ultimate { get; }

        /* Ability Logic */

        public bool CastBlocksAbilityUsage { get; set; }
        public bool CastBlocksMovement { get; set; }
        public bool CastBlocksAttack { get; set; }
        public bool CastBlocksBlock { get; set; }

        public IEnumerable<Ability> Abilities => new[]
            { AbilityDodge, AbilitySwitch /*, AbilityTagTeam, Ability1, Ability2, Ultimate*/ };

        public void UpdateAbilities() {
            if (!IsAlive)
                return;

            foreach (var ability in Abilities)
                ability.Update();
        }

        public bool StartDodge(Vector2 direction) {
            return AbilityDodge.Use(direction);
        }

        /* Parent */

        protected override bool CanApplyMovement => base.CanApplyMovement && !CastBlocksMovement;
        protected override bool CanStartAttack => base.CanStartAttack && !CastBlocksAttack;
        protected override bool CanStartBlocking => base.CanStartBlocking && !CastBlocksBlock;

        protected override UpdateDelegate UpdateActions => base.UpdateActions + UpdateAbilities;
    }
}