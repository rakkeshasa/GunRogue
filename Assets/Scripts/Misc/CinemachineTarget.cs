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
        // �ó׸ӽ� ī�޶� �Կ��� TargetGroup�� ����
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target { weight = 1f, radius = 1f, target = GameManager.Instance.GetPlayer().transform };

        CinemachineTargetGroup.Target[] cinemachingTargetArray = new CinemachineTargetGroup.Target[] { cinemachineGroupTarget_player };

        cinemachingTargetGroup.m_Targets = cinemachingTargetArray;
    }
}
