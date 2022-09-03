using Properties;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        protected string Name { get; }
        private Team Team { get; }

        protected virtual void OnKill(GenericCharacter target) {
            // intentionally left blank
        }
    }
}