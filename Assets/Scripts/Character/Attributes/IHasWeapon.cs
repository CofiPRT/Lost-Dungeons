using System.Collections.Generic;
using Scripts.Properties;
using UnityEngine;

namespace Scripts.Character.Attributes {
    public interface IHasWeapon {
        bool IsAttacking { get; set; }
        float AttackDamage { get; }
        float AttackSpeed { get; }
        float AttackCooldown { get; set; }
        float AttackRange { get; }
        float AttackAngle { get; }
        IEnumerable<Team> AttackableTeams { get; }
        void AttemptAttack(float damageMultiplier, AttackStrength attackStrength);
        void OnAttackSuccess(Implementation.Base.GenericCharacter target, float damageDealt);
        bool CanStartAttack { get; }
        void StartAttack(Vector2 direction);
        void EndAttack();
        void UpdateAttackCooldown();
    }
}