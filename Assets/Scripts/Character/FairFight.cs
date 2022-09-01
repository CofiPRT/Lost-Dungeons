using System;
using System.Collections.Generic;
using System.Linq;
using Character.Implementation.Ally;
using Character.Implementation.Enemy;
using Random = UnityEngine.Random;

namespace Character {
    public class FairFight : IComparable<FairFight> {
        private const int DefaultMaxFightingEnemies = 3;

        public GenericAlly Owner { get; set; }
        public GenericEnemy LastFoughtEnemy { get; set; }

        private readonly int maxFightingEnemies;

        private GenericEnemy teamUpEnemy;
        private readonly HashSet<GenericEnemy> fightingEnemies = new HashSet<GenericEnemy>();
        private readonly HashSet<GenericEnemy> waitingEnemies = new HashSet<GenericEnemy>();


        public FairFight(GenericAlly owner, int maxFightingEnemies = DefaultMaxFightingEnemies) {
            Owner = owner;
            this.maxFightingEnemies = maxFightingEnemies;
        }

        public void TeamUpOn(GenericEnemy enemy) {
            teamUpEnemy = enemy;
        }

        public void Subscribe(GenericEnemy enemy) {
            if (IsSubscribed(enemy))
                return;

            enemy.FairFight = this;

            if (MaxFightingEnemiesReached)
                waitingEnemies.Add(enemy);
            else
                fightingEnemies.Add(enemy);

            // stop teaming up on this enemy if we're engaged in a proper fight
            teamUpEnemy = null;
        }

        public void Unsubscribe(GenericEnemy enemy) {
            if (enemy == null)
                return;

            fightingEnemies.Remove(enemy);
            waitingEnemies.Remove(enemy);

            if (LastFoughtEnemy == enemy)
                LastFoughtEnemy = null;

            enemy.FairFight = null;
        }

        public void ForceSubscribe(GenericEnemy enemy) {
            if (IsFighting(enemy))
                return;

            // if there's no room for fighting, put a random fighting enemy into the waiting state
            if (MaxFightingEnemiesReached) {
                var randomEnemy = GetRandomFightingEnemy();
                fightingEnemies.Remove(randomEnemy);
                waitingEnemies.Add(randomEnemy);
            }

            // subscribe the new enemy
            Subscribe(enemy);

            LastFoughtEnemy = enemy;
        }

        public void UnsubscribeAll() {
            foreach (var enemy in fightingEnemies.ToArray())
                Unsubscribe(enemy);

            foreach (var enemy in waitingEnemies.ToArray())
                Unsubscribe(enemy);
        }

        public bool IsSubscribed(GenericEnemy enemy) {
            return fightingEnemies.Contains(enemy) || waitingEnemies.Contains(enemy);
        }

        public bool IsFighting(GenericEnemy enemy) {
            return fightingEnemies.Contains(enemy);
        }

        public bool IsWaiting(GenericEnemy enemy) {
            return waitingEnemies.Contains(enemy);
        }

        public bool InFight => fightingEnemies.Count > 0 || teamUpEnemy != null;
        public bool MaxFightingEnemiesReached => fightingEnemies.Count == maxFightingEnemies;

        public GenericEnemy GetRandomFightingEnemy() {
            if (!InFight)
                return null;

            return fightingEnemies.Count > 0
                ? fightingEnemies.ToArray()[Random.Range(0, fightingEnemies.Count)]
                : teamUpEnemy;
        }

        public void Update() {
            // unsubscribe dead enemies
            foreach (var enemy in fightingEnemies.Where(enemy => !enemy.IsAlive).ToArray())
                Unsubscribe(enemy);

            foreach (var enemy in waitingEnemies.Where(enemy => !enemy.IsAlive).ToArray())
                Unsubscribe(enemy);

            if (teamUpEnemy != null && !teamUpEnemy.IsAlive)
                teamUpEnemy = null;

            // if there's more room for fighting, subscribe a random waiting enemy
            if (MaxFightingEnemiesReached || waitingEnemies.Count <= 0) return;

            var randomEnemy = waitingEnemies.ToArray()[Random.Range(0, waitingEnemies.Count)];
            waitingEnemies.Remove(randomEnemy);
            fightingEnemies.Add(randomEnemy);
            randomEnemy.FairFight = this;
        }

        public int CompareTo(FairFight other) {
            // compare by fighting enemies
            if (fightingEnemies.Count > other.fightingEnemies.Count)
                return 1;

            if (fightingEnemies.Count < other.fightingEnemies.Count)
                return -1;

            // compare by waiting enemies
            if (waitingEnemies.Count > other.waitingEnemies.Count)
                return 1;

            if (waitingEnemies.Count < other.waitingEnemies.Count)
                return -1;

            return 0;
        }
    }
}