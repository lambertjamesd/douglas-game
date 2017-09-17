using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingSights : State {
    public PlayerMovement movementState;
    public DefaultMovement movement;
    public float speed;
    public CameraTarget cameraTarget;
    public float cameraOffset;

    public override void StateBegin()
    {
        movement.LockRotation();
        cameraTarget.targetOffset = movement.direction.transform.TransformDirection(Vector3.right) * cameraOffset;
    }

    public override IState UpdateState(float deltaTime)
    {
        if (!Input.GetButton("UseSights"))
        {
            return movementState;
        }
        else
        {
            return movementState.CommonMovement(deltaTime, speed);
        }
    }

    public override void StateEnd()
    {
        movement.UnlockRotation();
        cameraTarget.targetOffset = Vector3.zero;
    }
}
