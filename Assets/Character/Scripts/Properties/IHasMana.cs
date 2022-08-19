namespace Character.Scripts.Properties {
    public interface IHasMana {
        float Mana { get; }
        float MaxMana { get; }
        bool HasEnoughMana(float manaCost);
        void UseMana(float manaCost);
        void ReplenishMana(float manaAmount);
    }
}