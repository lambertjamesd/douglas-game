using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinding : MonoBehaviour
{
    public int width;
    public int height;
    public Vector2 tileSize;
    public LayerMask layers;

    private PathfindingGrid grid;

    public Vector3[] FindPath(Vector3 from, Vector3 to)
    {
        PathfindingState state = new PathfindingState(grid);
        state.Start(WorldToLocal(from));
        Vector2[] result = state.PathTo(WorldToLocal(to));

        if (result != null)
        {
            return result.Select(input => LocalToWorld(input)).ToArray();
        }
        else
        {
            return null;
        }
    }

    public void Start()
    {
        grid = new PathfindingGrid(width, height, tileSize);

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                Vector2 origin = new Vector2(x * tileSize.x, (y - height) * tileSize.y);
                if (Physics2D.OverlapArea(origin, origin + tileSize, layers) == null)
                {
                    grid.MarkPassible(x, y);
                }
            }
        }

        grid.BuildIndex();
    }

    private Vector3 LocalToWorld(Vector2 input)
    {
        return transform.TransformPoint(input.x, input.y - height * tileSize.y, 0.0f);
    }

    private Vector2 WorldToLocal(Vector3 input)
    {
        Vector2 result = transform.InverseTransformPoint(input);
        return result + new Vector2(0.0f, height * tileSize.y);
    }
    
	public void OnDrawGizmosSelected()
    {
        Color last = Gizmos.color;
        foreach (PathfindingNode node in grid.GetAllNodes())
        {
            Rect nodePos = node.Area;
            Gizmos.color = Color.cyan;
            Vector3 nodeCenter = LocalToWorld(nodePos.center); 
            Gizmos.DrawWireCube(nodeCenter, new Vector3(nodePos.width, nodePos.height, 0.0f));
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(nodeCenter, new Vector3(nodePos.width - tileSize.x, nodePos.height - tileSize.y, 0.0f));

            foreach (PathfindingEdge adjacent in node.GetAdjacentNodes())
            {
                Gizmos.DrawLine(nodeCenter, LocalToWorld(adjacent.to.Area.center));
            }
        }
        Gizmos.color = last;
    }
}
