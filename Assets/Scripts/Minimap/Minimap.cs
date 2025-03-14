using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent] 
public class Minimap : MonoBehaviour
{
    [Tooltip("Populate with the child MinimapPlayer gameobject")]
    [SerializeField] 
    private GameObject miniMapPlayer;

    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameManager.Instance.GetPlayer().transform;

        // 플레이어를 시네마머신 카메라 타깃으로 채우기
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;

        // 미니맵에 플레이어 아이콘 세팅
        SpriteRenderer spriteRenderer = miniMapPlayer.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = GameManager.Instance.GetPlayerMiniMapIcon();
        }
    }

    private void Update()
    {
        // 미니맵 아이콘이 플레이어를 따라가도록 함
        if (playerTransform != null && miniMapPlayer != null)
        {
            miniMapPlayer.transform.position = playerTransform.position;
        }
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(miniMapPlayer), miniMapPlayer);
    }

#endif
}
