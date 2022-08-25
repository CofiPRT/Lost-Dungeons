﻿using System;
using System.Collections.Generic;
using System.Linq;
using Character.Implementation.Base;
using Random = UnityEngine.Random;

namespace Character {
    public class FairFight : IComparable {
        private const int DefaultMaxFightingEnemies = 3;

        public GenericCharacter Owner { get; }
        public GenericCharacter LastFoughtEnemy { get; set; }

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

            if (MaxFightingEnemiesReached)
                waitingEnemies.Add(enemy);
            else
                fightingEnemies.Add(enemy);
        }

        public void Unsubscribe(GenericCharacter enemy) {
            fightingEnemies.Remove(enemy);
            waitingEnemies.Remove(enemy);
            
            if (LastFoughtEnemy == enemy)
                LastFoughtEnemy = null;
        }

        public void ForceSubscribe(GenericCharacter enemy) {
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

        public bool InFight => fightingEnemies.Count > 0;
        public bool MaxFightingEnemiesReached => fightingEnemies.Count == maxFightingEnemies;

        public GenericCharacter GetRandomFightingEnemy() {
            return !InFight ? null : fightingEnemies.ToArray()[Random.Range(0, fightingEnemies.Count)];
        }

        public int CompareTo(object obj) {
            if (!(obj is FairFight other))
                return 1;

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