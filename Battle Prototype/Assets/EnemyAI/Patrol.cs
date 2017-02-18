using UnityEngine;
using System.Collections;

public class Patrol : State {
	public State onSight;
	public Sight vision;
	public DefaultMovement movement;

	public RandomRange moveDistance;
	public RandomRange pauseTime;
	public RandomBoolean leftProbability;
	public RandomBoolean rightProbability;

	public float patrolSpeed;
	public Vector2 direction = Vector2.right;

	private IEnumerator currentPatrol = null;

	private float currentTime = 0.0f;

	public override State UpdateState(float deltaTime) {
		if (currentPatrol == null) {
			if (leftProbability.GenerateValue()) {
				direction = new Vector2(direction.y, -direction.x);
			} else if (rightProbability.GenerateValue()) {
				direction = new Vector2(-direction.y, direction.x);
			} else {
				direction = -direction;
			}

			currentPatrol = patrol();
		}

		if (!currentPatrol.MoveNext()) {
			currentPatrol = null;
		}

		if (vision != null) {
			Collider2D target = vision.GetVisibleObject();

			if (target != null) {
				return onSight;
			}
		}

		currentTime += deltaTime;

		return null;
	}

	private IEnumerator patrol() {
		movement.TargetVelocity = direction * patrolSpeed;

		float startTime = currentTime;
		float moveTime = moveDistance.GenerateValue() / patrolSpeed;

		while (startTime + moveTime > currentTime) {
			yield return null;
		}

		movement.TargetVelocity = Vector2.zero;
		
		startTime = currentTime;
		moveTime = pauseTime.GenerateValue();
		
		while (startTime + moveTime > currentTime) {
			yield return null;
		}
	}
}
