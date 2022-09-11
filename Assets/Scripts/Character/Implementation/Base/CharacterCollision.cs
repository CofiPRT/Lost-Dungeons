using System.Collections.Generic;
using Properties;
using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        private protected Rigidbody RigidBody { get; set; }
        private Collider Collider { get; set; }
        public bool IgnoreCollisions { get; set; }

        public Vector3 UpdatedCenterOfMass => Pos + RigidBody.centerOfMass;
        public Vector3 CenterOfMass => RigidBody.worldCenterOfMass;
        private Vector3 ColliderYTop => Collider.bounds.center + Vector3.up * Collider.bounds.extents.y;
        public Vector3 EyePosition => Vector3.Lerp(CenterOfMass, ColliderYTop, 0.8f);
        public Vector3 RelativeEyePosition => transform.InverseTransformPoint(EyePosition);

        private int CollisionLayers { get; set; }
        private readonly List<Collider> ignoredColliders = new List<Collider>();

        private void OnCollisionEnter(Collision collision) {
            if (!IgnoreCollisions || CollisionLayers != (CollisionLayers | (1 << collision.gameObject.layer))) return;

            Physics.IgnoreCollision(Collider, collision.collider);
            ignoredColliders.Add(collision.collider);
        }

        public void RestoreCollisions() {
            foreach (var col in ignoredColliders)
                Physics.IgnoreCollision(Collider, col, false);

            ignoredColliders.Clear();
        }

        private void AwakeCollider() {
            RigidBody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();
            CollisionLayers = LayerMask.GetMask(TeamUtils.AllLayers);
        }

        public bool CanSee(Vector3 position, out RaycastHit hit, bool addEyeHeight = true) {
            if (addEyeHeight)
                position += RelativeEyePosition;

            // raycast from the enemy to the fighting opponent, testing for terrain and barriers
            var ownPos = EyePosition;
            var direction = position - ownPos;
            var distance = direction.magnitude;
            var ray = new Ray(ownPos, direction);

            // if the raycast hit something, the enemy's vision is blocked
            return !Physics.Raycast(ray, out hit, distance, LayerMask.GetMask("Terrain", "Barrier"));
        }

        public bool CanSee(Vector3 position, bool addEyeHeight = true) {
            if (addEyeHeight)
                position += RelativeEyePosition;

            // raycast from the enemy to the fighting opponent, testing for terrain and barriers
            var ownPos = EyePosition;
            var direction = position - ownPos;
            var distance = direction.magnitude;
            var ray = new Ray(ownPos, direction);

            // if the raycast hit something, the enemy's vision is blocked
            return !Physics.Raycast(ray, out _, distance, LayerMask.GetMask("Terrain", "Barrier"));
        }

        public bool CanSee(GenericCharacter opponent) {
            return CanSee(opponent.EyePosition, false);
        }
    }
}