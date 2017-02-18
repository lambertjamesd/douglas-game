using UnityEngine;
using System.Collections;

public class Bow : MonoBehaviour {
	public Projectile projectile;

	private Projectile currentProjectile = null;

	public Projectile Draw() {
		if (currentProjectile == null) {
			currentProjectile = Instantiate(projectile);
			currentProjectile.transform.position = transform.position;
			currentProjectile.transform.rotation = transform.rotation;
			currentProjectile.transform.parent = transform;
		}

		return currentProjectile;
	}

	public void Fire(float speed) {
		if (currentProjectile == null) {
			Draw();
		}

		Vector3 velocity = transform.TransformDirection(Vector3.right) * speed;
		currentProjectile.Fire(velocity);

		currentProjectile = null;
	}

	public void LoadProjectile(Projectile toUse) {
		Projectile current = Draw();

		currentProjectile = Instantiate(toUse);
		currentProjectile.transform.position = current.transform.position;
		currentProjectile.transform.rotation = current.transform.rotation;
		currentProjectile.transform.parent = current.transform.parent;
		Destroy(current.gameObject);
	}
}
