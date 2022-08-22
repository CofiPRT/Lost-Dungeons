using Character.Abilities;

namespace Character.Attributes {
    public interface IHasAbilities {
        bool CastBlocksMovement { get; set; }
        Ability[] Abilities { get; }
        bool UseAbility(int index);
        void UpdateAbilities();
    }
}