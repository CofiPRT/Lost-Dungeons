using UnityEngine;

namespace Character.Implementation.Base {
    public partial class GenericCharacter {
        private const float Volume = 0.5f;

        protected AudioClip hurtSound;
        protected AudioClip deathSound;

        protected AudioClip stepSound;

        private AudioClip blockSound;
        private AudioClip blockBreakSound;

        private AudioClip attackSound;

        private void AwakeAudio() {
            const string rootPath = "Audio/Character/Shared/";

            blockSound = Resources.Load<AudioClip>(rootPath + "character_block");
            blockBreakSound = Resources.Load<AudioClip>(rootPath + "character_block_break");
            attackSound = Resources.Load<AudioClip>(rootPath + "character_attack");
        }

        internal void PlaySound(AudioClip clip) {
            AudioSource.PlayClipAtPoint(clip, Pos, Volume);
        }
    }
}