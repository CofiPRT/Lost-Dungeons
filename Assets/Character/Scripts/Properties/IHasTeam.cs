namespace Character.Scripts.Properties {
    public interface IHasTeam {
        Team Team { get; }
        void OnKill(Base.Character target);
    }
}