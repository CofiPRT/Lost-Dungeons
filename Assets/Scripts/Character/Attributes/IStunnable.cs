namespace Scripts.Character.Attributes {
    public interface IStunnable {
        float StunDuration { get; set; }
        bool IsStunned { get; }
        bool AttemptStun(float stunDuration, Implementation.Base.GenericCharacter source);
        void EndStun();
        void UpdateStunDuration();
    }
}