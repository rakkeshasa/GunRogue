using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
    // Action 델리게이트 생성
    public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;

    public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        // '?' : 델리게이트를 구독한 사람이 없으면 처리하지 않음
        // Invoke( Action의 매개변수<AimWeaponEvent, AimWeaponEventArgs> )
        OnWeaponAim?.Invoke(this, new AimWeaponEventArgs() { aimDirection = aimDirection, aimAngle = aimAngle, weaponAimAngle = weaponAimAngle, weaponAimDirectionVector = weaponAimDirectionVector });
    }
}

public class AimWeaponEventArgs : EventArgs
{
    public AimDirection aimDirection;
    public float aimAngle;
    public float weaponAimAngle;
    public Vector3 weaponAimDirectionVector;
}
