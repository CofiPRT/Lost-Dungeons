using Scripts.Properties;
using UnityEngine;

namespace Scripts.Character.Attributes {
    public interface IHasShield {
        bool IsBlocking { get; set; }
        float ShieldAngle { get; }
        float ShieldRechargeTime { get; }
        float ShieldCooldown { get; set; }
        BlockStrength BlockStrength { get; }
        float AttemptBlock(float damage, AttackStrength attackStrength, Implementation.Base.GenericCharacter source);
        bool CanStartBlocking { get; }
        void StartBlocking(Vector2 direction);
        void StopBlocking(bool forced = false);
        void UpdateShieldCooldown();
    }
}