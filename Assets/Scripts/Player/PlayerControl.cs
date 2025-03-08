using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Tooltip("The player WeaponShootPosition gameObject in the hieracrchy")]
    [SerializeField]
    private Transform weaponShootPosition;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        MovementInput();
        WeaponInput();
    }

    private void MovementInput()
    {
        player.idleEvent.CallIdleEvent();
    }

    private void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;

        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        // 마우스 좌표
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();
        
        // 총구 -> 마우스 벡터
        weaponDirection = (mouseWorldPosition - weaponShootPosition.position);

        // 플레이어 -> 마우스 벡터
        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

        // 이벤트 발생시키기
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }
}
