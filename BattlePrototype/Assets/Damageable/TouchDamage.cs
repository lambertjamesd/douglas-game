using UnityEngine;
using System.Collections;

public class TouchDamage : MonoBehaviour {
	public DamageInfo damageInfo;

	public void OnCollisionEnter2D(Collision2D collision) {
		Damageable damageable =	collision.gameObject.GetComponent<Damageable>();

		if (damageable != null) {
            damageable.Damage(new DamageSource(damageInfo, -collision.contacts[0].normal, transform.position));
		}
	}
}
