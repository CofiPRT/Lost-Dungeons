using UnityEngine;

namespace Scripts {
    public class AttackFinish : StateMachineBehaviour {
        private static readonly int AttackHash = Animator.StringToHash("attack");

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
            animator.SetInteger(AttackHash, 0);
        }
    }
}