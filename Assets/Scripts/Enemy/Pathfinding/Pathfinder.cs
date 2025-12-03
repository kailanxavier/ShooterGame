using UnityEngine;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour
{
    private GridManager grid;

    private void Awake()
    {
        grid = FindFirstObjectByType<GridManager>();
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        grid.ResetPathNodes();

        var path = new List<Vector3>();
        PathNode startNode = grid.NodeFromWorldPoint(startPos);
        PathNode targetNode = grid.NodeFromWorldPoint(targetPos);

        var openSet = new List<PathNode> { startNode };
        var closedSet = new HashSet<PathNode>();

        while (openSet.Count > 0)
        {
            PathNode current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < current.fCost || (openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost))
                {
                    current = openSet[i];
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == targetNode)
            {
                path = RetracePath(startNode, targetNode);
                break;
            }

            foreach (PathNode closeBy in grid.GetPathNodesCloseBy(current))
            {
                if (!closeBy.walkable || closedSet.Contains(closeBy)) continue;

                int newCost = current.gCost + GetDistance(current, closeBy);
                if (newCost < closeBy.gCost || !openSet.Contains(closeBy))
                {
                    closeBy.gCost = newCost;
                    closeBy.hCost = GetDistance(closeBy, targetNode);
                    closeBy.parent = current;

                    if (!openSet.Contains(closeBy))
                    {
                        openSet.Add(closeBy);
                    }
                }
            }
        }
        return path;
    }


    private List<Vector3> RetracePath(PathNode start, PathNode end)
    {
        var path = new List<Vector3>();
        PathNode current = end;

        while (current != start)
        {
            path.Add(current.worldPos);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    private int GetDistance(PathNode a, PathNode b)
    {
        int root2Times10 = 14;
        int squareScale = 10;

        int distX = Mathf.Abs(a.gridX - b.gridX);
        int distY = Mathf.Abs(a.gridY - b.gridY);

        return root2Times10 * Mathf.Min(distX, distY) + squareScale * Mathf.Abs(distX - distY);
    }
}
