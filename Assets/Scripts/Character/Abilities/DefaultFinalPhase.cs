namespace Character.Abilities {
    public class DefaultFinalPhase : AbilityPhase {
        public DefaultFinalPhase(
            Ability ability,
            float maxDuration = 0,
            bool useUserDelta = true,
            bool requireReactivation = false,
            float manaCostPerSecond = 0
        ) : base(ability, maxDuration, useUserDelta, requireReactivation, manaCostPerSecond) { }

        protected override void OnEnd() {
            ability.StartCooldown();
        }
    }
}