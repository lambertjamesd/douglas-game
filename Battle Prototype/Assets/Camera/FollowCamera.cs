using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {
    public Transform target;
    public Bounds bounds;
    public Camera useCamera;

	void LateUpdate () {
        if (target != null) {
    	    Bounds modifiedBounds = bounds;
            modifiedBounds.Expand(new Vector3(-2.0f * useCamera.orthographicSize * useCamera.aspect, -2.0f * useCamera.orthographicSize, 0.0f));

            Vector3 targetPosition = modifiedBounds.ClosestPoint(target.position);
            targetPosition.z = transform.position.z;
            transform.position = targetPosition;
        }
	}
}
