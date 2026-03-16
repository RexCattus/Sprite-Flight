using UnityEngine;
using System.Collections.Generic;

public class MyPathGrid : MonoBehaviour
{
    public static MyPathGrid Instance { get; private set; }

    public bool is2D = true;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    Node[,] grid;
    int gridSizeX, gridSizeY;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CreateGrid();
    }

    void CreateGrid()
    {
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / (nodeRadius * 2));
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / (nodeRadius * 2));
        grid = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint;
                if (is2D)
                {
                    worldPoint = transform.position
                        - Vector3.right * gridWorldSize.x / 2
                        - Vector3.up * gridWorldSize.y / 2
                        + Vector3.right * (x * nodeRadius * 2 + nodeRadius)
                        + Vector3.up * (y * nodeRadius * 2 + nodeRadius);
                }
                else
                {
                    worldPoint = transform.position
                        - Vector3.right * gridWorldSize.x / 2
                        - Vector3.forward * gridWorldSize.y / 2
                        + Vector3.right * (x * nodeRadius * 2 + nodeRadius)
                        + Vector3.forward * (y * nodeRadius * 2 + nodeRadius);
                }

                bool walkable = CheckWalkable(worldPoint);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    bool CheckWalkable(Vector3 point)
    {
        if (is2D)
            return !Physics2D.OverlapCircle(point, nodeRadius, unwalkableMask);
        else
            return !Physics.CheckSphere(point, nodeRadius, unwalkableMask);
    }

    // Gọi hàm này nếu map thay đổi runtime (cửa mở/đóng, vật cản xuất hiện...)
    public void RefreshGrid() => CreateGrid();

    public Node GetNodeFromWorldPos(Vector3 worldPos)
    {
        float percentX = Mathf.Clamp01((worldPos.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY;

        if (is2D)
            percentY = Mathf.Clamp01((worldPos.y - transform.position.y + gridWorldSize.y / 2) / gridWorldSize.y);
        else
            percentY = Mathf.Clamp01((worldPos.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos, AStarAgent requester)
    {
        Node startNode = GetNearestWalkableNode(GetNodeFromWorldPos(startPos));
        Node targetNode = GetNearestWalkableNode(GetNodeFromWorldPos(targetPos));

        // Reset toàn bộ node trước mỗi lần tìm — fix bug gCost cũ
        foreach (Node n in grid)
        {
            n.gCost = int.MaxValue;
            n.hCost = 0;
            n.parent = null;
        }

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);

        // SortedSet thay List → O(log n) thay vì O(n)
        SortedSet<Node> openSet = new SortedSet<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.Min;
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode, requester);
                return;
            }

            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newGCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newGCost < neighbour.gCost)
                {
                    // Phải remove trước khi update để SortedSet sắp xếp lại đúng
                    openSet.Remove(neighbour);
                    neighbour.gCost = newGCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    openSet.Add(neighbour);
                }
            }
        }

        // Không tìm được path — báo lại cho agent biết
        requester.SetPath(null);
    }

    void RetracePath(Node startNode, Node endNode, AStarAgent requester)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        requester.SetPath(path);
    }

    List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neighbours.Add(grid[checkX, checkY]);
            }
        }
        return neighbours;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY) return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    // Vẽ grid trong Scene view để debug
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1f));

        if (grid == null) return;
        foreach (Node n in grid)
        {
            Gizmos.color = n.walkable ? new Color(1, 1, 1, 0.1f) : new Color(1, 0, 0, 0.4f);
            Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeRadius * 1.8f));
        }
    }
    public Node GetNearestWalkableNode(Node node)
    {
        if (node.walkable) return node;

        // Tìm node walkable gần nhất theo vòng tròn mở rộng dần
        for (int radius = 1; radius < 5; radius++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX &&
                        checkY >= 0 && checkY < gridSizeY)
                    {
                        if (grid[checkX, checkY].walkable)
                            return grid[checkX, checkY];
                    }
                }
            }
        }
        return node; // fallback
    }
}
