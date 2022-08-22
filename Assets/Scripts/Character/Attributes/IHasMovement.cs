using UnityEngine;

namespace Character.Attributes {
    public interface IHasMovement {
        Rigidbody RigidBody { get; set; }
        Vector2 LookDirection { get; set; }
        Vector2 MovementApplication { get; set; }
        bool MovementRun { get; set; }
        float Velocity { get; set; }
        float Acceleration { get; }
        float Deceleration { get; }
        float RotationSpeed { get; }
        float MovementSpeedFactor { get; }
        bool CanApplyMovement { get; }
        void ApplyMovement(Vector2 direction, bool run, bool syncLookDirection);
        void StopMoving();
        void UpdateMovement();
        void UpdateLookDirection();
    }

    public class MovementApplication { }
}