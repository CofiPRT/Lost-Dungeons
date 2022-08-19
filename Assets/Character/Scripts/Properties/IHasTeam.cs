namespace Character.Scripts.Properties {
    public interface IHasTeam {
        TeamMembership TeamMembership { get; }
        void OnKill(BaseImplementations.Character target);
    }
}