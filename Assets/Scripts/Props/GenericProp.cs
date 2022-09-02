using System.Collections.Generic;
using Character.Implementation.Base;
using Properties;
using UnityEngine;

namespace Props {
    public class GenericProp : MonoBehaviour {
        private const float Force = 100f;
        private const float CastTargetHeight = 2;

        private GenericCharacter propTarget;
        private GenericCharacter source;

        private int collisionLayerMask;
        private readonly List<Collider> ignoredColliders = new List<Collider>();

        private Collider propCollider;
        private Rigidbody propRigidbody;

        private Vector3 castStartPosition;
        private Vector3 originalScale;

        public Vector3 Pos => transform.position;
        public Vector2 Pos2D => new Vector2(transform.position.x, transform.position.z);

        public void LaunchTowards(GenericCharacter target, GenericCharacter caster) {
            ReenablePhysics();
            propTarget = target;
            source = caster;
            propRigidbody.useGravity = false;
        }

        private void Awake() {
            collisionLayerMask = LayerMask.GetMask(TeamUtils.AllLayers);
            propRigidbody = GetComponent<Rigidbody>();
            propCollider = GetComponent<Collider>();
            originalScale = transform.localScale;
        }

        private void FixedUpdate() {
            if (propTarget == null)
                return;

            var direction = (propTarget.CenterOfMass - Pos).normalized;

            // apply force towards the target
            propRigidbody.AddForce(direction * (Force * Time.fixedDeltaTime), ForceMode.VelocityChange);
        }

        private void OnCollisionEnter(Collision collision) {
            if (!propTarget)
                return;

            if (collisionLayerMask == (collisionLayerMask | (1 << collision.gameObject.layer))) {
                // check if the collider belongs to the target
                if (collision.gameObject == propTarget.gameObject) {
                    // apply a stun and restore the idle status of the prop
                    propTarget.AttemptStun(5f, source);
                    propTarget = null;
                    RestoreCollisions();
                    propRigidbody.useGravity = true;
                } else {
                    // ignore physics with any other object
                    Physics.IgnoreCollision(collision.collider, propCollider);
                    ignoredColliders.Add(collision.collider);
                }
            }
        }

        public void StartCast() {
            propRigidbody.isKinematic = true;
            castStartPosition = transform.position;
        }

        public void CastLift(float coefficient) {
            // lerp between the start position and the target height
            transform.position = Vector3.Lerp(
                castStartPosition,
                new Vector3(castStartPosition.x, castStartPosition.y + CastTargetHeight, castStartPosition.z),
                coefficient
            );
        }

        public void CastLerpSize(float coefficient) {
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, coefficient);
        }

        public void ReenablePhysics() {
            // remove the kinematic flag from the rigidbody
            propRigidbody.isKinematic = false;
        }

        public void RestoreCollisions() {
            foreach (var col in ignoredColliders)
                Physics.IgnoreCollision(propCollider, col, false);

            ignoredColliders.Clear();
        }
    }
}