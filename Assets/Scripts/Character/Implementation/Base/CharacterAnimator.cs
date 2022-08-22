using Game;
using Properties;
using UnityEngine;

namespace Character.Implementation.Base {
    public abstract partial class GenericCharacter {
        public Animator Animator { get; set; }
        public float DeltaTime => Time.deltaTime * TickSpeed;

        public float TickSpeed => Team switch {
            Team.Player => GameController.Instance.gameTickSpeed * GameController.Instance.playerTickFactor,
            Team.Ally => GameController.Instance.gameTickSpeed,
            Team.Enemy => GameController.Instance.gameTickSpeed,
            _ => 1
        };
    }
}