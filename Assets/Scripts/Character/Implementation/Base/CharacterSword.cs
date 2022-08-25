﻿using System;
using System.Collections.Generic;
using System.Linq;
using Properties;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public bool IsAttacking { get; set; }
        public bool IsPreparingToAttack { get; set; }
        public float AttackDamage { get; }
        public float AttackSpeed { get; }
        public float AttackCooldown { get; set; }
        public float AttackRange { get; }
        public float AttackAngle { get; }

        public IEnumerable<Team> AttackableTeams => Team switch {
            Team.Player => new[] { Team.Enemy },
            Team.Ally => new[] { Team.Enemy },
            Team.Enemy => new[] { Team.Player, Team.Ally },
            _ => Array.Empty<Team>()
        };

        public void AttemptAttack(float damageMultiplier, AttackStrength attackStrength) {
            if (!IsAttacking) return;

            IsPreparingToAttack = false;

            var damage = (int)(AttackDamage * damageMultiplier);
            var opponents = GetOpponentsInAttackArea();

            foreach (var opponent in opponents) {
                // delegate the attack to the shield wielder
                var damageDealt = opponent.AttemptBlock(damage, attackStrength, this);

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

        public IEnumerable<GenericCharacter> GetOpponentsInAttackArea() {
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
            var angle = Vector2.Angle(Forward2D, opponentPos - Pos2D);
            return angle <= AttackAngle;
        }

        public virtual void OnAttackSuccess(GenericCharacter target, float damageDealt) {
            // intentionally left blank
        }

        public bool CanStartAttack => IsAlive && !IsAttacking && !IsStunned && AttackCooldown == 0;

        public void StartAttack(Vector2 direction) {
            if (!CanStartAttack) return;

            IsAttacking = true;
            IsPreparingToAttack = true;
            AttackCooldown = 1 / AttackSpeed;
            LookDirection = direction;

            // attacking can override blocking
            StopBlocking();

            // random attack animation out of two available
            Animator.SetInteger(AnimatorHash.Attacking, Random.Range(1, 3));
            Animator.SetFloat(AnimatorHash.AttackTickSpeed, AttackSpeed * TickSpeed);
        }

        public void EndAttack() {
            if (!IsAttacking) return;

            IsAttacking = false;
            Animator.SetInteger(AnimatorHash.Attacking, 0);
        }

        public void UpdateAttackCooldown() {
            if (!IsAlive)
                return;

            AttackCooldown = Mathf.Max(0, AttackCooldown - DeltaTime);
        }
    }
}