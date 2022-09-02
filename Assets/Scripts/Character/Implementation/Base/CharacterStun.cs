using Character.Misc;
using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        private float StunDuration { get; set; }
        private bool IsStunned => StunDuration > 0;

        public virtual bool AttemptStun(float stunDuration, GenericCharacter source) {
            if (!IsAlive || stunDuration <= 0) return false;

            StunDuration = stunDuration;
            EndAttack();
            StopMoving();
            StopBlocking(true);
            Animator.SetBool(AnimatorHash.Stunned, true);

            return true;
        }

        private void UpdateStunDuration() {
            if (!IsAlive)
                return;

            StunDuration = Mathf.Max(0, StunDuration - DeltaTime);
            if (StunDuration == 0)
                EndStun();
        }

        private void EndStun() {
            StunDuration = 0;
            Animator.SetBool(AnimatorHash.Stunned, false);
        }
    }
}