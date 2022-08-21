namespace Scripts.Properties {
    public enum AttackStrength {
        Weak,
        Medium,
        Strong
    }

    public static class AttackScore {
        public static int Of(AttackStrength strength) {
            return strength switch {
                AttackStrength.Weak => 1,
                AttackStrength.Medium => 2,
                AttackStrength.Strong => 3,
                _ => 0
            };
        }
    }
}