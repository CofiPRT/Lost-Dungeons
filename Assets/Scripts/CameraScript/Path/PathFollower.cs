using System.Collections.Generic;
using CameraScript.Path.Bezier;
using Game;
using UnityEngine;

namespace CameraScript.Path {
    public class PathFollower : MonoBehaviour {
        public bool startOnAwake = true;

        private readonly List<PathAnchor> anchors = new List<PathAnchor>();
        private BezierPath path;
        private bool active;

        private void Awake() {
            // get all path anchors
            foreach (Transform child in transform)
                anchors.Add(child.GetComponent<PathAnchor>());
        }

        // we need to do this in Start, as transform.position is not set in Awake
        private void Start() {
            // compute anchors for bezier curves
            var bezierAnchors = new List<Vector3> {
                anchors[0].Pos // start from the first point
            };

            for (var index = 0; index < anchors.Count - 1; index++) {
                var startAnchor = anchors[index];
                var endAnchor = anchors[index + 1];

                if (index >= anchors.Count - 2) {
                    // last anchor, just add it
                    bezierAnchors.Add(endAnchor.Pos);
                    continue;
                }

                // for intermediary anchors, create additional control points
                var nextAnchor = anchors[index + 2];

                var direction1 = (startAnchor.Pos - endAnchor.Pos).normalized;
                var direction2 = (nextAnchor.Pos - endAnchor.Pos).normalized;

                var bisector = (direction1 + direction2).normalized;
                var cross = Vector3.Cross(direction1, direction2);
                var angle = Vector3.Angle(direction1, direction2) * Mathf.Deg2Rad;
                var cosAngle = (Mathf.PI - angle) / 2;

                var len1 = Vector3.Distance(startAnchor.Pos, endAnchor.Pos);
                var len2 = Vector3.Distance(endAnchor.Pos, nextAnchor.Pos);

                // construct control point
                var controlPoint1 = endAnchor.Pos +
                                    Quaternion.AngleAxis(-90, cross) * bisector *
                                    Mathf.Cos(cosAngle) * len1 * 0.5f;
                var controlPoint2 = endAnchor.Pos +
                                    Quaternion.AngleAxis(90, cross) * bisector *
                                    Mathf.Cos(cosAngle) * len2 * 0.5f;

                bezierAnchors.Add(controlPoint1);
                bezierAnchors.Add(endAnchor.Pos);
                bezierAnchors.Add(controlPoint2);
            }

            // compute bezier curves out of the anchors
            var curves = new List<BezierCurve>();
            var anchorIndex = 0;
            while (anchorIndex < bezierAnchors.Count) {
                var anchorCount = anchorIndex == 0 ? 3 : 4;
                var curve = new BezierCurve(
                    bezierAnchors.GetRange(
                            anchorIndex,
                            Mathf.Min(anchorCount, bezierAnchors.Count - anchorIndex)
                        )
                        .ToArray()
                );
                curves.Add(curve);
                anchorIndex += anchorCount - 1;
            }

            // compute a bezier path out of the curves
            path = new BezierPath(curves.ToArray());

            // set anchors to inactive
            foreach (var anchor in anchors)
                anchor.gameObject.SetActive(false);

            if (startOnAwake)
                Activate();
        }

        public void Activate(bool reset = true) {
            active = true;

            if (reset || progress >= 1)
                progress = 0;
        }

        public void Stop() {
            active = false;
        }

        private float progress; // 0..1
        private bool allyAIActivated;

        private void Update() {
            if (!active) return;

            var cumulativeLength = path.GetCumulativeLength(progress);
            var curveIndex = cumulativeLength.curveIndex;
            var curveProgress = cumulativeLength.curveT;

            // interpolate speed, directions, and fov
            var speed = anchors[curveIndex].LerpSpeed(curveProgress, anchors[curveIndex + 1].speed);
            var rotation = anchors[curveIndex].LerpRot(curveProgress, anchors[curveIndex + 1].Rot);
            var fov = anchors[curveIndex].LerpFov(curveProgress, anchors[curveIndex + 1].fieldOfView);

            // move the camera
            CameraController.SetCustomTarget(
                cumulativeLength.point,
                rotation,
                fov,
                false,
                false
            );

            // update progress
            progress += speed * Time.deltaTime * GameController.GameTickSpeed / path.length;

            // check if we reached the end
            if (curveIndex == anchors.Count - 2 && curveProgress >= 0.9f) {
                Stop();
                FinishEvents?.Invoke();
                CameraController.SetFollowPlayer();
            }

            // activate AI if possible
            if (!allyAIActivated && anchors[curveIndex].activateAllyAI) {
                allyAIActivated = true;

                foreach (var ally in GameController.AliveAllies)
                    ally.ActivateAI();
            }
        }

        public delegate void FinishEventHandler();

        public event FinishEventHandler FinishEvents;

        private void OnDrawGizmos() {
            if (!Application.isPlaying) return;

            var color = Color.red;

            var steps = 10000;
            for (var i = 0; i < steps; i++) {
                var t = i / (float)steps;
                var t2 = (i + 1) / (float)steps;

                var p1 = path.GetPoint(t);
                var p2 = path.GetPoint(t2);

                Gizmos.DrawLine(p1, p2);

                // toggle color
                color = color == Color.red ? Color.blue : Color.red;
                Gizmos.color = color;
            }

            // draw control points
            Gizmos.color = Color.green;

            foreach (var curve in path.curves)
            foreach (var anchor in curve.points)
                Gizmos.DrawSphere(anchor, 0.1f);
        }
    }
}