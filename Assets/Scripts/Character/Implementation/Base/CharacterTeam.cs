using Properties;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public Team Team { get; }

        public void OnKill(GenericCharacter target) {
            // intentionally left blank
        }
    }
}