using System.Linq;
using Character.Scripts.Properties;
using UnityEngine;

namespace Character.Scripts.BaseImplementations {
    public class SwordCharacter : Character, IHasWeapon {
        private const float DefaultAttackDamage = 10;
        private const float DefaultAttackSpeed = 1; // in attacks per second
        private const float DefaultAttackRange = 1;
        private const float DefaultAttackAngle = Mathf.PI / 4; // in radians

        public SwordCharacter(TeamMembership teamMembership, float maxHealth = DefaultMaxHealth)
            : base(teamMembership, maxHealth) {
            AttackDamage = DefaultAttackDamage;
            AttackSpeed = DefaultAttackSpeed;
            AttackCooldown = 0;
            AttackRange = DefaultAttackRange;
            AttackAngle = DefaultAttackAngle;
        }

        public bool IsAttacking { get; set; }
        public float AttackDamage { get; }
        public float AttackSpeed { get; }
        public float AttackCooldown { get; set; }
        public float AttackRange { get; }
        public float AttackAngle { get; }

        public void AttemptAttack(float damageMultiplier, AttackStrength attackStrength) {
            if (!IsAttacking || IsStunned) return;

            // find the direction of the attack
            var direction = transform.forward;

            // make the direction parallel to the ground - remove the y component
            direction.y = 0;
            direction.Normalize();


            // check for attack-able objects in the attack range - first obtain a box overlap
            var currPosition = transform.position;

            // ReSharper disable once Unity.PreferNonAllocApi - we want every object in the range to be checked
            var closeObjects = Physics.OverlapBox(
                    currPosition,
                    new Vector3(AttackRange, AttackRange, AttackRange),
                    transform.rotation,
                    LayerMask.GetMask(TeamUtils.Opposite(TeamUtils.ToLayer(TeamMembership)))
                )
                .Select(x => x.gameObject.GetComponent<Character>())
                .ToList();

            // if no objects were hit, abort early
            if (closeObjects.Count == 0) return;

            var damage = (int)(AttackDamage * damageMultiplier);

            // the height difference is already accounted for, only check if the target is within the circle sector
            foreach (var closeObject in closeObjects) {
                // disregard the y position
                var closeObjectPosition = closeObject.transform.position;
                closeObjectPosition.y = currPosition.y;

                // the distance must be within range
                var distance = (closeObjectPosition - currPosition).magnitude;
                if (distance > AttackRange) continue;

                // the angle must be within the sector
                var angle = Vector3.Angle(direction, closeObjectPosition - currPosition);
                if (angle > AttackAngle) continue;

                // if the target has a shield, test whether they block this attack
                if (closeObject is IHasShield shield) {
                    shield.AttemptBlock(damage, attackStrength, this);
                } else {
                    var damageTaken = closeObject.TakeDamage(damage);

                    // notify self
                    if (damageTaken > 0)
                        OnAttackSuccess(closeObject, damageTaken);
                }
            }
        }

        public void OnAttackSuccess(Character target, float damageDealt) {
            // intentionally left blank
        }

        public void StartAttack() {
            if (IsAttacking) return;

            IsAttacking = true;
        }

        public void EndAttack() {
            if (!IsAttacking) return;

            IsAttacking = false;
        }
    }
}