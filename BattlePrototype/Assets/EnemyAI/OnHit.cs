using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHit : MonoBehaviour
{
    public State onHit;
    public StateMachine stateMachine;
    public Damageable damageable;
    public DefaultMovement movement;

    public void Start()
    {
        damageable.FilterDamage((source, damageable) =>
        {
            if (movement != null)
            {
                movement.SetDirection(-source.Direction);
            }

            if (onHit != null)
            {
                stateMachine.SetCurrentState(onHit);
            }
            return source;
        }, false);
    }
}
