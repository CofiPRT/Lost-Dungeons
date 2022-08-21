namespace Character.Scripts.Attributes {
    public interface IStunnable {
        float StunDuration { get; set; }
        bool IsStunned { get; }
        bool AttemptStun(float stunDuration, Generic.Character source);
        void EndStun();
    }
}