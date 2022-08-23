using Character.Implementation.Ally;
using Character.Implementation.Base;

namespace Character.Implementation.Player {
    public class GenericPlayer : GenericAlly {
        protected GenericPlayer(CharacterBuilder data) : base(data) { }
    }
}