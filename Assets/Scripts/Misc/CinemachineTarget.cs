using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachingTargetGroup;

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
        // 시네머신 카메라가 촬영할 TargetGroup을 생성
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target { weight = 1f, radius = 1f, target = GameManager.Instance.GetPlayer().transform };

        CinemachineTargetGroup.Target[] cinemachingTargetArray = new CinemachineTargetGroup.Target[] { cinemachineGroupTarget_player };

        cinemachingTargetGroup.m_Targets = cinemachingTargetArray;
    }
}
