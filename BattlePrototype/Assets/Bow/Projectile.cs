using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public Rigidbody2D rigidBody;
	public DamageInfo damageInfo;
	public float maxRange;
    private Vector2 startPosition;
	private OnHit onHit;
	private bool active = false;
	private float lifeTimer;

	public delegate void OnHit(Collider2D collider, bool didDamage);

    public static Transform projectileParent;

	public void Fire(Vector2 velocity, OnHit onHit = null) {
		rigidBody.isKinematic = false;
		rigidBody.velocity = velocity;
		this.onHit = onHit;
		active = true;
		lifeTimer = maxRange / velocity.magnitude;
        startPosition = transform.position;
        transform.SetParent(projectileParent, true);
    }

	public void Update() {
		if (lifeTimer > 0.0f) {
			lifeTimer -= Time.deltaTime;

			if (lifeTimer <= 0.0f) {
				Destroy(gameObject);
			}
		}
	}

	public void ColliderCommon(Collider2D collider)
	{
		if (active && collider.GetComponent<IgnoreProjectile>() == null) {
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

	public void OnTriggerEnter2D(Collider2D collider)
	{
		ColliderCommon(collider);
	}

	public void OnCollisionEnter2D(Collision2D collision)
	{
		IgnoreProjectile ignore = collision.gameObject.GetComponent<IgnoreProjectile> ();
		if (ignore != null)
		{
			if (ignore.reflect)
			{
				Redirect(Vector2.Reflect (collision.relativeVelocity, collision.contacts[0].normal));
			}
		}
		else
		{
			ColliderCommon(collision.collider);
		}
	}

    public void Redirect(Vector2 velocity, int layer = -1)
    {
        startPosition = transform.position;
        rigidBody.velocity = velocity;
        lifeTimer = maxRange / velocity.magnitude;
        if (layer >= 0 && layer < 32)
        {
            gameObject.layer = layer;
        }
    }

    public Vector2 Velocity
    {
        get
        {
            return rigidBody.velocity;
        }
    }
}
