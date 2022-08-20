using UnityEngine;

namespace Character.Scripts.Properties {
    public interface IHasShield {
        bool IsBlocking { get; set; }
        float ShieldAngle { get; }
        float ShieldRechargeTime { get; }
        float ShieldCooldown { get; set; }
        BlockStrength BlockStrength { get; }
        float AttemptBlock(float damage, AttackStrength attackStrength, Base.Character source);
        bool CanStartBlocking { get; }
        void StartBlocking(Vector2 direction);
        void StopBlocking(bool forced = false);
    }
}