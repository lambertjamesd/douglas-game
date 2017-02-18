using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour {

	public Damageable damageable;
	public GameObject trunk;

	// Use this for initialization
	void Start () {
		damageable.OnDeath(treeDied);
		damageable.FilterDamage(onHit, false);
	}

	void treeDied(Damageable damageable) {
		Destroy(trunk);
		Destroy(damageable);
	}

	DamageSource onHit(DamageSource source, Damageable damageable) {

		return source;
	}
}
