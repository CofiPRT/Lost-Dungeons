using CameraScript.Path.Bezier;
using UnityEngine;

namespace CameraScript.Path {
    public class PathAnchor : MonoBehaviour {
        [Range(30f, 120f)] public float fieldOfView = 45;

        [Range(-0.1f, 50f)] public float speed = 1;
        public bool rollCamera = true;

        public Ease speedEase = Ease.Sine;
        public Ease fovEase = Ease.Sine;
        public Ease rotEase = Ease.Sine;

        public bool activateAllyAI;

        public Vector3 Pos => transform.position;
        public Vector3 Rot => transform.eulerAngles;

        private BezierPath speedCurve;
        private BezierPath fovCurve;
        private BezierPath forwardCurve;
        private BezierPath upCurve;

        private void Awake() {
            speedCurve = GetPath(speedEase);
            fovCurve = GetPath(fovEase);
            forwardCurve = GetPath(rotEase);
        }

        private static BezierPath GetPath(Ease ease) {
            return new BezierPath(
                new[] {
                    new BezierCurve(
                        new[] {
                            new Vector3(0, 0, 0),
                            new Vector3((float)ease / 100f, 0, 0),
                            new Vector3(1 - (float)ease / 100f, 1, 0),
                            new Vector3(1, 1, 0)
                        }
                    )
                }
            );
        }

        public float LerpSpeed(float t, float targetSpeed) {
            return Mathf.Lerp(speed, targetSpeed, speedCurve.GetPoint(t).y);
        }

        public float LerpFov(float t, float targetFov) {
            return Mathf.Lerp(fieldOfView, targetFov, fovCurve.GetPoint(t).y);
        }

        public Vector3 LerpRot(float t, Vector3 targetRot) {
            // normal lerps can go into negative values, which spins the camera the wrong way
            // use special angle lerping which wraps around 360 degrees
            var newT = forwardCurve.GetPoint(t).y;
            return new Vector3(
                Mathf.LerpAngle(Rot.x, targetRot.x, newT),
                Mathf.LerpAngle(Rot.y, targetRot.y, newT),
                Mathf.LerpAngle(Rot.z, targetRot.z, newT)
            );
        }
    }
}