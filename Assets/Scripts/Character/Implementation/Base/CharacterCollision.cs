using System.Collections.Generic;
using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        private Rigidbody RigidBody { get; set; }
        private Collider Collider { get; set; }
        public bool IgnoreCollisions { get; set; }

        public Vector3 CenterOfMass => RigidBody.worldCenterOfMass;

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
            CollisionLayers = LayerMask.GetMask(Properties.TeamUtils.AllLayers);
        }
    }
}