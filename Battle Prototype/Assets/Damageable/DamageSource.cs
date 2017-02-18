using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class DamageInfo
{
	public float amount = 1.0f;
	public int damageBitmask = 0x1;
	public float knockbackDistance = 0.0f;
}

public class DamageSource
{
	private Vector2 direction;
	private DamageInfo damageInfo;

	public DamageSource(DamageInfo damageInfo, Vector2 direction)
	{
		this.damageInfo = damageInfo;
		this.direction = direction;
	}

	public float Amount {
		get {
			return damageInfo.amount;
		}
	}

	public Vector2 Direction {
		get {
			return direction;
		}
	}

	public int DamageBitmask {
		get {
			return damageInfo.damageBitmask;
		}
	}

	public float KnockbackDistance {
		get {
			return damageInfo.knockbackDistance;
		}
	}
}

