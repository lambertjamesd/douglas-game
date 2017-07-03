using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PathingTypes
{
    Walking,
    Flying,
}

[System.Serializable]
public class PathfindingLayer
{
    public PathfindingLayer(PathingTypes name, LayerMask collisionMask)
    {
        this.name = name;
        this.collisionMask = collisionMask;
    }

    public PathingTypes name;
    public LayerMask collisionMask;
}

public class Pathfinding : MonoBehaviour
{
    public int width;
    public int height;
    public Vector2 tileSize;
    public List<PathfindingLayer> layers = new List<PathfindingLayer>();

    private Dictionary<PathingTypes, PathfindingGrid> grids = new Dictionary<PathingTypes, PathfindingGrid>();
    private List<ProjectileCover> coverOptions = new List<ProjectileCover>();

    public Vector3[] FindPath(PathingTypes layerName, Vector3 from, Vector3 to)
    {
        if (grids.ContainsKey(layerName))
        {
            PathfindingState state = new PathfindingState(grids[layerName]);
            state.Start(WorldToLocal(from));
            Vector2[] result = state.PathTo(WorldToLocal(to));

            if (result != null)
            {
                return result.Select(input => LocalToWorld(input)).ToArray();
            }
        }

        return null;
    }

    public Vector3[] FindPath(PathingTypes layerName, Vector3 from, IEnumerable<Vector3> to)
    {
        if (grids.ContainsKey(layerName))
        {
            PathfindingState state = new PathfindingState(grids[layerName]);
            state.Start(WorldToLocal(from));
            Vector2[] result = state.PathToNearest(to.Select(WorldToLocal));

            if (result != null)
            {
                return result.Select(input => LocalToWorld(input)).ToArray();
            }
        }

        return null;
    }

    public IEnumerable<Vector3> FindCover(Vector3 from, float radius)
    {
        Vector2 fromLocal = WorldToLocal(from);
        foreach (ProjectileCover cover in coverOptions)
        {
            if (Vector2.Dot(cover.V1 - fromLocal, cover.Normal) > 0.0f)
            {
                yield return LocalToWorld(cover.GetCover(radius));
            }
        }
    }

    public void Start()
    {
        Vector3 position = transform.position;

        foreach (PathfindingLayer layer in layers)
        {
            PathfindingGrid grid = new PathfindingGrid(width, height, tileSize);

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    Vector2 origin = new Vector2(x * tileSize.x + position.x, (y - height) * tileSize.y + position.y);
                    if (Physics2D.OverlapArea(origin, origin + tileSize, layer.collisionMask) == null)
                    {
                        grid.MarkPassible(x, y);
                    }
                }
            }
            grid.BuildIndex();
            grids[layer.name] = grid;
        }

        if (grids.ContainsKey(PathingTypes.Walking) && grids.ContainsKey(PathingTypes.Flying))
        {
            coverOptions = CoverFinder.FindCover(grids[PathingTypes.Walking], grids[PathingTypes.Flying]);
        }
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
        foreach (PathfindingGrid grid in grids.Values)
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

        foreach (ProjectileCover cover in coverOptions)
        {
            Vector3 a = LocalToWorld(cover.V1 + cover.Normal * 0.25f);
            Vector3 b = LocalToWorld(cover.V2 + cover.Normal * 0.25f);

            Gizmos.DrawLine(a, b);
        }
    }
}
