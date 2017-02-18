using UnityEngine;
using System.Collections;

public class PhysicsUtil {
	static float GRAVITY = -9.8f;
	static float Z_Y_RATIO = 0.5f;
	
	public static IEnumerator MoveInLine(Transform target, Vector3 endPosition, float time) {
		float startTime = Time.time;
		Vector3 startPosition = target.position;
		
		while (Time.time < startTime + time) {
			target.position = Vector3.Lerp(startPosition, endPosition, (Time.time - startTime) / time);
			yield return null;
		}
		
		target.position = endPosition;
	}

	public static IEnumerator MoveInArc(Transform target, Vector3 endPosition, float time, bool useLocal = false) {
		float startTime = Time.time;
		Vector3 startPosition = useLocal ? target.localPosition : target.position;
		float startVelocity = InitialVelocity(time, endPosition.z - startPosition.z);

		while (Time.time < startTime + time) {
			float height = HeightAtTime(startPosition.z, startVelocity, Time.time - startTime);
			Vector3 position = Vector3.Lerp(startPosition, endPosition, (Time.time - startTime) / time) + new Vector3(0.0f, height * Z_Y_RATIO, height);
			if (useLocal) {
				target.localPosition = position;
			} else {
				target.position = position;
			}
			yield return null;
		}

		if (useLocal) {
			target.localPosition = endPosition;
		} else {
			target.position = endPosition;
		}
	}

	public static float HeightAtTime(float startHeight, float startVelocity, float time) {
		return startHeight + startVelocity * time + 0.5f * GRAVITY * time * time;
	}

	public static float InitialVelocity(float hangTime, float heightChange) {
		return (heightChange - 0.5f * GRAVITY * hangTime * hangTime) / hangTime;
	}
}