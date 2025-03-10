using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachingTargetGroup;

    [Tooltip("Populate with the CursorTarget gameobject")]
    [SerializeField]
    private Transform cursorTarget;

    private void Awake()
    {
        cinemachingTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
    }

    private void SetCinemachineTargetGroup()
    {
        // 시네머신 카메라가 촬영할 TargetGroup을 생성(플레이어, 마우스)
        // 가중치와 바운딩 박스의 반경을 세팅해 TargetGroup의 모든 요소가 화면에 들어오도록하며, 가중치를 통해 가운데 지점을 찍음
        // 현재 플레이어와 마우스가 1:1 비율이라면 카메라의 가운데는 플레이어와 마우스의 중간 지점이 된다.
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target { weight = 1f, radius = 2.5f, target = GameManager.Instance.GetPlayer().transform };
        CinemachineTargetGroup.Target cinemachineGroupTarget_cursor = new CinemachineTargetGroup.Target { weight = 1f, radius = 1f, target = cursorTarget };

        CinemachineTargetGroup.Target[] cinemachingTargetArray = new CinemachineTargetGroup.Target[] { cinemachineGroupTarget_player, cinemachineGroupTarget_cursor };

        cinemachingTargetGroup.m_Targets = cinemachingTargetArray;
    }

    private void Update()
    {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }
}
