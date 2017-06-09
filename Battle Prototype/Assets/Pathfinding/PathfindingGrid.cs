using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid {
    private PathfindingNode[,] nodes = null;
    private int width;
    private int height;
    private Vector2 cellSize;
    private HashSet<PathfindingNode> allNodes = new HashSet<PathfindingNode>();

    public HashSet<PathfindingNode> GetAllNodes()
    {
        return allNodes;
    }

    private void CombineCells()
    {
        for (int y = 0; y < height; ++y)
        {
            for (float x = cellSize.x * 0.5f; x < width * cellSize.x;)
            {
                PathfindingNode test = CellAt(new Vector2(x, (y + 0.5f) * cellSize.y));
                if (test == null)
                {
                    x += cellSize.x;
                }
                else
                {
                    test.CheckHorizontalCombine(this);
                    x += test.Area.width;
                }
            }
        }

        for (int y = 0; y < height; ++y)
        {
            for (float x = cellSize.x * 0.5f; x < width * cellSize.x;)
            {
                PathfindingNode test = CellAt(new Vector2(x, (y + 0.5f) * cellSize.y));
                if (test == null)
                {
                    x += cellSize.x;
                }
                else
                {
                    test.CheckVerticalCombine(this);
                    x += test.Area.width;
                }
            }
        }
    }

    private void BuildConnections()
    {
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                PathfindingNode test = nodes[x, y];

                if (test != null && !allNodes.Contains(test))
                {
                    allNodes.Add(test);
                    test.FindConnections(this);
                }
            }
        }
    }

    public void BuildIndex()
    {
        CombineCells();
        BuildConnections();
    }

    public PathfindingGrid(int width, int height, Vector2 cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        nodes = new PathfindingNode[width, height];
    }

    public PathfindingNode CellAt(Vector2 position)
    {
        int x = Mathf.FloorToInt(position.x / cellSize.x);
        int y = Mathf.FloorToInt(position.y / cellSize.y);
        
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            return null;
        }
        else
        {
            return nodes[x,y];
        }
    }

    public void WriteCell(Vector2 position, PathfindingNode node)
    {
        int x = Mathf.FloorToInt(position.x / cellSize.x);
        int y = Mathf.FloorToInt(position.y / cellSize.y);

        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            nodes[x, y] = node;
        }
    }

    public void MarkPassible(int x, int y)
    {
        nodes[x, y] = new PathfindingNode(cellSize, new Rect(x * cellSize.x, y * cellSize.y, cellSize.x, cellSize.y));
    }
}
