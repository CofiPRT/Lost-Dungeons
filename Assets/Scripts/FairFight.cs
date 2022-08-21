using System.Collections.Generic;
using System.Linq;
using Scripts.Character.Implementation.Base;
using UnityEngine;

namespace Scripts {
    public class FairFight {
        private const int DefaultMaxFightingEnemies = 3;

        public GenericCharacter Owner { get; }

        private readonly int maxFightingEnemies;
        private readonly HashSet<GenericCharacter> fightingEnemies = new HashSet<GenericCharacter>();
        private readonly HashSet<GenericCharacter> waitingEnemies = new HashSet<GenericCharacter>();

        public FairFight(GenericCharacter owner, int maxFightingEnemies = DefaultMaxFightingEnemies) {
            Owner = owner;
            this.maxFightingEnemies = maxFightingEnemies;
        }

        public void Subscribe(GenericCharacter enemy) {
            if (IsSubscribed(enemy))
                return;

            if (fightingEnemies.Count < maxFightingEnemies)
                fightingEnemies.Add(enemy);
            else
                waitingEnemies.Add(enemy);
        }

        public void Unsubscribe(GenericCharacter enemy) {
            fightingEnemies.Remove(enemy);
            waitingEnemies.Remove(enemy);
        }

        public void ForceSubscribe(GenericCharacter enemy) {
            if (IsFighting(enemy))
                return;

            // if there's no room for fighting, put a random fighting enemy into the waiting state
            if (fightingEnemies.Count == maxFightingEnemies) {
                var randomEnemy = fightingEnemies.ToArray()[Random.Range(0, fightingEnemies.Count)];
                fightingEnemies.Remove(randomEnemy);
                waitingEnemies.Add(randomEnemy);
            }

            // subscribe the new enemy
            Subscribe(enemy);
        }

        public bool IsSubscribed(GenericCharacter enemy) {
            return fightingEnemies.Contains(enemy) || waitingEnemies.Contains(enemy);
        }

        public bool IsFighting(GenericCharacter enemy) {
            return fightingEnemies.Contains(enemy);
        }

        public bool IsWaiting(GenericCharacter enemy) {
            return waitingEnemies.Contains(enemy);
        }
    }
}