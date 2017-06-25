using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverFinder {

    public static Vector2[] directions = 
    {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left
    };

    private static void FindCover(int x, int y, PathfindingGrid flyable, List<ProjectileCover> result)
    {
        foreach (Vector2 direction in directions)
        {
            if (flyable.GetCell((int)direction.x + x, (int)direction.y + y) == null)
            {
                Vector2 offset = new Vector2(-direction.y, direction.x);
                offset.x *= flyable.GetCellSize().x * 0.5f;
                offset.y *= flyable.GetCellSize().y * 0.5f;

                Vector2 center = (flyable.CellCenter(x, y) + flyable.CellCenter((int)direction.x + x, (int)direction.y + y)) * 0.5f;
                result.Add(new ProjectileCover(center + offset, center - offset));
            }
        }
    }

    public static List<ProjectileCover> FindCover(PathfindingGrid walkable, PathfindingGrid flyable)
    {
        List<ProjectileCover> result = new List<ProjectileCover>();
        for (int y = 0; y < walkable.GetHeight(); ++y)
        {
            for (int x = 0; x < walkable.GetWidth(); ++x)
            {
                if (walkable.GetCell(x, y) != null)
                {
                    FindCover(x, y, flyable, result);
                }
            }
        }
        return result;
    }
}
