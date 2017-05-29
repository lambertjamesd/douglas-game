using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadGun : State
{
    public State nextState;
    public Gun forGun;
    public ReloadAnimation currentAnimation;
    private float currentTimer = 0.0f;
    public DefaultMovement movement;
    public float moveSpeed;

    public override void StateBegin()
    {
        currentAnimation.gameObject.SetActive(true);
        currentAnimation.SetShotCount(forGun.shotsLeft);
        currentTimer = forGun.gunStats.reloadDelay;
    }

    public override IState UpdateState(float deltaTime)
    {
        float horz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        movement.TargetVelocity = new Vector2(horz, vert).normalized * moveSpeed;

        if (forGun.shotsLeft < forGun.gunStats.capacity)
        {
            if (currentTimer > 0.0f)
            {
                currentTimer -= deltaTime;
            }
            else
            {
                ++forGun.shotsLeft;
                currentAnimation.SetShotCount(forGun.shotsLeft);
                currentTimer = forGun.gunStats.reloadBulletDuration;
            }

            return null;
        }
        else
        {
            currentAnimation.gameObject.SetActive(false);
            return nextState;
        }
    }
}
