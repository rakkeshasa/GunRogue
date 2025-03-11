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
        // ������ : InstantiatedRoom
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        // �÷��̾ �濡 �����ϰ� ���� ���� ��Ӵٸ�
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
        // ���� ��Ƽ������ �������� �ʰ�, ���� �濡�� �����ϱ� ���� ���ο� ���͸��� ����
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

        // ���� ���͸���� �ǵ�����
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
