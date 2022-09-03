namespace Character.Implementation.Enemy.Groups {
    public class GroupSolo03 : GenericGroup {
        protected override void SpawnGroup() {
            GroupSpawner.SpawnSolo03(transform);
        }
    }
}