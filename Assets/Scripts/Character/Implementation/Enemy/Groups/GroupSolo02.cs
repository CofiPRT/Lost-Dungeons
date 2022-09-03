namespace Character.Implementation.Enemy.Groups {
    public class GroupSolo02 : GenericGroup {
        protected override void SpawnGroup() {
            GroupSpawner.SpawnSolo02(transform);
        }
    }
}