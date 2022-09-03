namespace Character.Implementation.Enemy.Groups {
    public class GroupDuo03 : GenericGroup {
        protected override void SpawnGroup() {
            GroupSpawner.SpawnDuo03(transform);
        }
    }
}