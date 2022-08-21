using Scripts.Character.Abilities;

namespace Scripts.Character.Attributes {
    public interface IHasAbilities {
        Ability[] Abilities { get; }
        bool UseAbility(int index);
        void UpdateAbilities();
    }
}