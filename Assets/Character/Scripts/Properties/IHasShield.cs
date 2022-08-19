namespace Character.Scripts.Properties {
    public interface IHasShield {
        bool IsBlocking { get; set; }
        float ShieldAngle { get; }
        float ShieldRechargeTime { get; }
        float ShieldCooldown { get; set; }
        BlockStrength BlockStrength { get; }
        float AttemptBlock(float damage, AttackStrength attackStrength, BaseImplementations.Character source);
        void StartBlocking();
        void StopBlocking(bool forced = false);
    }
}