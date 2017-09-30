using UnityEngine;
using System.Collections;

public class Bow : MonoBehaviour {
	public void FireSingle(Vector2 velocity, Projectile projectile) {
		Projectile currentProjectile = Instantiate(projectile);
		currentProjectile.transform.position = transform.position;
		currentProjectile.transform.rotation = transform.rotation;
        currentProjectile.Fire(velocity);
	}

	public void Fire(float speed, Projectile projectile)
    {
		Vector3 velocity = transform.TransformDirection(Vector3.right) * speed;
		FireSingle(velocity, projectile);
	}

    public void Fire(GunStats gunStats)
    {
        Vector3 forward = transform.TransformDirection(Vector3.right) * gunStats.speed;
        float angle = -gunStats.spread * 0.5f;
        for (int i = 0; i < gunStats.shellSplitCount; ++i)
        {
            FireSingle(Quaternion.AngleAxis(angle, Vector3.back) * forward, gunStats.round);
            angle += gunStats.spread / (gunStats.shellSplitCount - 1);
        }
    }
}
