using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CameraScript.Path.Bezier {
    public class BezierPath {
        private const float DefaultInterpolationStep = 0.001f;

        public readonly BezierCurve[] curves;
        public readonly float length;

        private readonly CumulativeLength[] cumulativeLengths;

        public BezierPath(BezierCurve[] curves, float interpolationStep = DefaultInterpolationStep) {
            this.curves = curves;
            length = curves.Sum(c => c.length);
            cumulativeLengths = ComputeCumulativeLengths(interpolationStep);
        }

        private CumulativeLength[] ComputeCumulativeLengths(float interpolationStep) {
            var cumulativeLens = new List<CumulativeLength>();
            var len = -1f;
            var prev = Vector3.zero;

            for (var curveIndex = 0; curveIndex < curves.Length; curveIndex++) {
                var curve = curves[curveIndex];
                var steps = Mathf.Max(3, Mathf.CeilToInt(curve.length / interpolationStep));

                for (var i = 0; i <= steps; i++) {
                    var t = (float)i / steps;
                    var point = curve.GetPoint(t);

                    if (len < 0) // the first len will always be 0
                        len = 0;
                    else // afterwards, start adding
                        len += Vector3.Distance(prev, point);

                    cumulativeLens.Add(new CumulativeLength(len, point, curveIndex, t));
                    prev = point;
                }
            }

            return cumulativeLens.ToArray();
        }

        public CumulativeLength GetCumulativeLength(float t) {
            var len = t * length;
            var index = Array.BinarySearch(cumulativeLengths, new CumulativeLength(len));

            // negative result is the complement of the index of the first element that is larger than the value
            if (index < 0)
                index = ~index;

            if (index >= cumulativeLengths.Length)
                index = cumulativeLengths.Length - 1;

            return cumulativeLengths[index];
        }

        public Vector3 GetPoint(float t) {
            return GetCumulativeLength(t).point;
        }
    }

    public class CumulativeLength : IComparable<CumulativeLength> {
        public readonly float length;
        public readonly Vector3 point;

        public readonly int curveIndex;
        public readonly float curveT;

        public CumulativeLength(float length) {
            this.length = length;
        }

        public CumulativeLength(float length, Vector3 point, int curveIndex, float curveT) {
            this.length = length;
            this.point = point;
            this.curveIndex = curveIndex;
            this.curveT = curveT;
        }

        public int CompareTo(CumulativeLength other) {
            return length.CompareTo(other.length);
        }
    }
}