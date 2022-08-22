namespace Properties {
    public enum Team {
        Player,
        Ally,
        Enemy
    }

    public static class TeamUtils {
        public static Team FromLayer(string layer) {
            return layer switch {
                "Player" => Team.Player,
                "Ally" => Team.Ally,
                "Enemy" => Team.Enemy,
                _ => throw new System.Exception($"Unknown layer: {layer}")
            };
        }

        public static string ToLayer(Team membership) {
            return membership switch {
                Team.Player => "Player",
                Team.Ally => "Ally",
                Team.Enemy => "Enemy",
                _ => throw new System.Exception($"Unknown membership: {membership}")
            };
        }
    }
}