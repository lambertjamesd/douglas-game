using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DamageListener : System.IDisposable
{
    Damageable target;
    Damageable.DamageFilter filter;

    public DamageListener(Damageable target, Damageable.DamageFilter filter, bool highPriority)
    {
        this.target = target;
        this.filter = filter;

        target.FilterDamage(filter, highPriority);
    }

    public void Dispose()
    {
        target.UnfilterDamage(this.filter);
    }
}

public class Damageable : MonoBehaviour {
	public float maxHealth;
	public int damageBitmask = 0x1;
	private float currentHealth;
	public bool immortal = false;

	public delegate DamageSource DamageFilter(DamageSource source, Damageable damageable);
	public delegate void DeathAlert(Damageable damageable);

	private List<DamageFilter> damageFilters = new List<DamageFilter>();
	private List<DeathAlert> deathListeners = new List<DeathAlert>();

	public void Awake() {
		currentHealth = maxHealth;
	}

	public float CurrentHealth 
    {
		get 
        {
			return currentHealth;
		}
        set
        {
            currentHealth = value;
        }
	}

	public float MaxHealth 
    {
		get 
        {
			return maxHealth;
		}
	}

    public bool IsDead
    {
        get
        {
            return currentHealth <= 0.0f;
        }
    }

	public bool Damage(DamageSource source) 
    {
		bool result = false;

		if ((source.DamageBitmask & damageBitmask) != 0) 
        {
			for (int i = 0; i < damageFilters.Count && source != null; ++i) 
            {
				source = damageFilters[i](source, this);
			}

			if (source != null) 
            {
				currentHealth -= source.Amount;
				result = true;
			}

			if (currentHealth <= 0.0f && !immortal) 
            {
				for (int i = 0; i < deathListeners.Count; ++i) 
                {
					deathListeners[i](this);
				}

				deathListeners.Clear();
			}

		}

		return result;
	}

	public DamageFilter FilterDamage(DamageFilter filter, bool highPriority) {
		if (highPriority) {
			this.damageFilters.Insert(0, filter);
		} else {
			this.damageFilters.Add(filter);
		}
		return filter;
	}

	public void UnfilterDamage(DamageFilter filter) {
		this.damageFilters.Remove(filter);
	}

    public DeathAlert OnDeath(DeathAlert alert) {
		this.deathListeners.Add(alert);
		return alert;
	}

	public void OffDeath(DeathAlert alert) {
		this.deathListeners.Remove(alert);
	}

}
