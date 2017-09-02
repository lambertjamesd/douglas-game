using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {
    public Transform target;
    public Bounds bounds;
    public Camera useCamera;

	void LateUpdate () {
        if (target != null) {
    	    Bounds modifiedBounds = bounds;
            Vector3 expandOffset = new Vector3(-2.0f * useCamera.orthographicSize * useCamera.aspect, -2.0f * useCamera.orthographicSize, 0.0f);

            if (-expandOffset.x > modifiedBounds.size.x)
            {
                expandOffset.x = -modifiedBounds.size.x;
            }

            if (-expandOffset.y > modifiedBounds.size.y)
            {
                expandOffset.y = -modifiedBounds.size.y;
            }

            modifiedBounds.Expand(expandOffset);

            Vector3 targetPosition = modifiedBounds.ClosestPoint(target.position);
            targetPosition.z = transform.position.z;
            transform.position = targetPosition;
        }
	}
}
