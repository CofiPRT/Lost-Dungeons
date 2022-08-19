using Character.Scripts;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game {
    public class GameController : MonoBehaviour {
        // singleton
        public static GameController Instance { get; private set; }

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        // game data
        public Transform player1;
        public Transform player2;

        public float gameTickSpeed = 1.0f;
        public float playerTickFactor = 1.0f;

        public float changePlayerCooldown = 5.0f;
        private float changePlayerCooldownCurrent;

        private Action action;

        private void Start() {
            player1 = Instantiate(player1, new Vector3(0, 1, 0), Quaternion.identity);
            player2 = Instantiate(player2, new Vector3(1, 0, 0), Quaternion.identity);

            player1.GetComponent<PlayerController>().ally = player2;
            player2.GetComponent<PlayerController>().ally = player1;

            UpdatePlayerControl();
        }

        private void Update() {
            if (action != null)
                action.Update();
            else
                UpdateNormal();
        }

        private void UpdateNormal() {
            if (changePlayerCooldownCurrent == 0 && InputController.ChangePlayerPressed())
                StartAction(new PlayerChangeAction());

            if (changePlayerCooldownCurrent > 0)
                changePlayerCooldownCurrent = Mathf.Max(
                    changePlayerCooldownCurrent - Time.deltaTime * gameTickSpeed,
                    0
                );
        }

        private void StartAction(Action newAction, bool endGracefully = true) {
            if (action != null && endGracefully)
                action.End();

            action = newAction;
            action.Start();
        }

        private void UpdatePlayerControl(bool p1 = false, bool p2 = true) {
            player1.GetComponent<PlayerController>().automated = p1;
            player2.GetComponent<PlayerController>().automated = p2;
        }

        private void AllowInputs(bool value) {
            player1.GetComponent<PlayerController>().updateInputs = value;
            player2.GetComponent<PlayerController>().updateInputs = value;
        }

        public float GetPlayerTickSpeed() {
            return gameTickSpeed * playerTickFactor;
        }

        private abstract class Action {
            protected GameController gc = Instance;
            protected float elapsedTime;

            protected abstract float GetLength();
            protected abstract void UpdateInternal();

            public virtual void Start() { }
            public virtual void End() { }

            public void Update() {
                UpdateInternal();

                if (elapsedTime >= GetLength()) {
                    End();
                    gc.action = null;
                }

                elapsedTime += Time.deltaTime;
            }
        }

        private class PlayerChangeAction : Action {
            private bool changed;

            private FilmGrain filmGrain;
            private Vignette vignette;
            private LensDistortion lensDistortion;

            public override void Start() {
                if (UnityEngine.Camera.main != null) {
                    var profile = UnityEngine.Camera.main.GetComponent<Volume>().profile;
                    profile.TryGet(out filmGrain);
                    profile.TryGet(out vignette);
                    profile.TryGet(out lensDistortion);
                }

                // prepare the camera effects of this profile
                filmGrain.active = true;
                vignette.active = true;
                lensDistortion.active = true;

                filmGrain.intensity.value = 0f;
                vignette.intensity.value = 0f;
                lensDistortion.intensity.value = 0f;

                // start slow motion
                gc.gameTickSpeed = 0.1f;

                // block player inputs
                gc.AllowInputs(false);
            }

            public override void End() {
                // update player control and inputs
                gc.UpdatePlayerControl();
                gc.AllowInputs(true);
                gc.changePlayerCooldownCurrent = gc.changePlayerCooldown;

                // disable the camera effects of this profile
                filmGrain.active = false;
                vignette.active = false;
                lensDistortion.active = false;

                gc.gameTickSpeed = 1.0f;
            }

            protected override float GetLength() {
                return 1.5f;
            }

            protected override void UpdateInternal() {
                var length = GetLength();
                var lerpLength = length / 6;

                if (elapsedTime < length / 2) {
                    // smooth start the camera effects
                    var param = elapsedTime / lerpLength;

                    filmGrain.intensity.value = Mathf.Lerp(0f, 1f, param);
                    vignette.intensity.value = Mathf.Lerp(0f, 0.25f, param);
                    lensDistortion.intensity.value = Mathf.Lerp(0f, -0.5f, param);
                } else {
                    // smooth end the camera effects
                    var param = (elapsedTime - (GetLength() - lerpLength)) / lerpLength;

                    filmGrain.intensity.value = Mathf.Lerp(1f, 0f, param);
                    vignette.intensity.value = Mathf.Lerp(0.25f, 0f, param);
                    lensDistortion.intensity.value = Mathf.Lerp(-0.5f, 0f, param);

                    // if not changed yet, interchange the players
                    if (!changed) {
                        (gc.player1, gc.player2) = (gc.player2, gc.player1);

                        changed = true;
                    }
                }
            }
        }
    }
}