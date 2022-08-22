using UnityEngine;

public class PlayerAI : MonoBehaviour {
    public float walkStartDistance = 3.0f;
    public float walkStopDistance = 2.0f;
    public float runStartDistance = 5.0f;
    public float runStopDistance = 2.75f;

    private PlayerController controller;

    private void Start() {
        controller = GetComponent<PlayerController>();
    }

    private void Update() {
        if (!controller.automated || !controller.updateInputs)
            return;

        var allyPos = controller.ally.transform.position;
        var allyDistance = Vector3.Distance(transform.position, allyPos);

        if (allyDistance > walkStartDistance)
            controller.movementInput = allyPos - transform.position; // apply movement towards ally
        else if (allyDistance < walkStopDistance)
            controller.movementInput = Vector3.zero;

        // handle running
        if (allyDistance > runStartDistance)
            controller.runPressed = true;
        else if (allyDistance < runStopDistance)
            controller.runPressed = false;
    }
}