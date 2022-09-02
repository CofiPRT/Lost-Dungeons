using System.Linq;
using Character.Implementation.Base;
using Character.Implementation.Enemy;
using Properties;
using UnityEngine;

namespace Character.Implementation.Ally.AIChecks {
    public class AllyTeamUpCheck : GenericCharacter.BaseAICheck {
        private const float SearchRadius = 10f;

        private readonly GenericAlly instance;

        public AllyTeamUpCheck(GenericAlly instance) : base(instance, 2, 3) {
            this.instance = instance;
        }

        protected override void Perform() {
            if (instance.FairFight.InFight)
                return; // already in a fight, don't team-up

            // look for a fight - search for an enemy in a radius
            // ReSharper disable once Unity.PreferNonAllocApi
            var enemies = Physics.OverlapSphere(
                    instance.Pos,
                    SearchRadius,
                    LayerMask.GetMask(instance.AttackableTeams.Select(TeamUtils.ToLayer).ToArray())
                )
                .Select(x => x.GetComponent<GenericEnemy>())
                .Where(x => x != null && x.IsAlive && x.FairFight != null && x.FairFight.IsFighting(x))
                .ToList();

            // choose one enemy at random
            if (enemies.Count == 0)
                return;

            var enemy = enemies[Random.Range(0, enemies.Count)];
            instance.FairFight.TeamUpOn(enemy);
        }
    }
}