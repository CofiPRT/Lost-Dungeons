namespace Character.Attributes {
    public interface IHasHealth {
        bool IsAlive { get; }
        float DeathTime { get; set; }
        float Health { get; set; }
        float MaxHealth { get; }
        float TakeDamage(float damage, Implementation.Base.GenericCharacter source = null);
        void Heal(float healAmount);
        void OnDeath();
        void UpdateDeathTime();
    }
}