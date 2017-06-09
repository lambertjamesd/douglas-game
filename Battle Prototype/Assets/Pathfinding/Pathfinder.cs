﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public Pathfinding pathfinding;
    public DefaultMovement movement;
    public float moveSpeed = 1.0f;
    public float radius = 0.5f;

    private Vector3[] currentWaypoints;
    private int currentWaypointIndex;

    public void PathTo(Vector3 position)
    {
        if (pathfinding != null)
        {
            currentWaypoints = pathfinding.FindPath(transform.position, position);
            currentWaypointIndex = 0;
        }
    }

    void Update()
    {
        if (IsActive)
        {
            Vector2 offset = currentWaypoints[currentWaypointIndex] - transform.position;

            if (offset.sqrMagnitude < radius * radius)
            {
                ++currentWaypointIndex;
                
                if (IsActive)
                {
                    movement.TargetVelocity = Vector2.zero;
                }
            }
            else
            {
                movement.TargetVelocity = moveSpeed * offset.normalized;
            }
        }
    }

    public bool IsActive
    {
        get
        {
            return currentWaypoints != null && currentWaypointIndex < currentWaypoints.Length;
        }
    }
}