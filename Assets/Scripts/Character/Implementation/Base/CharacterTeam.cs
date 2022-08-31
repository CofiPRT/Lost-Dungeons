using Properties;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public string Name { get; }
        public Team Team { get; }

        public virtual void OnKill(GenericCharacter target) {
            // intentionally left blank
        }
    }
}