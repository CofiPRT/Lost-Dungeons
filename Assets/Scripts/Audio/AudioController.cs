using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Audio {
    public class AudioController : MonoBehaviour {
        public List<AudioClip> ambienceClips;
        public AudioClip battleClip;

        public List<float> battleClipSeekTimes = new List<float> { 0 };
        public float earlyEndSeekTime;

        [Range(0f, 0.05f)] public float ambienceVolume = 0.01f;
        [Range(0f, 0.05f)] public float battleVolume = 0.01f;

        public float ambienceFadeOutTime = 3;
        public float battleFadeOutTime = 5;
        public float battleTimeout = 5;

        public bool forceBattle;

        private AudioSource ambienceSource;
        private AudioSource battleSource;

        private void Awake() {
            ambienceSource = gameObject.AddComponent<AudioSource>();
            ambienceSource.playOnAwake = false;
            ambienceSource.volume = ambienceVolume;

            battleSource = gameObject.AddComponent<AudioSource>();
            battleSource.playOnAwake = false;
            battleSource.volume = battleVolume;
            battleSource.loop = true;
        }

        private void Start() {
            // choose a song to play
            if (forceBattle)
                PlayBattle(true);
            else
                PlayAmbience();
        }

        private float timeSinceLastFight;
        private bool playingBattle;

        // only one of each may exist at any given moment
        private Coroutine ambienceCoroutine;
        private Coroutine battleCoroutine;

        private void Update() {
            // restart battle music if it's been too long
            if (earlyEndSeekTime > 0 && playingBattle && battleSource.time >= earlyEndSeekTime)
                battleSource.time = battleClipSeekTimes[0];

            if (forceBattle)
                return; // keep playing battle music

            var inFight = GameController.ControlledPlayer.FairFight.InFight;

            if (inFight)
                timeSinceLastFight = 0;
            else
                timeSinceLastFight += Time.deltaTime;

            if (!playingBattle && inFight) {
                playingBattle = true;

                PlayBattle();

                StopAllCoroutines();
                StartCoroutine(FadeIn(battleSource, battleVolume, ambienceFadeOutTime));
                StartCoroutine(FadeOut(ambienceSource, ambienceFadeOutTime));
            } else if (playingBattle && !inFight && timeSinceLastFight > battleTimeout) {
                playingBattle = false;

                StopAllCoroutines();
                StartCoroutine(FadeOut(battleSource, battleFadeOutTime, true));
            } else if (!playingBattle && !inFight) {
                // if the song ended, play one more
                if (ambienceSource.time >= ambienceSource.clip.length)
                    PlayAmbience();
            }
        }

        private void PlayAmbience() {
            ambienceSource.clip = ambienceClips[Random.Range(0, ambienceClips.Count)];
            ambienceSource.Play();
        }

        private void PlayBattle(bool start = false) {
            battleSource.clip = battleClip;
            battleSource.time = battleClipSeekTimes[start ? 0 : Random.Range(0, battleClipSeekTimes.Count)];
            battleSource.Play();

            Debug.Log("Playing from " + battleSource.time);
        }

        private IEnumerator FadeIn(AudioSource source, float targetVolume, float time) {
            var startVolume = source.volume;
            var startTime = Time.time;
            var endTime = startTime + time;

            while (Time.time < endTime) {
                source.volume = Mathf.Lerp(startVolume, targetVolume, (Time.time - startTime) / time);
                yield return null;
            }

            source.volume = targetVolume;
        }

        private IEnumerator FadeOut(AudioSource source, float time, bool playAmbience = false) {
            var startVolume = source.volume;
            var startTime = Time.time;
            var endTime = startTime + time;

            while (Time.time < endTime) {
                source.volume = Mathf.Lerp(startVolume, 0, (Time.time - startTime) / time);
                yield return null;
            }

            source.volume = 0;
            source.Stop();

            if (playAmbience) {
                PlayAmbience();
                StartCoroutine(FadeIn(ambienceSource, ambienceVolume, ambienceFadeOutTime));
            }
        }
    }
}