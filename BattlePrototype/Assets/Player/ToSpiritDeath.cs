using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToSpiritDeath : MonoBehaviour {
    public Damageable target = null;
    public PlayerMovement player = null;

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
        StoryManager.GetSingleton().SetBoolean("player_is_dead", true);
        target.CurrentHealth = target.MaxHealth;
        StartCoroutine(DeathSequence.Start());
    }
}
