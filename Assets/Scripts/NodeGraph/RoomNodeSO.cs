using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        // ��� �ʱ� ����
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle nodeStyle)
    {
        // ��� �ڽ� �׸���
        GUILayout.BeginArea(rect, nodeStyle);

        // BeginChangeCheck�� EndChangeCheck ���̿��� ������ ��������� üũ��
        EditorGUI.BeginChangeCheck();

        if(parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            // ��� Ÿ��(�Ա�, ���� ��)�� ���� �Ұ��� RoomNode ����
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            // ��� Ÿ���� ������ �� �ְ� ��
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
            roomNodeType = roomNodeTypeList.list[selection];

            // ��� ������ ���� - ���� ���´� ������ �� ������.
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isCorridor
                && roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {
                if (childRoomNodeIDList.Count > 0)
                {
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);

                        if (childRoomNode != null)
                        {
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
                        }
                    }
                }
            }
        }

        // ��ȭ�� �����ƴٸ� �����ϱ�(����Ƽ�� ������ �����ص� �ڵ�����x)
        // ���� ���� Ű�� ���� ��� �ʱ�ȭ�Ǵ� ������ ������
        // SetDirty : ��Ƽ �÷��׸� ����
        // ��Ұ� ����� ������ �����ϴ� ���� ��ȿ�����̹Ƿ� ��Ƽ ǥ�ø� �س��� ���߿� �����Ҷ� ǥ�ð� �����͸� ����
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for(int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }
        return roomArray;
    }

    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // ��Ŭ�� ��
        if(currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        // ��Ŭ�� ��
        else if(currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    private void ProcessLeftClickDownEvent()
    {
        // Selection.activeObject : Unity �����Ϳ��� ���õ� ������Ʈ�� ������
        // ex) �÷��̾� �������� Ŭ�� -> activeObject���� �÷��̾� �������� �����
        Selection.activeObject = this; 

        if(isSelected)
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }

    private void ProcessRightClickDownEvent(Event currentEvnet)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvnet.mousePosition);
    }

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    private void ProcessLeftClickUpEvent()
    {
        if(isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if(currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        // currentEvent.delta : ���콺�� �������� ������
        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        if(IsChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }

        return false;
    }

    public bool IsChildRoomValid(string childID)
    {
        // 1���� ������ 1���� �����游 �����ؾ� �� -> �̹� �ִٸ� �߰� ���ϵ��� ��ġ
        bool isConnectedBossNodeAlready = false;

        foreach(RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
                isConnectedBossNodeAlready = true;
        }

        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
            return false;

        // �ڽ� ����� Ÿ���� None���� üũ
        if(roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
            return false;

        // �̹� �ڽ� ���� ���� �ִ� ���
        if(childRoomNodeIDList.Contains(childID))
            return false;

        // �� �ڽ��� ���Խ�Ű�� ����
        if (id == childID)
            return false;

        // ��带 ��������� ������ �� ����
        if (parentRoomNodeIDList.Contains(childID))
            return false;

        // �ڽ� ���� �θ� ��带 1���� ���� �� ����(2�� �̻� �Ұ�)
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
            return false;

        // ���������� ����� �� ����
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
            return false;

        // �波���� ����� �� ����. ������ ��-����-���� ���¸� ���Ѿ���
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
            return false;

        // ���� 4�� �̻��� ������ ���� �� ����
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
            return false;

        // �Ա��� �׻� �ֻ��� �θ���
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
            return false;

        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
            return false;

        return true;
    }

    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {
        if(childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;
        }

        return false;
    }

    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }

        return false;
    }

#endif
}
