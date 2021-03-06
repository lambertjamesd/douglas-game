﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingEdge
{
    public PathfindingNode to;
    public Rect edge;

    public PathfindingEdge(PathfindingNode to, Rect edge)
    {
        this.to = to;
        this.edge = edge;
    }

    public override bool Equals(object obj)
    {
        return to.Equals(obj);
    }

    public override int GetHashCode()
    {
        return to.GetHashCode();
    }
}

public class PathfindingNode {
    private Vector2 cellSize;
    private Rect area;
    private HashSet<PathfindingEdge> adjacentNodes = new HashSet<PathfindingEdge>();

    public PathfindingNode(Vector2 cellSize, Rect area)
    {
        this.cellSize = cellSize;
        this.area = area;
    }

    public HashSet<PathfindingEdge> GetAdjacentNodes()
    {
        return adjacentNodes;
    }

    public Rect Area
    {
        get
        {
            return area;
        }
    }

    public void CheckHorizontalCombine(PathfindingGrid grid)
    {
        PathfindingNode test = grid.CellAt(LeftProbePos());

        while (test != null && test.area.height == area.height)
        {
            test.ReplaceWith(this, grid);
            area.width += test.area.width;

            test = grid.CellAt(LeftProbePos());
        }
    }

    public void CheckVerticalCombine(PathfindingGrid grid)
    {
        PathfindingNode test = grid.CellAt(TopProbePos());

        while (test != null && test.area.width == area.width)
        {
            test.ReplaceWith(this, grid);
            area.height += test.area.height;

            test = grid.CellAt(TopProbePos());
        }
    }

    private Vector2 LeftProbePos()
    {
        return new Vector2(area.xMax + cellSize.x * 0.5f, area.yMin + cellSize.y * 0.5f);
    }

    private Vector2 TopProbePos()
    {
        return new Vector2(area.xMin + cellSize.x * 0.5f, area.yMax + cellSize.y * 0.5f);
    }

    private void ReplaceWith(PathfindingNode with, PathfindingGrid grid)
    {
        for (float y = area.yMin + cellSize.y * 0.5f; y < area.yMax; y += cellSize.y)
        {
            for (float x = area.xMin + cellSize.x * 0.5f; x < area.xMax; x += cellSize.x)
            {
                grid.WriteCell(new Vector2(x, y), with);
            }
        }
    }

    public static Rect Intersection(Rect a, Rect b)
    {
        float minX = Mathf.Max(a.xMin, b.xMin);
        float maxX = Mathf.Min(a.xMax, b.xMax);
        float minY = Mathf.Max(a.yMin, b.yMin);
        float maxY = Mathf.Min(a.yMax, b.yMax);
        return new Rect(new Vector2(minX, minY), new Vector2(Mathf.Max(0.0f, maxX - minX), Mathf.Max(0.0f, maxY - minY)));
    }

    public void FindConnections(PathfindingGrid grid)
    {
        foreach (Vector2 pos in AdjacentCells())
        {
            PathfindingNode test = grid.CellAt(pos);
            
            if (test != null)
            {
                adjacentNodes.Add(new PathfindingEdge(test, Intersection(Area, test.Area)));
            }
        }
    }

    public IEnumerable<Vector2> AdjacentCells()
    {
        float above = area.yMax + cellSize.y * 0.5f;
        float below = area.yMin - cellSize.y * 0.5f;
        for (float x = area.xMin + cellSize.x * 0.5f; x < area.xMax; x += cellSize.x)
        {
            yield return new Vector2(x, above);
            yield return new Vector2(x, below);
        }

        float right = area.xMax + cellSize.x * 0.5f;
        float left = area.xMin - cellSize.x * 0.5f;
        for (float y = area.yMin + cellSize.y * 0.5f; y < area.yMax; y += cellSize.y)
        {
            yield return new Vector2(right, y);
            yield return new Vector2(left, y);
        }
    }
}
