using Character.Implementation.Ally;
using Character.Implementation.Enemy;
using Character.Implementation.Player;
using Character.Misc;
using Props;
using UnityEngine;

namespace Game.Util {
    public class Instances : MonoBehaviour {
        [Header("Players")]
        public PlayerTristian tristian;
        public PlayerReinald reinald;
        public GenericAlly ally;

        [Header("Enemies")]
        public EnemyWhite enemyWhite;
        public EnemyOrange enemyOrange;
        public EnemyBlack enemyBlack;
        public EnemyViolet enemyViolet;

        [Header("Props")]
        public GenericProp vase;
        public GenericProp bottle;
        public GenericProp cauldron;
        public GenericProp inkwell;
        public GenericProp cup;
        public GenericProp jar;

        [Header("Misc")]
        public Canvas redHealthBar;
        public Canvas greenHealthBar;
        public GenericDecoy decoy;
    }
}