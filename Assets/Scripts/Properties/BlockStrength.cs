namespace Properties {
    public enum BlockStrength {
        Weak,
        Medium,
        Strong
    }

    public static class BlockScore {
        public static int Of(BlockStrength strength) {
            return strength switch {
                BlockStrength.Weak => 1,
                BlockStrength.Medium => 2,
                BlockStrength.Strong => 3,
                _ => 0
            };
        }
    }
}