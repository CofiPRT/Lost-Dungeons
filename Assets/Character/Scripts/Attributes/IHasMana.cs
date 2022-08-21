namespace Character.Scripts.Attributes {
    public interface IHasMana {
        float Mana { get; set; }
        float MaxMana { get; }
        bool UseMana(float manaCost);
        void ReplenishMana(float manaAmount);
    }
}