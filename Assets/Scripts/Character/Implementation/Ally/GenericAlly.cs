using Character.Implementation.Ally.AIChecks;
using Character.Implementation.Base;
using Character.Implementation.Player;
using Game;
using UnityEngine;

namespace Character.Implementation.Ally {
    public abstract class GenericAlly : GenericCharacter {
        protected GenericAlly(CharacterBuilder data) : base(data) {
            FairFight = new FairFight(this);
            AIChecks = new BaseAICheck[] {
                new AllyFollowCheck(this),
                new AllyAttackCheck(this),
                new AllyBlockCheck(this),
                new AllyWanderCheck(this)
            };
        }

        public FairFight FairFight { get; }
        public GenericPlayer Leader => GameController.ControllerPlayer;

        /* Parent */

        public override Vector2 LookDirection =>
            FairFight.InFight ? FairFight.LastFoughtEnemy.Pos2D - Pos2D : Forward2D;

        public override void OnAttackSuccess(GenericCharacter target, float damageDealt) {
            FairFight.LastFoughtEnemy = target;
            FairFight.ForceSubscribe(target);
        }
        
        public override void OnDamageTaken(float damageTaken, GenericCharacter source) {
            FairFight.LastFoughtEnemy = source;
        }
    }
}