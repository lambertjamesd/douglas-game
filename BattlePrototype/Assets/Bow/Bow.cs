using UnityEngine;
using System.Collections;

public class Bow : MonoBehaviour {
	public Projectile projectile;

	public void FireSingle(Vector2 velocity) {
		Projectile currentProjectile = Instantiate(projectile);
		currentProjectile.transform.position = transform.position;
		currentProjectile.transform.rotation = transform.rotation;
		currentProjectile.transform.parent = transform;
        currentProjectile.Fire(velocity);
	}

	public void Fire(float speed)
    {
		Vector3 velocity = transform.TransformDirection(Vector3.right) * speed;
		FireSingle(velocity);
	}

    public void Fire(GunStats gunStats)
    {
        Vector3 forward = transform.TransformDirection(Vector3.right) * gunStats.speed;
        float angle = -gunStats.spread * 0.5f;
        for (int i = 0; i < gunStats.shellSplitCount; ++i)
        {
            FireSingle(Quaternion.AngleAxis(angle, Vector3.back) * forward);
            angle += gunStats.spread / (gunStats.shellSplitCount - 1);
        }
    }

	public void LoadProjectile(Projectile toUse) {
        projectile = toUse;
	}
}
