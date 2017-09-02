using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineUpShot : State {
    public State lostTargetState;
    public State shootState;
    private Transform target;
    public Sight sight;
    public DefaultMovement movement;
    private Vector3 lastSeenLocation;
    public LayerMask raycastLayers;
    public float targetOffset;
    public float moveSpeed;

    public override void StateBegin()
    {
        movement.LockRotation();

        Collider2D visibleObject = sight.GetVisibleObject();
        
        if (visibleObject != null)
        {
            target = visibleObject.transform;
        }
    }

    public static IEnumerator LineUp(Transform target, Transform from, DefaultMovement movement, Sight sight, float targetOffset, float moveSpeed)
    {
        do
        {
            movement.TargetVelocity = TargetVelocity(target, from, movement, targetOffset) * moveSpeed;
            yield return null;
        }
        while (sight.canSeeObject(target.gameObject) && (movement.TargetVelocity != Vector2.zero || movement.Velocity != Vector2.zero));
    }

    public static Vector2 TargetVelocity(Transform target, Transform from, DefaultMovement movement, float targetOffset)
    {
        Vector2 offset = target.transform.position - from.position;

        Vector2 moveDirection = (Mathf.Abs(offset.x) > Mathf.Abs(offset.y)) ?
            new Vector2(0.0f, Mathf.Sign(offset.y)) :
            new Vector2(Mathf.Sign(offset.x), 0.0f);

        movement.SetDirection(offset);

        if (Vector2.Dot(moveDirection, offset) < targetOffset)
        {
            return Vector2.zero;
        }
        else
        {
            return moveDirection;
        }
    }

    public override IState UpdateState(float deltaTime)
    {
        if (target != null)
        {
            Vector2 offset = target.transform.position - transform.position;
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, offset, offset.magnitude, raycastLayers);

            if (hit2D != null)
            {
                Vector2 targetVelocity = TargetVelocity(target, transform, movement, targetOffset) * moveSpeed;
                movement.TargetVelocity = targetVelocity;

                if (targetVelocity == Vector2.zero && movement.Velocity == Vector2.zero)
                {
                    return shootState;
                }

                return null;
            }
            else
            {
                target = null;
            }
        }

        return lostTargetState;
    }

    public override void StateEnd()
    {
        movement.UnlockRotation();
    }
}
