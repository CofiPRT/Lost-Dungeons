namespace Character.Scripts.Properties {
    public interface IStunnable {
        float StunDuration { get; set; }
        bool IsStunned { get; }
        bool AttemptStun(float stunDuration, BaseImplementations.Character source);
    }
}