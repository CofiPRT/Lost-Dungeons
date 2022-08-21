namespace Scripts.Character.Abilities {
    public class DefaultFinalPhase : AbilityPhase {
        public DefaultFinalPhase(
            Ability ability,
            float manaCostPerSecond = 0,
            float maxDuration = 0,
            bool useUserDelta = true,
            bool requireReactivation = false
        ) : base(ability, manaCostPerSecond, maxDuration, useUserDelta, requireReactivation) { }

        protected override void OnEnd() {
            ability.StartCooldown();
        }
    }
}