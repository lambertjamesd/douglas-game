using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindSearchNodeComparator : IComparer<PathfindingSearchNode>
{
    public int Compare(PathfindingSearchNode x, PathfindingSearchNode y)
    {
        if (x.currentDistance > y.currentDistance)
        {
            return 1;
        }
        else if (y.currentDistance > x.currentDistance)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}

public class PathfindingSearchNode : IComparable<PathfindingSearchNode>
{
    public PathfindingNode node;
    public Vector2 startPoint;
    public PathfindingSearchNode backNode;
    public float currentDistance;

    public PathfindingSearchNode(PathfindingNode node, Vector2 startPoint, PathfindingSearchNode backNode, float currentDistance)
    {
        this.node = node;
        this.startPoint = startPoint;
        this.backNode = backNode;
        this.currentDistance = currentDistance;
    }

    public int CompareTo(PathfindingSearchNode other)
    {
        if (currentDistance < other.currentDistance)
        {
            return -1;
        }
        else if (currentDistance > other.currentDistance)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}

public class PathfindingState
{
    public PriorityQueue<PathfindingSearchNode> nextNodes = new PriorityQueue<PathfindingSearchNode>();
    public HashSet<PathfindingNode> expandedNodes = new HashSet<PathfindingNode>();
    public PathfindingGrid grid;

    public PathfindingState(PathfindingGrid grid)
    {
        this.grid = grid;
    }

    public void Start(Vector2 location)
    {
        nextNodes.Enqueue(new PathfindingSearchNode(grid.CellAt(location), location, null, 0.0f));
    }

    public Vector2[] BuildPath(Vector2 endPoint, PathfindingSearchNode node)
    {
        int length = 1;

        PathfindingSearchNode current = node;

        while (current != null)
        {
            ++length;
            current = current.backNode;
        }

        Vector2[] result = new Vector2[length];
        int index = length - 1;
        result[index] = endPoint;

        current = node;

        while (current != null)
        {
            --index;
            result[index] = current.startPoint;
            current = current.backNode;
        }

        return result;
    }

    public Vector2[] PathTo(Vector2 end)
    {
        PathfindingNode endNode = grid.CellAt(end);

        while (nextNodes.Count > 0)
        {
            PathfindingSearchNode next = ExpandNext();

            if (next != null && next.node == endNode)
            {
                return BuildPath(end, next);
            }
        }

        return null;
    }

    public Vector2 NearestPoint(Rect area, Vector2 point)
    {
        return new Vector2(Mathf.Clamp(point.x, area.xMin, area.xMax), Mathf.Clamp(point.y, area.yMin, area.yMax));
    }

    public float Distance(Vector2 a, Vector2 b)
    {
        Vector2 offset = b - a;

        float minOffset = Mathf.Min(Mathf.Abs(offset.x), Mathf.Abs(offset.y));
        float maxOffset = Mathf.Max(Mathf.Abs(offset.x), Mathf.Abs(offset.y));

        return (maxOffset - minOffset) + Mathf.Sqrt(minOffset * minOffset * 2);
    }

    public PathfindingSearchNode ExpandNext()
    {
        PathfindingSearchNode next = nextNodes.Dequeue();

        if (expandedNodes.Contains(next.node))
        {
            return null;
        }

        expandedNodes.Add(next.node);

        foreach (PathfindingEdge edge in next.node.GetAdjacentNodes())
        {
            if (!expandedNodes.Contains(edge.to))
            {
                Vector2 nextPoint = NearestPoint(edge.edge, next.startPoint);
                float distance = Distance(nextPoint, next.startPoint);

                nextNodes.Enqueue(new PathfindingSearchNode(edge.to, nextPoint, next, next.currentDistance + distance));
            }
        }

        return next;
    }
}