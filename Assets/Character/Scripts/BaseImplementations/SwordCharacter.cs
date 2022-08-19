using System.Linq;
using Character.Scripts.Properties;
using UnityEngine;

namespace Character.Scripts.BaseImplementations {
    public class SwordCharacter : Character, IHasWeapon {
        protected const float DefaultAttackDamage = 10;
        protected const float DefaultAttackSpeed = 1; // in attacks per second
        protected const float DefaultAttackRange = 1;
        protected const float DefaultAttackAngle = Mathf.PI / 4; // in radians

        public SwordCharacter(
            TeamMembership teamMembership,
            float maxHealth = DefaultMaxHealth,
            float attackDamage = DefaultAttackDamage,
            float attackSpeed = DefaultAttackSpeed,
            float attackRange = DefaultAttackRange,
            float attackAngle = DefaultAttackAngle
        ) : base(teamMembership, maxHealth) {
            AttackDamage = attackDamage;
            AttackSpeed = attackSpeed;
            AttackRange = attackRange;
            AttackAngle = attackAngle;
        }

        /* IHasWeapon */

        public bool IsAttacking { get; set; }
        public float AttackDamage { get; }
        public float AttackSpeed { get; }
        public float AttackCooldown { get; set; }
        public float AttackRange { get; }
        public float AttackAngle { get; }

        public void AttemptAttack(float damageMultiplier, AttackStrength attackStrength) {
            if (!IsAttacking) return;

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

                float damageDealt = 0;

                // if the target has a shield, delegate the attack to it
                if (closeObject is IHasShield shield)
                    damageDealt = shield.AttemptBlock(damage, attackStrength, this);
                else
                    damageDealt = closeObject.TakeDamage(damage);

                // notify self
                if (damageDealt > 0)
                    OnAttackSuccess(closeObject, damageDealt);
            }
        }

        public void OnAttackSuccess(Character target, float damageDealt) {
            // intentionally left blank
        }

        public void StartAttack() {
            if (IsAttacking || IsStunned || AttackCooldown > 0) return;

            IsAttacking = true;
            AttackCooldown = 1 / AttackSpeed;
        }

        public void EndAttack() {
            if (!IsAttacking) return;

            IsAttacking = false;
        }

        /* Parent */

        public override bool AttemptStun(float stunDuration, Character source) {
            if (!base.AttemptStun(stunDuration, source))
                return false;

            EndAttack();
            return true;
        }

        /* Unity */

        protected override void Update() {
            base.Update();

            AttackCooldown = Mathf.Max(0, AttackCooldown - Time.deltaTime);
        }
    }
}