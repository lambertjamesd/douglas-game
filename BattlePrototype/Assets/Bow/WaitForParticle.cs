using UnityEngine;
using System.Collections;

public class WaitForParticle : MonoBehaviour {

	public ParticleSystem target;
	public bool destroyOnFinish = true;
	
	// Update is called once per frame
	void Update () {
		if (target != null && !target.IsAlive() && destroyOnFinish) {
			Destroy(gameObject);
		}
	}
}
