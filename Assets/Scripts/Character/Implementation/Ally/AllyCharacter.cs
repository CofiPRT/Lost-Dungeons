using Character.Implementation.Base;
using Properties;

namespace Character.Implementation.Ally {
    public class AllyCharacter : GenericAlly {
        public AllyCharacter() : base(CreateData()) { }

        private static CharacterBuilder CreateData() {
            return new CharacterBuilder {
                name = "Ally",
                team = Team.Ally
            };
        }
    }
}