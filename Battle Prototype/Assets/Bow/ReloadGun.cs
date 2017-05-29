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
    public int showCount = 0;
    public InventorySlot inventory;

    public override void StateBegin()
    {
        currentAnimation.gameObject.SetActive(true);
        currentAnimation.SetShotCount(forGun.shotsLeft);
        currentTimer = forGun.gunStats.reloadDelay;
        ++showCount;
    }

    public override IState UpdateState(float deltaTime)
    {
        State useItem = inventory.checkUseItems();

        if (useItem != null)
        {
            return useItem;
        }
        else if (forGun.shotsLeft < forGun.gunStats.capacity && inventory.inventory.arrows > 0)
        {
            float horz = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");

            movement.TargetVelocity = new Vector2(horz, vert).normalized * moveSpeed;

            if (currentTimer > 0.0f)
            {
                currentTimer -= deltaTime;
            }
            else
            {
                ++forGun.shotsLeft;
                --inventory.inventory.arrows;
                currentAnimation.SetShotCount(forGun.shotsLeft);
                currentTimer = forGun.gunStats.reloadBulletDuration;
            }

            return null;
        }
        else
        {
            return nextState;
        }
    }

    public override void StateEnd()
    {
        StartCoroutine(HideReloadAnimation());
    }

    IEnumerator HideReloadAnimation()
    {
        float timer = 0.5f;

        while (timer > 0.0f)
        {
            yield return null;
            currentAnimation.SetShotCount(forGun.shotsLeft);
            timer -= Time.deltaTime;
        }

        --showCount;

        if (showCount == 0)
        {
            currentAnimation.gameObject.SetActive(false);
        }
    }
}
