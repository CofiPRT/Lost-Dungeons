namespace Character.Implementation.Enemy.Groups {
    public class GroupDuo01 : GenericGroup {
        protected override void SpawnGroup() {
            GroupSpawner.SpawnDuo01(transform);
        }
    }
}