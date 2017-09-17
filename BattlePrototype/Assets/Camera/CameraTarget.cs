using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour {

    public Vector3 targetOffset = Vector3.zero;
    public float moveVelocity = 3.0f;
	
	// Update is called once per frame
	void Update () {
        Vector3 currentPosition = transform.localPosition;
        Vector3 offset = targetOffset - currentPosition;
        float moveAmount = moveVelocity * Time.unscaledDeltaTime;

        if (offset.sqrMagnitude < moveAmount * moveAmount)
        {
            transform.localPosition = targetOffset; 
        }
        else
        {
            transform.localPosition = transform.localPosition + offset.normalized * moveAmount;
        }
	}
}
