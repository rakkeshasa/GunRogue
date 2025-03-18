using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPosition;
    public int gCost = 0; // 출발점부터의 거리
    public int hCost = 0; // 도착점까지의 거리
    public Node parentNode;

    public int fCost { get { return gCost + hCost; } }

    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        parentNode = null;
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }

        return compare;
    }
}
