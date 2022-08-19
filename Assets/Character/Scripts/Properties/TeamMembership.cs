namespace Character.Scripts.Properties {
    public enum TeamMembership {
        Ally,
        Enemy
    }

    public static class TeamUtils {
        public static TeamMembership FromLayer(string layer) {
            return layer switch {
                "Ally" => TeamMembership.Ally,
                "Enemy" => TeamMembership.Enemy,
                _ => throw new System.Exception($"Unknown layer: {layer}")
            };
        }

        public static string ToLayer(TeamMembership membership) {
            return membership switch {
                TeamMembership.Ally => "Ally",
                TeamMembership.Enemy => "Enemy",
                _ => throw new System.Exception($"Unknown membership: {membership}")
            };
        }

        public static TeamMembership Opposite(TeamMembership membership) {
            return membership switch {
                TeamMembership.Ally => TeamMembership.Enemy,
                TeamMembership.Enemy => TeamMembership.Ally,
                _ => throw new System.Exception($"Unknown membership: {membership}")
            };
        }

        public static string Opposite(string layer) {
            return layer switch {
                "Ally" => "Enemy",
                "Enemy" => "Ally",
                _ => throw new System.Exception($"Unknown layer: {layer}")
            };
        }
    }
}