using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    [Header("Only flag the RoomNodeTypes that should be visible in the editor")]
    public bool displayInNodeGraphEditor = true;

    [Header("One Type Should Be A Corridor")]
    public bool isCorridor;

    [Header("One Type Should Be A CorridorNS")]
    public bool isCorridorNS;

    [Header("One Type Should Be A CorridorEW")]
    public bool isCorridorEW;

    [Header("One Type Should Be An Entrance")]
    public bool isEntrance;

    [Header("One Type Should Be A Boss Room")]
    public bool isBossRoom;

    [Header("One Type Should Be None (Unassigned)")]
    public bool isNone;

#if UNITY_EDITOR

    // 스크립트가 로드되거나 인스펙터에서 값이 변경될 때 Unity가 호출하는 에디터 전용 함수
    // 인스펙터에서 값이 변경된 후 작업을 수행합니다(예: 데이터가 특정 범위 내에 있는지 확인)
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }

#endif
}
