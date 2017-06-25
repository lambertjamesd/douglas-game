﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToSpiritDeath : MonoBehaviour {
    public Damageable target = null;

    void Start()
    {
        if (target == null)
        {
            target = GetComponent<Damageable>();
        }

        if (target != null)
        {
            target.OnDeath(this.OnDie);
        }
    }

    void OnDie(Damageable damageable)
    {
        target.CurrentHealth = target.MaxHealth;
        GameObject.FindGameObjectWithTag("GameController").GetComponent<WorldController>().SwitchTo(MapDirections.Dead);
    }
}