using UnityEngine;

public class GridNodes
{
    private int width;
    private int height;

    // 2차원 배열 선언
    private Node[,] gridNode;

    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridNode = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Node의 위치 설정
                gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    public Node GetGridNode(int xPos, int yPos)
    {
        if (xPos < width && yPos < height)
        {
            return gridNode[xPos, yPos];
        }
        else
        {
            Debug.Log("Requested grid node is out of range");
            return null;
        }
    }
}
