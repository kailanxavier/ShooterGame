using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector3 gridWorldSize = new (20, 1, 20);
    [SerializeField] private float nodeRadius = 0.15f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool drawGrid = true;
    [SerializeField] private float raycastHeight = 10f;

    private PathNode[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        // create grid
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new PathNode[gridSizeX, gridSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.z / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 p = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                Ray ray = new Ray(p + Vector3.up * gridWorldSize.y, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, raycastHeight, groundMask))
                {
                    p = hitInfo.point;
                }

                bool walkable = !Physics.CheckSphere(p, nodeRadius * 0.9f, obstacleMask);

                grid[x, y] = new PathNode(walkable, p, x, y);
            }
        }
    }

    public PathNode NodeFromWorldPoint(Vector3 wPos)
    {
        float percentX = Mathf.Clamp01((wPos.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((wPos.z + gridWorldSize.z / 2) / gridWorldSize.z);

        int x = Mathf.Clamp(Mathf.FloorToInt(gridSizeX * percentX), 0, gridSizeX - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(gridSizeY * percentY), 0, gridSizeY - 1);

        return grid[x, y];
    }

    public List<PathNode> GetPathNodesCloseBy(PathNode node)
    {
        var closeBy = new List<PathNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    closeBy.Add(grid[checkX, checkY]);
                }
            }
        }

        return closeBy;
    }

    public void ResetPathNodes()
    {
        if (grid == null) return;
        foreach (var node in grid)
        {
            node.gCost = int.MaxValue;
            node.hCost = 0;
            node.parent = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGrid) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, gridWorldSize);

        foreach (var node in grid)
        {
            Gizmos.color = node.walkable ? Color.white : Color.red;

            Gizmos.DrawCube(
                node.worldPos,
                new Vector3(nodeRadius * 2, 0.05f, nodeRadius * 2)
            );
        }

        if (grid == null) return;
    }

}
