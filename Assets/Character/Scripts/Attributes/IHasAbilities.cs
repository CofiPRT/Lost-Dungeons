using Character.Scripts.Abilities;

namespace Character.Scripts.Attributes {
    public interface IHasAbilities {
        Ability[] Abilities { get; }
        bool UseAbility(int index);
        void UpdateAbilities();
    }
}