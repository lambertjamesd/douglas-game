using UnityEngine;
using System.Collections;

public class DefaultDeath : MonoBehaviour
{
	public Damageable target = null;

	void Start ()
	{
		if (target == null) {
			target = GetComponent<Damageable>();
		}

		if (target != null) {
			target.OnDeath(this.OnDie);
		}
	}

	void OnDie(Damageable damageable) {
		Destroy(gameObject);
	}
}

