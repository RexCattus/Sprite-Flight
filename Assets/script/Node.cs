using UnityEngine;

public class Node : System.IComparable<Node>
{
    public bool walkable;
    public Vector3 worldPos;
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public Node parent;
    public int fCost => gCost + hCost;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        gCost = int.MaxValue; // khởi tạo để dễ kiểm tra chưa được visit
    }

    // Dùng cho SortedSet — so sánh fCost, tie-break bằng hCost
    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0) compare = hCost.CompareTo(other.hCost);
        if (compare == 0) compare = gridX.CompareTo(other.gridX);
        if (compare == 0) compare = gridY.CompareTo(other.gridY);
        return compare;
    }
}