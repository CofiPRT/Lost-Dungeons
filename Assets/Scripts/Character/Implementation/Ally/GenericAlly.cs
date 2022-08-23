using Character.Implementation.Ally.AIChecks;
using Character.Implementation.Base;
using Character.Implementation.Player;
using Game;

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
        public GenericPlayer Leader => GameController.Instance.controllerPlayer;
    }
}