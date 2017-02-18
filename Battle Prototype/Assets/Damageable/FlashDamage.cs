using UnityEngine;
using System.Collections;

public class FlashDamage : MonoBehaviour {
	public float flashTime = 1.0f;
	public Damageable damage;
	public Flasher flasher;

	// Use this for initialization
	void Start () {
		Flasher flasher = GetComponent<Flasher>();

		if (flasher != null) {
			damage.FilterDamage((source, damageable) => {
				StartCoroutine(flash());
				return source;
			}, false);
		}
	}

	IEnumerator flash() {
		flasher.enabled = true;
		yield return new WaitForSeconds(flashTime);
		
		if (flasher != null) {
			flasher.enabled = false;
		}
	}
}
