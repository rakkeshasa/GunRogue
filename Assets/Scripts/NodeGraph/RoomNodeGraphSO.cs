using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

    private void Awake()
    {
        LoadRoomNodeDictionary();
    }

    private void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();

        foreach(RoomNodeSO node in roomNodeList)
        {
            roomNodeDictionary[node.id] = node;
        }
    }

    public RoomNodeSO GetRoomNode(RoomNodeTypeSO roomNodeType)
    {
        foreach(RoomNodeSO node in roomNodeList)
        {
            if (node.roomNodeType == roomNodeType)
            {
                return node;
            }
        }
        return null;
    }

    public IEnumerable<RoomNodeSO> GetChildRoomNodes(RoomNodeSO parentRoomNode)
    {
        // �÷����� ��ȯ������, IEnumerable<T>�� ��������Ƿ� ���ͷ����͸� ���� �ϳ��� ��Ҹ� ��ȯ
        foreach (string childNodeID in parentRoomNode.childRoomNodeIDList)
        {   
            // �Լ��� ����Ǹ� ����� ��� ��ȯ���� �ʰ�, ȣ���ڰ� ��û�� ������ �ϳ��� ��ȯ�ϴ� ���
            // yield return�� ����ϸ� �޸𸮿� ��� ��Ҹ� �������� �ʰ�, ȣ���ڰ� foreach�� ��û�� ������ �ϳ��� ��ȯ�մϴ�.
            // ȣ���ڰ� �ϳ��� ���������� �ڽĳ�� id�� �����ü� �ִ�.
            yield return GetRoomNode(childNodeID);
        }
    }

    public RoomNodeSO GetRoomNode(string roomNodeID)
    {
        // TryGetValue() �޼���� Dictionary���� ������ Ű�� ����� ���� �˻��ϴ� �� ���
        if (roomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNode))
        {
            return roomNode;
        }
        return null;
    }

#if UNITY_EDITOR

    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;

    public void OnValidate()
    {
        // �����Ϳ��� ���� ������ ���� ������ ��ųʸ� ä���
        LoadRoomNodeDictionary();
    }

    public void SetNodeToDrawConnectionLineFrom(RoomNodeSO node, Vector2 position)
    {
        roomNodeToDrawLineFrom = node;
        linePosition = position;
    }

#endif
}
