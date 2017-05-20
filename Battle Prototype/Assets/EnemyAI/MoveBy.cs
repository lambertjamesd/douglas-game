using UnityEngine;
using System.Collections;

public class MoveBy : State {

	public Vector2 offset = Vector2.zero;
	public float moveSpeed = 1.0f;
	public DefaultMovement movement;
	public State nextState;
	private float timer = 0.0f;
	
	public override void StateBegin() {
		movement.TargetVelocity = offset.normalized * moveSpeed;
		timer = offset.magnitude / moveSpeed;
	}
	
	public override IState UpdateState(float deltaTime) {
		if (timer >= 0.0) {
			timer -= deltaTime;
			return null;
		} else {
			movement.TargetVelocity = Vector2.zero;
			return nextState;
		}
	}
}

