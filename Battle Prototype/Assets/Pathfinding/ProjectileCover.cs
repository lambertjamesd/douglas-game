using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCover {
    private Vector2 v1;
    private Vector2 v2;
    private Vector2 normal;

    public ProjectileCover(Vector2 v1, Vector2 v2)
    {
        this.v1 = v1;
        this.v2 = v2;

        normal = new Vector2(-(v1.y - v2.y), v1.x - v2.x).normalized;
    }

    public Vector2 GetCover(float radius)
    {
        return (v1 + v2) * 0.5f + normal * radius;
    }

    public Vector2 Normal
    {
        get
        {
            return normal;
        }
    }

    public Vector2 V1
    {
        get
        {
            return v1;
        }
    }
}
