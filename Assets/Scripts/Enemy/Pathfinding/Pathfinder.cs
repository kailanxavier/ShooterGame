using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using System.Net;

public class Pathfinder : MonoBehaviour
{
    private GridManager grid;
    //private int nodesExploredThisFrame = 0;

    private void Awake()
    {
        grid = FindFirstObjectByType<GridManager>();
    }

    //private void Update()
    //{
    //    nodesExploredThisFrame = 0;
    //}

    public PathNode FindNearestWalkableNodeInRadius(PathNode targetNode, int radius)
    {
        var nearby = grid.GetNodesAroundPlayer(targetNode, radius);

        PathNode nearest = null;
        float bestDist = float.MaxValue;

        foreach (var node in nearby)
        {
            if (!node.walkable) continue;

            float dist = Vector2.Distance(new Vector2(node.gridX, node.gridY), new Vector2(targetNode.gridX, targetNode.gridY));

            if (dist < bestDist)
            {
                bestDist = dist;
                nearest = node;
            }
        }
        return nearest;
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        grid.ResetPathNodes();

        PathNode startNode = grid.NodeFromWorldPoint(startPos);
        PathNode rawTargetNode = grid.NodeFromWorldPoint(targetPos);

        int searchRadius = 2;
        PathNode targetNode = grid.GetNearestWalkableNode(rawTargetNode, searchRadius);

        if (targetNode == null)
        {
            // no valid tiles found
            // can we return empty list?
            return new List<Vector3>();
        }

        var openSet = new List<PathNode> { startNode };
        var closedSet = new HashSet<PathNode>();
        var path = new List<Vector3>();

        while (openSet.Count > 0)
        {
            PathNode current = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                //nodesExploredThisFrame++;
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

    //private void OnGUI()
    //{
    //    GUI.enabled = true;
    //    GUIStyle gUIStyle = new GUIStyle();
    //    gUIStyle.fontSize = 48;
    //    GUI.TextArea(new Rect(0, 0, 500, 200), $" Nodes explored: {nodesExploredThisFrame}", gUIStyle);
    //}
}
