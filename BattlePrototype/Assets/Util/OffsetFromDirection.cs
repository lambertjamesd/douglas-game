using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetFromDirection : MonoBehaviour {
    public Vector3[] offsetFromDirection = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };

    public Vector3[] directions = new Vector3[] { Vector3.down, Vector3.left, Vector3.back, Vector3.right };

    public Vector3 GetOffset()
    {
        int index = -1;
        float amount = float.MinValue;

        for (int i = 0; i < offsetFromDirection.Length; ++i)
        {
            float current = Vector3.Dot(transform.TransformDirection(Vector3.down), directions[i]);

            if (current > amount)
            {
                index = i;
                amount = current;
            }
        }

        return offsetFromDirection[index];
    }

    public void Update()
    {
        transform.localPosition = GetOffset();
    }
}
