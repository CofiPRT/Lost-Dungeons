namespace Character.Scripts.Attributes {
    public interface IHasHealth {
        bool IsAlive { get; }
        float DeathTime { get; set; }
        float Health { get; set; }
        float MaxHealth { get; }
        float TakeDamage(float damage, Generic.Character source = null);
        void Heal(float healAmount);
        void OnDeath();
    }
}