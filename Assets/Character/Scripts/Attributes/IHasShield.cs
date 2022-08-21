using Character.Scripts.Properties;
using UnityEngine;

namespace Character.Scripts.Attributes {
    public interface IHasShield {
        bool IsBlocking { get; set; }
        float ShieldAngle { get; }
        float ShieldRechargeTime { get; }
        float ShieldCooldown { get; set; }
        BlockStrength BlockStrength { get; }
        float AttemptBlock(float damage, AttackStrength attackStrength, Generic.Character source);
        bool CanStartBlocking { get; }
        void StartBlocking(Vector2 direction);
        void StopBlocking(bool forced = false);
    }
}