using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Gun : State
{
    public State nextState;
    public Bow bow;
    public GunStats gunStats;
    public VariableStore stateStore;
    public InventorySlot inventory;
    public float lastFireTime = 0.0f;

    public int GetShotsLeft()
    {
        return gunStats.GetShotsLeft(stateStore);
    }

    public void SetShotsLeft(int value)
    {
        gunStats.SetShotsLeft(stateStore, value);
    }

    public override void StateBegin()
    {
        gunStats = inventory.GetCurrentGun() ?? gunStats;
    }

    public override IState UpdateState(float deltaTime)
    {
        if (GetShotsLeft() > 0 && lastFireTime + gunStats.shotDelay < Time.time)
        {
            bow.Fire(gunStats);
            SetShotsLeft(GetShotsLeft() - 1);
            lastFireTime = Time.time;
        }
        return nextState;
    }
}
