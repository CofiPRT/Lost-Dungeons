﻿using Game;
using Properties;
using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public Animator Animator { get; set; }
        public float DeltaTime => Time.deltaTime * TickSpeed;
        public float FixedDeltaTime => Time.fixedDeltaTime * TickSpeed;

        public float TickSpeed => Team switch {
            Team.Player => GameController.GameTickSpeed * GameController.PlayerTickFactor,
            Team.Ally => GameController.GameTickSpeed,
            Team.Enemy => GameController.GameTickSpeed,
            _ => 1
        };

        private void UpdateTickSpeeds() {
            Animator.SetFloat(AnimatorHash.AnimationTickSpeed, TickSpeed);
            Animator.SetFloat(AnimatorHash.MovementTickSpeed, MovementSpeedFactor * TickSpeed);
            Animator.SetFloat(AnimatorHash.AttackTickSpeed, AttackSpeed * TickSpeed);
        }

        private void AwakeAnimator() {
            Animator = GetComponent<Animator>();
        }
    }
}