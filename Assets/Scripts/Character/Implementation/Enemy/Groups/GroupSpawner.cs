using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Character.Implementation.Enemy.Groups {
    public static class GroupSpawner {
        private static Instances Instances => GameController.DefaultInstances;

        private static Vector2 Rotate(Vector2 v, float radians) {
            var ca = Mathf.Cos(radians);
            var sa = Mathf.Sin(radians);

            return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
        }

        private static void SpawnGroup(Transform origin, IReadOnlyCollection<GenericEnemy> enemies) {
            var position = origin.position;
            var forward = origin.forward;

            var direction2D = new Vector2(forward.x, forward.z);
            var direction3D = new Vector3(direction2D.x, 0, direction2D.y);

            var rotationAngle = Mathf.PI * 2 / enemies.Count;

            foreach (var enemy in enemies) {
                var spawnedEnemy = Object.Instantiate(
                    enemy,
                    position + direction3D + Vector3.up * 0.5f,
                    Quaternion.LookRotation(-direction3D),
                    GameController.SpawnContainer
                );
                GameController.AliveEnemies.Add(spawnedEnemy);

                direction2D = Rotate(direction2D, rotationAngle);
                direction3D = new Vector3(direction2D.x, 0, direction2D.y);
            }
        }

        public static void SpawnSolo01(Transform origin) {
            SpawnGroup(
                origin,
                new GenericEnemy[] {
                    Instances.enemyWhite,
                    Instances.enemyWhite,
                    Instances.enemyWhite,
                    Instances.enemyWhite
                }
            );
        }

        public static void SpawnSolo02(Transform origin) {
            SpawnGroup(
                origin,
                new GenericEnemy[] {
                    Instances.enemyOrange,
                    Instances.enemyWhite,
                    Instances.enemyWhite,
                    Instances.enemyWhite
                }
            );
        }

        public static void SpawnSolo03(Transform origin) {
            SpawnGroup(
                origin,
                new GenericEnemy[] {
                    Instances.enemyBlack,
                    Instances.enemyOrange,
                    Instances.enemyOrange,
                    Instances.enemyWhite,
                    Instances.enemyWhite
                }
            );
        }

        public static void SpawnDuo01(Transform origin) {
            SpawnGroup(
                origin,
                new GenericEnemy[] {
                    Instances.enemyBlack,
                    Instances.enemyOrange,
                    Instances.enemyOrange,
                    Instances.enemyOrange
                }
            );
        }

        public static void SpawnDuo02(Transform origin) {
            SpawnGroup(
                origin,
                new GenericEnemy[] {
                    Instances.enemyBlack,
                    Instances.enemyBlack,
                    Instances.enemyOrange,
                    Instances.enemyOrange
                }
            );
        }

        public static void SpawnDuo03(Transform origin) {
            SpawnGroup(
                origin,
                new GenericEnemy[] {
                    Instances.enemyViolet,
                    Instances.enemyBlack,
                    Instances.enemyBlack,
                    Instances.enemyOrange,
                    Instances.enemyOrange
                }
            );
        }
    }
}