namespace Character.Scripts.Properties {
    public interface IHasWeapon {
        bool IsAttacking { get; set; }
        float AttackDamage { get; }
        float AttackSpeed { get; }
        float AttackCooldown { get; set; }
        float AttackRange { get; }
        float AttackAngle { get; }
        void AttemptAttack(float damageMultiplier, AttackStrength attackStrength);
        void OnAttackSuccess(BaseImplementations.Character target, float damageDealt);
        void StartAttack();
        void EndAttack();
    }
}