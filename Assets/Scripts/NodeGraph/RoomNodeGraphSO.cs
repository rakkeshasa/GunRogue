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
        // 컬렉션을 반환하지만, IEnumerable<T>을 사용했으므로 이터레이터를 통해 하나씩 요소를 반환
        foreach (string childNodeID in parentRoomNode.childRoomNodeIDList)
        {   
            // 함수가 실행되면 결과를 즉시 반환하지 않고, 호출자가 요청할 때마다 하나씩 반환하는 방식
            // yield return을 사용하면 메모리에 모든 요소를 저장하지 않고, 호출자가 foreach로 요청할 때마다 하나씩 반환합니다.
            // 호출자가 하나씩 순차적으로 자식노드 id를 가져올수 있다.
            yield return GetRoomNode(childNodeID);
        }
    }

    public RoomNodeSO GetRoomNode(string roomNodeID)
    {
        // TryGetValue() 메서드는 Dictionary에서 지정된 키와 연결된 값을 검색하는 데 사용
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
        // 에디터에서 변경 사항이 있을 때마다 딕셔너리 채우기
        LoadRoomNodeDictionary();
    }

    public void SetNodeToDrawConnectionLineFrom(RoomNodeSO node, Vector2 position)
    {
        roomNodeToDrawLineFrom = node;
        linePosition = position;
    }

#endif
}
