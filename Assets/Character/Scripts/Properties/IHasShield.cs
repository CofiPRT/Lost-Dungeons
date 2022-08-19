namespace Character.Scripts.Properties {
    public interface IHasShield {
        bool IsBlocking { get; }
        void AttemptBlock(float damage, AttackStrength attackStrength, IHasWeapon source);
        void OnBlockSuccess(float damage, AttackStrength attackStrength, IHasWeapon source);
        void OnBlockFail(float damage, AttackStrength attackStrength, IHasWeapon source);
    }
}