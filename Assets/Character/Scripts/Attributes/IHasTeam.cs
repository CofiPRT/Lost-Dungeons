using Character.Scripts.Properties;

namespace Character.Scripts.Attributes {
    public interface IHasTeam {
        Team Team { get; }
        void OnKill(Generic.Character target);
    }
}