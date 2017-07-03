using UnityEngine;
using System.Collections;

[System.Serializable]
public class PatrolParameters
{
    public RandomRange moveDistance;
    public RandomRange pauseTime;
    public RandomBoolean leftProbability;
    public RandomBoolean rightProbability;

    public float patrolSpeed;
}

public class Patrol : State {
	public State onSight;
	public Sight vision;
	public DefaultMovement movement;

    public PatrolParameters patrolParameters;
	public Vector2 direction = Vector2.right;

	private IEnumerator currentPatrol = null;

	public override IState UpdateState(float deltaTime) {
		if (currentPatrol == null) {
            if (patrolParameters.leftProbability.GenerateValue())
            {
				direction = new Vector2(direction.y, -direction.x);
            }
            else if (patrolParameters.rightProbability.GenerateValue())
            {
				direction = new Vector2(-direction.y, direction.x);
			} else {
				direction = -direction;
			}

            currentPatrol = AsyncUtil.HandleNested(PatrolSegment(movement, direction, patrolParameters));
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

		return null;
	}

    public static IEnumerator PatrolUntilSight(DefaultMovement movement, PatrolParameters parameters, Sight sight)
    {
        return AsyncUtil.Race(new IEnumerator[] {
            PatrolForever(movement, parameters),
            AsyncUtil.WaitUntil(() => sight.GetVisibleObject() != null),
        });
    }

    public static IEnumerator PatrolForever(DefaultMovement movement, PatrolParameters parameters)
    {
        Vector2 direction = Vector2.right;

        while (true)
        {
            if (parameters.leftProbability.GenerateValue())
            {
                direction = new Vector2(direction.y, -direction.x);
            }
            else if (parameters.rightProbability.GenerateValue())
            {
                direction = new Vector2(-direction.y, direction.x);
            }
            else
            {
                direction = -direction;
            }

            yield return PatrolSegment(movement, direction, parameters);
        }
    }

    public static IEnumerator PatrolSegment(DefaultMovement movement, Vector2 direction, PatrolParameters parameters)
    {
        movement.TargetVelocity = direction * parameters.patrolSpeed;

        float moveTime = parameters.moveDistance.GenerateValue() / parameters.patrolSpeed;

        yield return AsyncUtil.Pause(moveTime);

		movement.TargetVelocity = Vector2.zero;

        yield return AsyncUtil.Pause(parameters.pauseTime.GenerateValue());
	}
}
