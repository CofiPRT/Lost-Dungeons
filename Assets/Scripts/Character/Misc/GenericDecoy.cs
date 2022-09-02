using Game;
using UnityEngine;

namespace Character.Misc {
    public class GenericDecoy : MonoBehaviour {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private Animator animator;
        private SkinnedMeshRenderer skinnedRenderer;

        private void Awake() {
            animator = GetComponent<Animator>();
            skinnedRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        private void Update() {
            animator.SetFloat(
                AnimatorHash.AnimationTickSpeed,
                GameController.GameTickSpeed * GameController.PlayerTickFactor
            );
        }

        public void LerpTransparency(float coefficient) {
            // update color alpha
            skinnedRenderer.material.SetColor(BaseColor, new Color(1, 1, 1, coefficient));
        }
    }
}