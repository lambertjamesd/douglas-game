using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public Rigidbody2D rigidBody;
	public DamageInfo damageInfo;
	public float maxRange = 50.0f;
    private Vector2 startPosition;
	private OnHit onHit;
	private bool active = false;
	private float lifeTimer;

	public delegate void OnHit(Collider2D collider, bool didDamage);

	public void Fire(Vector2 velocity, OnHit onHit = null) {
		rigidBody.isKinematic = false;
		rigidBody.velocity = velocity;
		this.onHit = onHit;
		transform.parent = null;
		active = true;
		lifeTimer = maxRange / velocity.magnitude;
        startPosition = transform.position;

    }

	public void Update() {
		if (lifeTimer > 0.0f) {
			lifeTimer -= Time.deltaTime;

			if (lifeTimer <= 0.0f) {
				Destroy(gameObject);
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D collider) {
		if (active) {
			active = false;
			Damageable damageable = collider.gameObject.GetComponent<Damageable>();

			bool didDamage = false;

			if (damageable != null) {
				didDamage = damageable.Damage(new DamageSource(damageInfo, transform.TransformDirection(Vector3.right), startPosition));
			}

			if (onHit != null) {
				this.onHit(collider, didDamage);
			}

			Destroy(gameObject);
		}
	}
}
