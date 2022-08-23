using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Camera {
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
            profile.TryGet(out motionBlur);
        }

        private FilmGrain filmGrain;
        private Vignette vignette;
        private LensDistortion lensDistortion;
        private Bloom bloom;
        private MotionBlur motionBlur;

        public static FilmGrain FilmGrain => Instance.filmGrain;
        public static Vignette Vignette => Instance.vignette;
        public static LensDistortion LensDistortion => Instance.lensDistortion;
        public static Bloom Bloom => Instance.bloom;
        public static MotionBlur MotionBlur => Instance.motionBlur;
    }
}