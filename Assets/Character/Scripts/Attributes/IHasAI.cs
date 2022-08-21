namespace Character.Scripts.Attributes {
    public interface IHasAI {
        bool UseAI { get; set; }
        void SetAI(bool useAI);
        void RunAI();
    }
}