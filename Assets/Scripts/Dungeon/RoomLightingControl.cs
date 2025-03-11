using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(InstantiatedRoom))]
public class RoomLightingControl : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom;

    private void Awake()
    {
        instantiatedRoom = GetComponent<InstantiatedRoom>();
    }

    private void OnEnable()
    {
        // 발행자 : InstantiatedRoom
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        // 플레이어가 방에 입장하고 방이 아직 어둡다면
        if (roomChangedEventArgs.room == instantiatedRoom.room && !instantiatedRoom.room.isLit)
        {
            FadeInRoomLighting();
            FadeInDoors();

            instantiatedRoom.room.isLit = true;
        }
    }


    private void FadeInRoomLighting()
    {
        StartCoroutine(FadeInRoomLightingRoutine(instantiatedRoom));
    }


    private IEnumerator FadeInRoomLightingRoutine(InstantiatedRoom instantiatedRoom)
    {
        // 기존 머티리얼을 변경하지 않고, 개별 방에만 적용하기 위해 새로운 머터리얼 생성
        Material material = new Material(GameResources.Instance.variableLitShader);

        TilemapRenderer[] renderers = 
        {
            instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>(),
            instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>(),
            instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>(),
            instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>(),
            instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>()
        };

        foreach (var renderer in renderers)
        {
            renderer.material = material;
        }

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        // 기존 머터리얼로 되돌리기
        foreach (var renderer in renderers)
        {
            renderer.material = GameResources.Instance.litMaterial;
        }
    }

    private void FadeInDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            DoorLightingControl doorLightingControl = door.GetComponentInChildren<DoorLightingControl>();
            doorLightingControl.FadeInDoor(door);
        }
    }
}
