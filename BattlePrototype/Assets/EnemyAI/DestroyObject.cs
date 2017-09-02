using UnityEngine;
using System.Collections;

public class DestroyObject : State {
	public override void StateBegin() {
		Destroy(gameObject);
	}
}
