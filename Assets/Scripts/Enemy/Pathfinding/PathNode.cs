using UnityEngine;

public class PathNode
{
    public bool walkable;
    public Vector3 worldPos;
    public int gridX, gridY;
    public int gCost, hCost;
    public PathNode parent;

    public int regionId = -1;
    public bool visited = false;

    public int fCost => gCost + hCost;

    public PathNode(bool walkable, Vector3 worldPos, int x, int y)
    {
        this.walkable = walkable;
        this.worldPos = worldPos;
        gridX = x;
        gridY = y;
    }
}
