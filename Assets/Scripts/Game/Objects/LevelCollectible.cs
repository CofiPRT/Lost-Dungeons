using Character.Implementation.Player;
using UnityEngine;

namespace Game.Objects {
    public class LevelCollectible : MonoBehaviour {
        private const float LevitationSpeed = 2f;
        private const float LiftSpeed = 5f;
        private const float SpinSpeed = 40f;

        private Vector3 originalPosition;

        private bool collected;
        private float collectedTime;

        private void Start() {
            GameController.AddCollectible(this);
            originalPosition = transform.position;

            // set inactive
            gameObject.SetActive(false);
        }

        private void OnTriggerStay(Collider other) {
            if (collected) return;

            // if the controlled player is in the trigger, the item will be collected
            var player = other.GetComponent<GenericPlayer>();

            if (player != GameController.ControlledPlayer)
                return;

            collected = true;
            collectedTime = Time.time;
            GameController.RemoveCollectible(this);
        }

        private void Update() {
            if (collected) {
                // the item will be destroyed after 1 second
                if (Time.time - collectedTime > 1f)
                    Destroy(gameObject);

                // the item gets lifted
                transform.position += Vector3.up * (LiftSpeed * Time.deltaTime);

                return;
            }

            // the item levitates with a sine curve
            transform.position = originalPosition + Vector3.up * Mathf.Sin(Time.time * LevitationSpeed);

            // the item spins around the y axis
            transform.Rotate(0, Time.deltaTime * SpinSpeed, 0);
        }
    }
}