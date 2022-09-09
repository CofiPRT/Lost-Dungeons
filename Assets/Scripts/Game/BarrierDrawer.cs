using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class BarrierDrawer : MonoBehaviour {
        public float lowerYLimit = -10f;
        public float upperYLimit = 10f;
        public bool keepMeshesVisible;

        private void Awake() {
            var yLevel = (lowerYLimit + upperYLimit) / 2f;
            var ySize = upperYLimit - lowerYLimit;

            // get barrier groups in hierarchy
            foreach (Transform group in transform) {
                // this is a barrier group, save its anchors
                var anchors = new List<Transform>();

                foreach (Transform child in group.transform) {
                    anchors.Add(child);

                    // place child at y level
                    var position = child.position;
                    child.position = new Vector3(position.x, yLevel, position.z);
                }

                for (var index = 0; index < anchors.Count; index++) {
                    var currAnchor = anchors[index];
                    var nextAnchor = anchors[(index + 1) % anchors.Count];

                    var currTransform = currAnchor.transform;
                    var nextTransform = nextAnchor.transform;

                    var currPosition = currTransform.position;
                    var nextPosition = nextTransform.position;

                    // get the distance between the two anchors
                    var distance = Vector3.Distance(currPosition, nextPosition);

                    // create a cube between the two anchors
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = (currPosition + nextPosition) / 2f;
                    cube.transform.localScale = new Vector3(1f, ySize, distance);

                    // set cube rotation
                    cube.transform.LookAt(currTransform);

                    // set the parent to this game object
                    cube.transform.parent = transform;

                    if (!keepMeshesVisible)
                        cube.GetComponent<MeshRenderer>().enabled = false;
                }

                // destroy the anchors
                foreach (var anchor in anchors)
                    Destroy(anchor.gameObject);
            }
        }
    }
}