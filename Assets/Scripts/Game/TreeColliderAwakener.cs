using UnityEngine;

namespace Game {
    public class TreeColliderAwakener : MonoBehaviour {
        private void Awake() {
            GetComponent<TerrainCollider>().enabled = false;
            GetComponent<TerrainCollider>().enabled = true;
        }
    }
}