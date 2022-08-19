namespace Character.Scripts.Properties {
    public interface IHasHealth {
        bool IsAlive { get; set; }
        float Health { get; set; }
        float MaxHealth { get; }
        float TakeDamage(float damage, BaseImplementations.Character source = null);
        void Heal(float healAmount);
        void OnDeath();
    }
}