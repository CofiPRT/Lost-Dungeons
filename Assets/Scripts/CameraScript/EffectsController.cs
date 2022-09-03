using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CameraScript {
    public class EffectsController : MonoBehaviour {
        // singleton
        private static EffectsController Instance { get; set; }

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;

            var profile = GetComponent<Volume>().profile;
            profile.TryGet(out filmGrain);
            profile.TryGet(out vignette);
            profile.TryGet(out lensDistortion);
            profile.TryGet(out bloom);
        }

        private FilmGrain filmGrain;
        private Vignette vignette;
        private LensDistortion lensDistortion;
        private Bloom bloom;

        /* lerp data */

        private Color vignetteStartColor;
        private Color vignetteEndColor;
        private float vignetteIntensity;
        private float filmGrainIntensity;
        private float lensDistortionIntensity;
        private float bloomIntensity;

        public static void Prepare(
            Color? vignetteStartColor = null,
            Color? vignetteEndColor = null,
            float vignetteIntensity = 0.4f,
            float filmGrainIntensity = 1.0f,
            float lensDistortionIntensity = -0.5f,
            float bloomIntensity = 1.0f
        ) {
            Instance.vignetteStartColor = vignetteStartColor ?? Color.black;
            Instance.vignetteEndColor = vignetteEndColor ?? Instance.vignetteStartColor;
            Instance.vignetteIntensity = vignetteIntensity;
            Instance.filmGrainIntensity = filmGrainIntensity;
            Instance.lensDistortionIntensity = lensDistortionIntensity;
            Instance.bloomIntensity = bloomIntensity;

            Instance.vignette.color.value = Instance.vignetteStartColor;
            Instance.vignette.active = true;
            Instance.filmGrain.active = true;
            Instance.lensDistortion.active = true;
            Instance.bloom.active = true;
        }

        public static void Lerp(float coefficient, bool lerpIntensity = true, bool lerpColor = true) {
            if (lerpColor)
                Instance.vignette.color.value = Color.Lerp(
                    Instance.vignetteStartColor,
                    Instance.vignetteEndColor,
                    coefficient
                );

            if (lerpIntensity) {
                Instance.vignette.intensity.value = Mathf.Lerp(0, Instance.vignetteIntensity, coefficient);
                Instance.filmGrain.intensity.value = Mathf.Lerp(0, Instance.filmGrainIntensity, coefficient);
                Instance.lensDistortion.intensity.value = Mathf.Lerp(0, Instance.lensDistortionIntensity, coefficient);
                Instance.bloom.intensity.value = Mathf.Lerp(0, Instance.bloomIntensity, coefficient);
            }
        }

        public static void ResetEffects() {
            Instance.vignette.active = false;
            Instance.filmGrain.active = false;
            Instance.lensDistortion.active = false;
            Instance.bloom.active = false;
        }
    }
}