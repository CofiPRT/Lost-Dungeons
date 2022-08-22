using System.Runtime.CompilerServices;
using Character.Implementation.Base;
using Properties;

namespace Character.Implementation.Ally {
    public class GenericAlly : KnightCharacter {
        protected GenericAlly(CharacterBuilder data) : base(SetTeam(data)) {
            FairFight = new FairFight(this);
        }

        private static CharacterBuilder SetTeam(CharacterBuilder data) {
            data.team = Team.Enemy;
            return data;
        }

        public FairFight FairFight { get; }
    }
}