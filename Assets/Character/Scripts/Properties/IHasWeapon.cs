using System.Collections.Generic;
using UnityEngine;

namespace Character.Scripts.Properties {
    public interface IHasWeapon {
        bool IsAttacking { get; set; }
        float AttackDamage { get; }
        float AttackSpeed { get; }
        float AttackCooldown { get; set; }
        float AttackRange { get; }
        float AttackAngle { get; }
        IEnumerable<Team> AttackableTeams { get; }
        void AttemptAttack(float damageMultiplier, AttackStrength attackStrength);
        void OnAttackSuccess(Base.Character target, float damageDealt);
        bool CanStartAttack { get; }
        void StartAttack(Vector2 direction);
        void EndAttack();
    }
}