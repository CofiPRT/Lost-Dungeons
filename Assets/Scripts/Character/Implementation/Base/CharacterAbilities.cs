using Character.Abilities;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public bool CastBlocksMovement { get; set; }
        public Ability[] Abilities { get; }

        public bool UseAbility(int index) {
            return Abilities[index].Use();
        }

        public void UpdateAbilities() {
            if (!IsAlive)
                return;

            foreach (var ability in Abilities)
                ability.Update();
        }
    }
}