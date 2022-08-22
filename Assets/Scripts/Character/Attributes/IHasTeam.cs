using Properties;

namespace Character.Attributes {
    public interface IHasTeam {
        Team Team { get; }
        void OnKill(Implementation.Base.GenericCharacter target);
    }
}