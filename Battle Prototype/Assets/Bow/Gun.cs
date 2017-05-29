using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GunStats
{
    public float speed;
    public int capacity;
    public float reloadDelay;
    public float reloadBulletDuration;
}

public class Gun : State
{
    public State nextState;
    public Bow bow;
    public GunStats gunStats;
    public int shotsLeft;

    public override IState UpdateState(float deltaTime)
    {
        if (shotsLeft > 0)
        {
            bow.Fire(gunStats.speed);
            --shotsLeft;
        }
        return nextState;
    }
}
