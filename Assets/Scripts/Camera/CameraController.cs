using System;
using Game;
using UnityEngine;

namespace Camera {
    public class CameraController : MonoBehaviour {
        private const double DistanceEqualsTolerance = 10e-3;

        // singleton
        public static CameraController Instance { get; private set; }

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        public static Vector3 Forward => Instance.transform.forward;
        public static Vector2 Forward2D => new Vector2(Forward.x, Forward.z).normalized;

        public static Vector3 Right => Instance.transform.right;
        public static Vector2 Right2D => new Vector2(Right.x, Right.z).normalized;

        [Range(0.1f, 20.0f)] public float smoothSpeed = 10f; // for lerping

        public float heightFromTarget;
        public float distanceFromTarget = 5.0f;

        [Range(0.1f, 10.0f)] public float horizontalSensitivity = 1.0f;
        [Range(0.1f, 10.0f)] public float verticalSensitivity = 1.0f;

        public bool canRotate = true;
        public bool invertY;

        [Range(-90.0f, 0.0f)] public float minYAngle = -45.0f;
        [Range(0.0f, 90.0f)] public float maxYAngle = 45.0f;

        // COLLISION - RELATED
        public LayerMask collisionLayer;
        [Range(1.0f, 5.0f)] public float fovDivisionFactor = 3.41f; // used in clip point computation

        private UnityEngine.Camera usedCamera;
        private Vector2 rotation;

        private void Start() {
            usedCamera = GetComponent<UnityEngine.Camera>();
        }

        public void ApplyInput(float horizontal, float vertical) {
            if (!canRotate) {
                PerformCollision();
                return;
            }

            rotation.x += horizontalSensitivity * horizontal;
            rotation.y += verticalSensitivity * vertical * (invertY ? 1 : -1);

            // keep in bounds
            rotation.x = Mathf.Repeat(rotation.x, 360);
            rotation.y = Mathf.Clamp(rotation.y, minYAngle, maxYAngle);

            var desiredRotation = Quaternion.Euler(rotation.y, rotation.x, 0);

            // smooth lerping
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);

            PerformCollision();
        }

        private void PerformCollision() {
            var target = GameController.ControlledPlayer;

            var targetPos = target.transform.position + target.GetComponent<Rigidbody>().centerOfMass;
            var desiredPosition = targetPos - transform.forward * distanceFromTarget;
            desiredPosition += new Vector3(0, heightFromTarget, 0);

            // compute offsets for every axis
            var z = usedCamera.nearClipPlane;
            var x = Mathf.Tan(usedCamera.fieldOfView / fovDivisionFactor) * z;
            var y = x / usedCamera.aspect;

            // relative to the camera's orientation
            var camRot = usedCamera.transform.rotation;

            var clipPoints = new[] {
                camRot * new Vector3(-x, y, z) + desiredPosition, // top left
                camRot * new Vector3(x, y, z) + desiredPosition, // top right
                camRot * new Vector3(-x, -y, z) + desiredPosition, // bottom left
                camRot * new Vector3(-x, y, z) + desiredPosition, // bottom right
                desiredPosition // we will be raycasting from all these points
            };

            // find the clip point that needs to be the closest the target
            var distance = distanceFromTarget;

            foreach (var clipPoint in clipPoints) {
                // perform a raycast from the target position to each of the clip points
                var direction = clipPoint - targetPos;

                if (!Physics.Raycast(targetPos, direction, out var hit, distanceFromTarget, collisionLayer))
                    continue;

                if (hit.distance < distance)
                    distance = hit.distance;
            }

            // update to the adjusted distance
            if (Math.Abs(distance - distanceFromTarget) > DistanceEqualsTolerance)
                desiredPosition += transform.forward * (distanceFromTarget - distance);

            // perform smooth lerping
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
    }
}