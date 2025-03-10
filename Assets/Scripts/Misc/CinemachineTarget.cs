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
        // �ó׸ӽ� ī�޶� �Կ��� TargetGroup�� ����(�÷��̾�, ���콺)
        // ����ġ�� �ٿ�� �ڽ��� �ݰ��� ������ TargetGroup�� ��� ��Ұ� ȭ�鿡 ���������ϸ�, ����ġ�� ���� ��� ������ ����
        // ���� �÷��̾�� ���콺�� 1:1 �����̶�� ī�޶��� ����� �÷��̾�� ���콺�� �߰� ������ �ȴ�.
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
