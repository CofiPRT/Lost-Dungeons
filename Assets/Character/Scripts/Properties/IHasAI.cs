namespace Character.Scripts.Properties {
    public interface IHasAI {
        bool UseAI { get; set; }
        void SetAI(bool useAI);
        void RunAI();
    }
}