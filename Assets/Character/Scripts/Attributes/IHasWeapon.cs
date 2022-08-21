using System.Collections.Generic;
using Character.Scripts.Properties;
using UnityEngine;

namespace Character.Scripts.Attributes {
    public interface IHasWeapon {
        bool IsAttacking { get; set; }
        float AttackDamage { get; }
        float AttackSpeed { get; }
        float AttackCooldown { get; set; }
        float AttackRange { get; }
        float AttackAngle { get; }
        IEnumerable<Team> AttackableTeams { get; }
        void AttemptAttack(float damageMultiplier, AttackStrength attackStrength);
        void OnAttackSuccess(Generic.Character target, float damageDealt);
        bool CanStartAttack { get; }
        void StartAttack(Vector2 direction);
        void EndAttack();
    }
}