using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    // �ȼ� ����
    public const float pixelsPerUnit = 16f;
    public const float tileSizePixels = 16f;

    // �� �� �ִϸ��̼� ����
    public const float fadeInTime = 0.5f; // time to fade in the room
    public const float doorUnlockDelay = 1f;

    // ���� ���� ����
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;

    // �� ���� ����
    public const int maxChildCorridors = 3;

    // �ִϸ����� �Ķ����
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static float baseSpeedForPlayerAnimations = 8f;

    // �� ���� �ִϸ�����
    public static int open = Animator.StringToHash("open");
    public static int close = Animator.StringToHash("close");

    // �÷��̾� �±�
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";

    // ��� ����
    // ���Ͱ� ������ ������ ������ ��ݰ��� ������� �ʰ� �÷��̾��� ��ݰ��� ���
    // �߻� ��ġ�� ���Ⱑ �ƴ� �÷��̾ �������� ������ ��
    public const float useAimAngleDistance = 3.5f;

    // UI ����
    public const float uiAmmoIconSpacing = 4f;
}
