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
            AbilityTagTeam = new TagTeamAbility(this);
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
        public SwitchAbility AbilitySwitch { get; }
        public TagTeamAbility AbilityTagTeam { get; }

        /* Specific Abilities */

        public IAbility Ability1 { get; }
        public IAbility Ability2 { get; }
        public IAbility Ultimate { get; }

        /* Ability Logic */

        public bool CastBlocksAbilityUsage { get; set; }
        public bool CastBlocksMovement { get; set; }
        public bool CastBlocksAttack { get; set; }
        public bool CastBlocksBlock { get; set; }

        public IEnumerable<IAbility> Abilities => new IAbility[]
            { AbilityDodge, AbilitySwitch, AbilityTagTeam /*Ability1, Ability2, Ultimate*/ };

        public void UpdateAbilities() {
            if (!IsAlive)
                return;

            foreach (var ability in Abilities)
                ability.Update();
        }

        public bool StartDodge(Vector2 direction) {
            return AbilityDodge.Use(direction);
        }

        public bool StartTagTeam(bool changePlayers) {
            return AbilityTagTeam.Use(changePlayers);
        }

        /* Util */

        public void SwapPositions(GenericPlayer other) {
            var ownTransform = transform;
            var otherTransform = other.transform;

            (ownTransform.position, otherTransform.position) = (otherTransform.position, ownTransform.position);
        }

        public void SwapFairFights(GenericPlayer other) {
            (FairFight, other.FairFight) = (other.FairFight, FairFight);

            // also change owners of the fair fights
            FairFight.Owner = this;
            other.FairFight.Owner = other;
        }

        /* Parent */

        protected override bool CanApplyMovement => base.CanApplyMovement && !CastBlocksMovement;
        protected override bool CanStartAttack => base.CanStartAttack && !CastBlocksAttack;
        protected override bool CanStartBlocking => base.CanStartBlocking && !CastBlocksBlock;

        protected override UpdateDelegate UpdateActions => base.UpdateActions + UpdateAbilities;
    }
}