using Character.Implementation.Enemy;
using Character.Implementation.Player;
using Character.Misc;
using Props;
using UnityEngine;

namespace Game {
    public class Instances : MonoBehaviour {
        [Header("Players")] public GenericPlayer tristian;
        public GenericPlayer reinald;

        [Header("Enemies")] public GenericEnemy enemyWhite;

        [Header("Props")] public GenericProp vase;
        public GenericProp bottle;
        public GenericProp cauldron;
        public GenericProp inkwell;
        public GenericProp cup;
        public GenericProp jar;

        [Header("Misc")] public Canvas healthBar;
        public GenericDecoy decoy;
    }
}