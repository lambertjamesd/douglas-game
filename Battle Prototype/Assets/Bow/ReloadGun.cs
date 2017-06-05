using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadGun : State
{
    public State nextState;
    public Gun forGun;
    public Transform animationCLocation;
    private ReloadAnimation currentReloadAnimation;
    private float currentTimer = 0.0f;
    public DefaultMovement movement;
    public float moveSpeed;
    public int showCount = 0;
    public InventorySlot inventory;
    public bool isPlayer = true;

    public override void StateBegin()
    {
        if (currentReloadAnimation != null)
        {
            Destroy(currentReloadAnimation.gameObject);
        }

        currentReloadAnimation = Instantiate<ReloadAnimation>(forGun.gunStats.reloadAnimation);
        currentReloadAnimation.transform.SetParent(animationCLocation, false);
        currentReloadAnimation.transform.localPosition = Vector3.zero;
        currentReloadAnimation.SetShotCount(forGun.shotsLeft);
        currentTimer = forGun.gunStats.reloadDelay;
    }

    public override IState UpdateState(float deltaTime)
    {
        State useItem = isPlayer ? inventory.checkUseItems() : null;

        if (useItem != null)
        {
            return useItem;
        }
        else if (forGun.shotsLeft < forGun.gunStats.capacity && inventory.GetAmmoCount(forGun.gunStats.type) > 0)
        {
            if (isPlayer)
            {
                float horz = Input.GetAxisRaw("Horizontal");
                float vert = Input.GetAxisRaw("Vertical");

                movement.TargetVelocity = new Vector2(horz, vert).normalized * moveSpeed;
            }

            if (currentTimer > 0.0f)
            {
                currentTimer -= deltaTime;
            }
            else
            {
                ++forGun.shotsLeft;
                inventory.GiveAmmo(forGun.gunStats.type, -1);
                currentReloadAnimation.SetShotCount(forGun.shotsLeft);
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
        StartCoroutine(HideReloadAnimation(currentReloadAnimation));
        currentReloadAnimation = null;
    }

    IEnumerator HideReloadAnimation(ReloadAnimation animation)
    {
        float timer = 0.5f;

        while (timer > 0.0f && animation != null)
        {
            animation.SetShotCount(forGun.shotsLeft);
            timer -= Time.deltaTime;
            yield return null;
        }
        
        if (animation != null)
        {
            Destroy(animation.gameObject);
        }
    }
}
