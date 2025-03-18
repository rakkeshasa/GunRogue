using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AStar
{
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPos, Vector3Int endGridPos)
    {
        // 방의 좌하단을 기준으로 좌표를 재측정
        startGridPos -= (Vector3Int)room.templateLowerBounds;
        endGridPos -= (Vector3Int)room.templateLowerBounds;

        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeSet = new HashSet<Node>();

        int roomWidth = room.templateUpperBounds.x - room.templateLowerBounds.x;
        int roomHeight = room.templateUpperBounds.y - room.templateLowerBounds.y;
        GridNodes gridNodes = new GridNodes(roomWidth + 1, roomHeight + 1);

        Node startNode = gridNodes.GetGridNode(startGridPos.x, startGridPos.y);
        Node targetNode = gridNodes.GetGridNode(endGridPos.x, endGridPos.y);

        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeSet, room.instantiatedRoom);

        if (endPathNode != null)
        {
            return CreatePathStack(endPathNode, room);
        }

        return null;
    }

    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeSet, InstantiatedRoom room)
    {
        openNodeList.Add(startNode);

        while (openNodeList.Count > 0)
        {
            // 우선순위 정렬
            /*openNodeList.Sort();
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);*/

            // Min을 활용한 A*
            Node currentNode = openNodeList.Min();
            int currentNodeIdx = openNodeList.IndexOf(currentNode);
            openNodeList.RemoveAt(currentNodeIdx);

            if (currentNode == targetNode)
            {
                return currentNode;
            }

            closedNodeSet.Add(currentNode);
            CheckNearNodes(currentNode, targetNode, gridNodes, openNodeList, closedNodeSet, room);
        }

        return null;
    }

    private static Stack<Vector3> CreatePathStack(Node targetNode, Room room)
    {
        Stack<Vector3> pathStack = new Stack<Vector3>();
        Node nextNode = targetNode; // 마지막 자식 노드(도착지)

        // Grid 셀의 정중앙 추출
        Vector3 midPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        midPoint.z = 0f;

        // 백트래킹
        while(nextNode != null)
        {
            int nodeWorldPosX = nextNode.gridPosition.x + room.templateLowerBounds.x;
            int nodeWorldPosY = nextNode.gridPosition.y + room.templateLowerBounds.y;
            Vector3 worldPos = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(nodeWorldPosX, nodeWorldPosY, 0));
            worldPos += midPoint;
            
            pathStack.Push(worldPos);

            nextNode = nextNode.parentNode;
        }

        return pathStack;
    }

    private static void CheckNearNodes(Node currentNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeSet, InstantiatedRoom room)
    {
        Vector2Int currentNodePos = currentNode.gridPosition;
        Node nearNode;

        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                // 자기 자신의 경우
                if (i == 0 && j == 0)
                    continue;

                nearNode = GetValidNodeNeighbour(currentNodePos.x + i, currentNodePos.y + j, gridNodes, closedNodeSet, room);

                if(nearNode != null)
                {
                    int penaltyCost = room.aStarMovementPenalty[nearNode.gridPosition.x, nearNode.gridPosition.y];
                    int newCost = currentNode.gCost + GetDistance(currentNode, nearNode) + penaltyCost;
                    bool isValidInOpenList = openNodeList.Contains(nearNode);

                    if(newCost < nearNode.gCost || !isValidInOpenList)
                    {
                        nearNode.gCost = newCost;
                        nearNode.hCost = GetDistance(nearNode, targetNode);
                        nearNode.parentNode = currentNode;

                        if(!isValidInOpenList)
                        {
                            openNodeList.Add(nearNode);
                        }
                    }
                }
            }
        }
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int distanceY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        // 작은 수 만큼 대각선으로 이동, 남은 수 만큼 직선으로 이동
        if(distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }

    private static Node GetValidNodeNeighbour(int nodePosX, int nodePosY, GridNodes gridNodes, HashSet<Node> closedNodeSet, InstantiatedRoom room)
    {
        // 방의 범위를 벗어난 경우
        if (nodePosX >= room.room.templateUpperBounds.x - room.room.templateLowerBounds.x || nodePosY < 0 || nodePosX >= room.room.templateUpperBounds.y - room.room.templateLowerBounds.y || nodePosY < 0)
        {
            return null;
        }

        Node nearNode = gridNodes.GetGridNode(nodePosX, nodePosY);
        int penaltyCost = room.aStarMovementPenalty[nearNode.gridPosition.x, nearNode.gridPosition.y];
        
        // 이미 방문한 노드거나 장애물이 있다면
        if(closedNodeSet.Contains(nearNode) || penaltyCost == 0)
        {
            return null;
        }
        else
        {
            return nearNode;
        }
        
    }

}
