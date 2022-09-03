namespace Character.Implementation.Enemy.Groups {
    public class GroupDuo02 : GenericGroup {
        protected override void SpawnGroup() {
            GroupSpawner.SpawnDuo02(transform);
        }
    }
}