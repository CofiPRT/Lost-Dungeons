using Character.Implementation.Base;
using Game;
using Properties;
using UnityEngine;

namespace Character.Implementation.Enemy {
    public class EnemyBlack : GenericEnemy {
        public EnemyBlack() : base(CreateData()) { }

        private static CharacterBuilder CreateData() {
            return new CharacterBuilder {
                name = "Black Enemy",

                maxHealth = 100,
                attackDamage = 10,

                attackStrength = AttackStrength.Strong,
                blockStrength = BlockStrength.Medium
            };
        }

        private bool hasSpawnedBackup;

        private static readonly Vector2[] BackupSpawnPositions = {
            new Vector2(-1, -1),
            new Vector2(-1, 1)
        };

        protected internal override float TakeDamage(float damage, GenericCharacter source = null) {
            var damageTaken = base.TakeDamage(damage, source);

            if (Health <= MaxHealth * 0.5f && !hasSpawnedBackup) {
                hasSpawnedBackup = true;
                SpawnBackup();
            }

            return damageTaken;
        }

        private void SpawnBackup() {
            var ownTransform = transform;

            foreach (var spawnPosition in BackupSpawnPositions) {
                var pos = Pos2D + Forward2D * spawnPosition.x + Right2D * spawnPosition.y;
                var pos3D = new Vector3(pos.x, Pos.y + 1, pos.y);
                Instantiate(
                    GameController.DefaultInstances.enemyWhite,
                    pos3D,
                    ownTransform.rotation,
                    GameController.SpawnContainerEnemies
                );
            }
        }
    }
}