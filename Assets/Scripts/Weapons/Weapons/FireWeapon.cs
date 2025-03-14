using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float firePreChargeTimer = 0f;
    private float fireRateCoolDownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;

    private void Awake()
    {
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
    }

    private void OnEnable()
    {
        // 발행자 : PlayerControl
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update()
    {
        fireRateCoolDownTimer -= Time.deltaTime;
    }

    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEventArgs);
    }

    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponPreCharge(fireWeaponEventArgs);

        if (fireWeaponEventArgs.fire)
        {
            if (IsWeaponReadyToFire())
            {
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);
                ResetCoolDownTimer();
                ResetPrechargeTimer();
            }
        }
    }

    private void WeaponPreCharge(FireWeaponEventArgs fireWeaponEventArgs)
    {
        // 차지중이라면
        if (fireWeaponEventArgs.firePreviousFrame)
        {
            // 저번 프레임부터 차지중이였다면 차지 타이머 감소
            firePreChargeTimer -= Time.deltaTime;
        }
        else
        {
            ResetPrechargeTimer();
        }
    }

    private bool IsWeaponReadyToFire()
    {
        // 남은 총알이 0개인 경우
        if (activeWeapon.GetCurrentWeapon().weaponRemainingAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo)
            return false;

        // 재장전 중일 경우
        if (activeWeapon.GetCurrentWeapon().isWeaponReloading)
            return false;

        // 연사 속도 타이머가 아직 갱신되지 않은 경우 또는 차지 중인 경우
        if (firePreChargeTimer > 0f || fireRateCoolDownTimer > 0f)
            return false;

        // 탄창에 남은 탄약이 없는 경우
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity && activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo <= 0)
        {
            // 재장전 이벤트 발생
            reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);
            return false;
        }

        return true;
    }

    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

        if (currentAmmo != null)
        {
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
        }
    }

    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        int ammoCounter = 0;
        int ammoPerShot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);

        float ammoSpawnInterval;

        // 여러발의 탄환이 나가는가(샷건)
        if (ammoPerShot > 1)
        {
            ammoSpawnInterval = Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax);
        }
        else
        {
            ammoSpawnInterval = 0f;
        }

        while (ammoCounter < ammoPerShot)
        {
            ammoCounter++;

            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];
            float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

            // IFireable 인터페이스(컴포넌트)를 가지는 게임 오브젝트만 추출
            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);
            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            yield return new WaitForSeconds(ammoSpawnInterval);
        }

        // 무한 탄약이 아니면 총알 감소
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
        {
            activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
            activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
        }

        weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());

        WeaponSoundEffect();
    }

    private void ResetCoolDownTimer()
    {
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }

    private void ResetPrechargeTimer()
    {
        firePreChargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
    }

    private void WeaponSoundEffect()
    {
        if(activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect);
        }
    }
}
