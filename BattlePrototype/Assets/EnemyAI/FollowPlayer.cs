using UnityEngine;
using System.Collections;
using System.Linq;

public class FollowPlayer : State {
	public float moveSpeed = 1.0f;
	public float sightRadius;
	public bool gridMovement = false;
	public State afterState = null;
	public Sight sight = null;
	
	public LayerMask sightLayers;
	
	private DefaultMovement movement;

	public void Start() {
		movement = GetComponent<DefaultMovement>();
	}
	
	public override IState UpdateState(float deltaTime) {
		if(sight == null) {
			return null;
		}

		Collider2D currentTarget = sight.GetVisibleObject();

		if (currentTarget != null) {
			Vector3 targetDirection = currentTarget.transform.position - transform.position;

			targetDirection.z = 0.0f;

			if (gridMovement) {
				float minMove = moveSpeed * Time.deltaTime;

				targetDirection = new Vector3(
					Mathf.Sign(targetDirection.x) * Mathf.Min(Mathf.Abs(0.5f * targetDirection.x / minMove), 1.0f), 
					Mathf.Sign(targetDirection.y) * Mathf.Min(Mathf.Abs(0.5f * targetDirection.y / minMove), 1.0f), 
					0.0f
				) * moveSpeed;
			} else {
				targetDirection.Normalize();
			}

			movement.TargetVelocity = targetDirection * moveSpeed;

			return null;
		} else {
			movement.TargetVelocity = Vector2.zero;

			return afterState;
		}
	}
}
