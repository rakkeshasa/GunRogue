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

        // �÷��̾ �ó׸��ӽ� ī�޶� Ÿ������ ä���
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;

        // �̴ϸʿ� �÷��̾� ������ ����
        SpriteRenderer spriteRenderer = miniMapPlayer.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = GameManager.Instance.GetPlayerMiniMapIcon();
        }
    }

    private void Update()
    {
        // �̴ϸ� �������� �÷��̾ ���󰡵��� ��
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
