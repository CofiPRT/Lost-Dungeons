using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Audio {
    public class AudioController : MonoBehaviour {
        private List<AudioClipData> ambienceClips;
        private AudioClipData battleClip;

        public List<float> battleClipSeekTimes = new List<float> { 0 };

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

            battleSource = gameObject.AddComponent<AudioSource>();
            battleSource.playOnAwake = false;
            battleSource.loop = true;

            // acquire ambience clips
            ambienceClips = new List<AudioClipData>();
            foreach (var clipData in transform.Find("Ambience").GetComponentsInChildren<AudioClipData>())
                ambienceClips.Add(clipData);

            // acquire battle clip
            battleClip = transform.Find("Battle").GetComponent<AudioClipData>();
        }

        private void Start() {
            // choose a song to play
            if (forceBattle) {
                PlayBattle(true);
                ambienceSource.volume = 0;
                battleSource.volume = battleVolume;
            } else {
                PlayAmbience();
                ambienceSource.volume = ambienceVolume;
                battleSource.volume = 0;
            }
        }

        private float earlyEndTime;
        private float timeSinceLastFight;
        private bool playingBattle;

        // only one of each may exist at any given moment
        private Coroutine ambienceCoroutine;
        private Coroutine battleCoroutine;

        private void Update() {
            // restart battle music if it's been too long
            if (earlyEndTime > 0 && playingBattle && battleSource.time >= earlyEndTime)
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
                if (ambienceSource.time >= earlyEndTime)
                    PlayAmbience();
            }
        }

        private void PlayAmbience() {
            var clipData = ambienceClips[Random.Range(0, ambienceClips.Count)];
            ambienceSource.clip = clipData.audioClip;
            ambienceSource.Play();
            earlyEndTime = clipData.earlyEndTime;
        }

        private void PlayBattle(bool start = false) {
            battleSource.clip = battleClip.audioClip;

            // only restart if the volume is 0
            if (battleSource.volume == 0 || start) {
                battleSource.time = battleClipSeekTimes[start ? 0 : Random.Range(0, battleClipSeekTimes.Count)];
                battleSource.Play();
            }

            earlyEndTime = battleClip.earlyEndTime;
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