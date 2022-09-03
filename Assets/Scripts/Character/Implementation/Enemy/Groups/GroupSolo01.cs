namespace Character.Implementation.Enemy.Groups {
    public class GroupSolo01 : GenericGroup {
        protected override void SpawnGroup() {
            GroupSpawner.SpawnSolo01(transform);
        }
    }
}