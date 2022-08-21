using Scripts.Character.Implementation.Base;
using Scripts.Properties;

namespace Scripts.Character.Implementation.Enemy {
    public abstract class GenericEnemy : KnightCharacter {
        protected GenericEnemy(CharacterData data) : base(SetTeam(data)) { }

        private static CharacterData SetTeam(CharacterData data) {
            data.team = Team.Enemy;
            return data;
        }

        /* IHasAI */

        public override void RunAI() { }
    }
}