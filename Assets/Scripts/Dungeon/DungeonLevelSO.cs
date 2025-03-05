using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    [Space(10)]
    [Header("BASIC LEVEL DETAILS")]
    [Tooltip("The name for the level")]
    public string levelName;

    [Space(10)]
    [Header("ROOM TEMPLATES FOR LEVEL")]
    [Tooltip("Populate the list with the room templates that you want to be part of the level." +
        "You need to ensure that room templates are included for all room node types that are specified in the Room Node Graphs for the level")]
    public List<RoomTemplateSO> roomTemplateList;

    [Space(10)]
    [Header("ROOM NODE GRAPHS FOR LEVEL")]
    [Tooltip("Populate this list with the room node graphs which should be randomly selected from for the level")]
    public List<RoomNodeGraphSO> roomNodeGraphList;

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
            return;
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
            return;

        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        foreach(RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null)
                return;

            if (roomTemplateSO.roomNodeType.isCorridorEW)
                isEWCorridor = true;

            if(roomTemplateSO.roomNodeType.isCorridorNS)
                isNSCorridor = true;

            if(roomTemplateSO.roomNodeType.isEntrance)
                isEntrance = true;
        }

        if(!isEWCorridor)
        {
            Debug.Log("In " + this.name.ToString() + " : No E/W Corridor Room Type Specified");
        }

        if (!isNSCorridor)
        {
            Debug.Log("In " + this.name.ToString() + " : No N/S Corridor Room Type Specified");
        }

        if(!isEntrance)
        {
            Debug.Log("In " + this.name.ToString() + " : No Entrance Corridor Room Type Specified");
        }

        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if (roomNodeGraph == null)
                return;

            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (roomNodeSO == null)
                    continue;

                // 복도와 입구는 위에서 체크함
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW || roomNodeSO.roomNodeType.isCorridorNS ||
                    roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone)
                    continue;

                bool isRoomNodeTypeFound = false;

                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
                {
                    if (roomTemplateSO == null)
                        continue;

                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }

                if(!isRoomNodeTypeFound)
                {
                    Debug.Log("In " + this.name.ToString() + " : No room template " + roomNodeSO.roomNodeType.name.ToString() + " found for node graph "
                        + roomNodeGraph.name.ToString());
                }
            }
        }
    }

#endif
}
