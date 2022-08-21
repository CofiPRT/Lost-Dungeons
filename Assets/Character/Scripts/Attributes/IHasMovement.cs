using UnityEngine;

namespace Character.Scripts.Attributes {
    public interface IHasMovement {
        Rigidbody RigidBody { get; set; }
        Vector2 LookDirection { get; set; }
        float RotationSpeed { get; }
        float MovementSpeedFactor { get; }
        bool CanApplyMovement { get; }
        void ApplyMovement(Vector2 direction, bool run);
        void StopMoving();
    }
}