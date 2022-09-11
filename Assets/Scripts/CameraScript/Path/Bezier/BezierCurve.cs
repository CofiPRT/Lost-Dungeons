using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CameraScript.Path.Bezier {
    public class BezierCurve {
        private const int DefaultSteps = 10000;

        public readonly Vector3[] points;
        public readonly float length;

        public BezierCurve(Vector3[] points, int steps = DefaultSteps) {
            this.points = points;
            length = CalculateLength(steps);
        }

        public Vector3 GetPoint(float t) {
            return TransformPoint(points, t);
        }

        private static Vector3 TransformPoint(IReadOnlyList<Vector3> tPoints, float t) {
            if (tPoints.Count == 1)
                return tPoints[0];

            var point1 = TransformPoint(tPoints.Take(tPoints.Count - 1).ToArray(), t);
            var point2 = TransformPoint(tPoints.Skip(1).ToArray(), t);
            var newT = 1 - t;
            return new Vector3(
                newT * point1.x + t * point2.x,
                newT * point1.y + t * point2.y,
                newT * point1.z + t * point2.z
            );
        }

        private float CalculateLength(int steps) {
            var len = 0f;
            var lastPoint = GetPoint(0);

            for (var i = 1; i <= steps; i++) {
                var t = i / (float)steps;
                var point = GetPoint(t);

                len += Vector3.Distance(lastPoint, point);
                lastPoint = point;
            }

            return len;
        }
    }
}