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
        if (GetShotsLeft() > 0)
        {
            bow.LoadProjectile(gunStats.round);
            bow.Fire(gunStats.speed);
            SetShotsLeft(GetShotsLeft() - 1);
        }
        return nextState;
    }
}
