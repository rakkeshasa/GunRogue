using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    // 픽셀 설정
    public const float pixelsPerUnit = 16f;
    public const float tileSizePixels = 16f;

    // 방 문 애니메이션 설정
    public const float fadeInTime = 0.5f; // time to fade in the room
    public const float doorUnlockDelay = 1f;

    // 던전 빌드 세팅
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;

    // 방 관련 세팅
    public const int maxChildCorridors = 3;

    // 애니메이터 파라미터
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

    // 문 전용 애니메이터
    public static int open = Animator.StringToHash("open");
    public static int close = Animator.StringToHash("close");

    // 플레이어 태그
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";

    // 사격 제어
    // 몬스터가 가까이 붙으면 무기의 사격각을 사용하지 않고 플레이어의 사격각을 사용
    // 발사 위치도 무기가 아닌 플레이어를 기준으로 나가게 함
    public const float useAimAngleDistance = 3.5f;

    // A스타 알고리즘 관련
    public const int defaultPathPenalty = 40;
    public const int preferredPathPenalty = 1;

    // UI 전용
    public const float uiAmmoIconSpacing = 4f;
}
