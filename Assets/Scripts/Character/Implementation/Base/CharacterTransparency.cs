using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        private SkinnedMeshRenderer skinnedRenderer;
        public Material material;
        public Material transparentMaterial;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private void AwakeTransparency() {
            skinnedRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        public void StartTransparency() {
            // change material to transparent
            skinnedRenderer.material = transparentMaterial;

            // set alpha to full
            skinnedRenderer.material.SetColor(BaseColor, new Color(1, 1, 1, 1));
        }

        public void LerpTransparency(float coefficient) {
            // update color alpha
            skinnedRenderer.material.SetColor(BaseColor, new Color(1, 1, 1, coefficient));
        }

        public void StopTransparency() {
            // set material back to original
            skinnedRenderer.material = material;
        }
    }
}