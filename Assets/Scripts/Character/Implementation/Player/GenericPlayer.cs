using System;
using System.Collections.Generic;
using System.Linq;
using Camera;
using Character.Abilities;
using Character.Abilities.Shared;
using Character.Implementation.Ally;
using Character.Implementation.Base;
using Character.Implementation.Enemy;
using UnityEngine;

namespace Character.Implementation.Player {
    public abstract class GenericPlayer : GenericAlly {
        protected GenericPlayer(string name, Color signatureColor) : base(CreateData(name)) {
            SignatureColor = signatureColor;

            AbilityDodge = new DodgeAbility(this);
            AbilitySwitch = new SwitchAbility(this);
            AbilityTagTeam = new TagTeamAbility(this);
        }

        private static CharacterBuilder CreateData(string name) {
            return new CharacterBuilder {
                name = name,
                team = Properties.Team.Player
            };
        }

        public Color SignatureColor { get; }

        /* Shared Abilities */

        public DodgeAbility AbilityDodge { get; }
        public SwitchAbility AbilitySwitch { get; }
        public TagTeamAbility AbilityTagTeam { get; }

        /* Specific Abilities */

        public IAbility Ability1 { get; protected set; }
        public IAbility Ability2 { get; protected set; }
        public IAbility Ultimate { get; protected set; }

        protected bool UltimateActive { get; private set; }

        /* Ability Logic */

        public bool CastBlocksAbilityUsage { get; set; }
        public bool CastBlocksMovement { get; set; }
        public bool CastBlocksAttack { get; set; }
        public bool CastBlocksBlock { get; set; }
        public bool CastBlocksMovementLookDirectionSync { get; set; }

        private IEnumerable<IAbility> Abilities => new[]
            { AbilityDodge, AbilitySwitch, AbilityTagTeam, Ability1, Ability2, Ultimate };

        public virtual void StartUltimate() {
            UltimateActive = true;
        }

        public void StopUltimate() {
            UltimateActive = false;
        }

        private void UpdateAbilities() {
            if (!IsAlive)
                return;

            foreach (var ability in Abilities)
                ability.Update();
        }

        public bool StartDodge(Vector2 direction) {
            return AbilityDodge.Use(direction);
        }

        public bool StartTagTeam(bool changePlayers) {
            return AbilityTagTeam.Use(changePlayers);
        }

        /* Util */

        public void SwapPositions(GenericPlayer other) {
            var ownTransform = transform;
            var otherTransform = other.transform;

            (ownTransform.position, otherTransform.position) = (otherTransform.position, ownTransform.position);
        }

        public void SwapFairFights(GenericPlayer other) {
            (FairFight, other.FairFight) = (other.FairFight, FairFight);

            // also change owners of the fair fights
            FairFight.Owner = this;
            other.FairFight.Owner = other;
        }

        private const float ConeAngle = Mathf.PI / 8;
        private const float ConeDistance = 10f;

        public T FindObjectInCone<T>(
            string layer,
            Func<T, Vector3> positionSelector,
            Func<T, bool> extraCondition = null
        )
            where T : class {
            // ReSharper disable once Unity.PreferNonAllocApi
            var props = Physics.OverlapSphere(
                    Pos,
                    ConeDistance,
                    LayerMask.GetMask(layer)
                )
                .Select(x => x.GetComponent<T>())
                .Where(
                    x => x != null &&
                         (extraCondition == null || extraCondition(x)) &&
                         ObjectInCone(positionSelector(x))
                )
                .ToList();

            return props.Count == 0
                ? null
                : props.OrderBy(x => Vector3.Distance(Pos, positionSelector(x))).First();
        }

        public static GenericEnemy FindEnemyInPropDistance(Vector3 origin) {
            // ReSharper disable once Unity.PreferNonAllocApi
            var enemies = Physics.OverlapSphere(
                    origin,
                    5f,
                    LayerMask.GetMask("Enemy")
                )
                .Select(x => x.GetComponent<GenericEnemy>())
                .Where(x => x != null && x.IsAlive)
                .ToList();

            return enemies.Count == 0
                ? null
                : enemies.OrderBy(x => Vector3.Distance(origin, x.Pos)).First();
        }

        private bool ObjectInCone(Vector3 objPos) {
            var direction = (objPos - CameraController.Pos).normalized;
            var angle = Vector3.Angle(direction, CameraController.Forward) * Mathf.Deg2Rad;
            return angle < ConeAngle;
        }

        /* Parent */

        protected override bool CanApplyMovement => base.CanApplyMovement && !CastBlocksMovement;
        protected override bool CanStartAttack => base.CanStartAttack && !CastBlocksAttack;
        protected override bool CanStartBlocking => base.CanStartBlocking && !CastBlocksBlock;

        protected override bool MovementCanSyncLookDirection =>
            base.MovementCanSyncLookDirection && !CastBlocksMovementLookDirectionSync;

        protected override UpdateDelegate UpdateActions => base.UpdateActions + UpdateAbilities;

        protected override void OnAttackSuccess(GenericCharacter target, float damageDealt) {
            base.OnAttackSuccess(target, damageDealt);

            ReplenishMana(5f);
        }

        public override void StartAttack(Vector2 direction) {
            base.StartAttack(direction);

            Ultimate.OnAttack();
        }
    }
}