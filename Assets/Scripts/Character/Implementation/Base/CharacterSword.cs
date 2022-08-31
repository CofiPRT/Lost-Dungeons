using System;
using System.Collections.Generic;
using System.Linq;
using Properties;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        protected bool IsAttacking { get; set; }
        public bool IsPreparingToAttack { get; private set; }
        private bool AttackBlocksMovement { get; set; }

        private float AttackDamage { get; }
        private float AttackSpeed { get; }
        internal float AttackRange { get; }
        private float AttackAngle { get; }
        private AttackStrength AttackStrength { get; }

        private float LastAttackTimestamp { get; set; }
        private int AttackStreak { get; set; }

        protected internal IEnumerable<Team> AttackableTeams => Team switch {
            Team.Player => new[] { Team.Enemy },
            Team.Ally => new[] { Team.Enemy },
            Team.Enemy => new[] { Team.Player, Team.Ally },
            _ => Array.Empty<Team>()
        };

        public void AttemptAttack() {
            if (!IsAttacking) return;

            IsPreparingToAttack = false;
            var opponents = GetOpponentsInAttackArea();

            foreach (var opponent in opponents) {
                // delegate the attack to the shield wielder
                var damageDealt = opponent.AttemptBlock(AttackDamage, AttackStrength, this);

                // notify self
                if (damageDealt > 0)
                    OnAttackSuccess(opponent, damageDealt);
            }
        }

        public List<GenericCharacter> GetOpponentsInAttackRange() {
            // ReSharper disable once Unity.PreferNonAllocApi - we want every object in the range to be checked
            return Physics.OverlapSphere(
                    Pos,
                    AttackRange,
                    LayerMask.GetMask(AttackableTeams.Select(TeamUtils.ToLayer).ToArray())
                )
                .Select(x => x.gameObject.GetComponent<GenericCharacter>())
                .Where(x => x != null && x.IsAlive)
                .ToList();
        }

        private IEnumerable<GenericCharacter> GetOpponentsInAttackArea() {
            // check for attack-able objects in the attack range - first obtain a sphere overlap
            var opponents = GetOpponentsInAttackRange();

            // if no objects were hit, abort early
            if (opponents.Count == 0) return Array.Empty<GenericCharacter>();

            // the height difference is already accounted for, only check if the target is within the circle sector
            return opponents.Where(OpponentInAttackArea).ToArray();
        }

        public bool OpponentInAttackArea(GenericCharacter opponent) {
            var opponentPos = opponent.Pos2D;

            // the distance must be within range
            var distance = (opponentPos - Pos2D).magnitude;
            if (distance > AttackRange) return false;

            // the angle must be within the sector
            var angle = Vector2.Angle(Forward2D, opponentPos - Pos2D) * Mathf.Deg2Rad;
            return angle <= AttackAngle;
        }

        protected virtual void OnAttackSuccess(GenericCharacter target, float damageDealt) {
            // intentionally left blank
        }

        private bool CanStartAttack => IsAlive && !IsAttacking && !IsStunned;

        public void StartAttack(Vector2 direction) {
            if (!CanStartAttack) return;

            IsAttacking = true;
            IsPreparingToAttack = true;
            LookDirection = direction;

            // attacking can override blocking
            StopBlocking();

            // random attack animation
            Animator.SetInteger(AnimatorHash.AttackID, GetAttackID());
            Animator.SetFloat(AnimatorHash.AttackTickSpeed, AttackSpeed * TickSpeed);
        }

        private int GetAttackID() {
            var attackDelay = Time.time - LastAttackTimestamp;
            var acceptableDelay = 1 / AttackSpeed * TickSpeed;

            // reset attack streak
            if (attackDelay > acceptableDelay)
                AttackStreak = 0;

            // finisher animation, reset streak
            if (AttackStreak >= 3) {
                AttackStreak = 0;
                AttackBlocksMovement = true;
                return Random.Range(5, 7 + 1);
            }

            // combo animation, continue streak
            if (AttackStreak >= 2) {
                AttackStreak++;
                AttackBlocksMovement = true;
                return Random.Range(3, 4 + 1);
            }

            // normal animation, continue streak
            AttackStreak++;
            return Random.Range(1, 2 + 1);
        }

        private void EndAttack() {
            if (!IsAttacking) return;

            IsAttacking = false;
            IsPreparingToAttack = false;
            AttackBlocksMovement = false;
            Animator.SetInteger(AnimatorHash.AttackID, 0);
            LastAttackTimestamp = Time.time;
        }
    }
}