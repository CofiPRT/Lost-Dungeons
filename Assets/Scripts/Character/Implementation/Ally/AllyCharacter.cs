using Character.Implementation.Base;

namespace Character.Implementation.Ally {
    public class AllyCharacter : GenericAlly {
        public AllyCharacter() : base(CreateData()) { }

        private static CharacterBuilder CreateData() {
            return new CharacterBuilder {
                name = "Ally",
                team = Properties.Team.Ally
            };
        }
    }
}