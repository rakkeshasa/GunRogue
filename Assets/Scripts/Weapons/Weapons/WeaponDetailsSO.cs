using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject
{
    [Space(10)]
    [Header("WEAPON BASE DETAILS")]
    [Tooltip("Weapon name")]
    public string weaponName;

    [Tooltip("The sprite for the weapon - the sprite should have the 'generate physics shape' option selected ")]
    public Sprite weaponSprite;

    [Space(10)]
    [Header("WEAPON CONFIGURATION")]
    [Tooltip("Weapon Shoot Position - the offset position for the end of the weapon from the sprite pivot pont")]
    public Vector3 weaponShootPosition;

    [Tooltip("Weapon current ammo")]
    public AmmoDetailsSO weaponCurrentAmmo;

    [Tooltip("Weapon shoot effect SO - contains particle effect parameters to be used in conjunction with the weaponShootEffectPrefab")]
    public WeaponShootEffectSO weaponShootEffect;

    [Tooltip("The firing sound effect SO for the weapon")]
    public SoundEffectSO weaponFiringSoundEffect;

    [Tooltip("The reloading sound effect SO for the weapon")]
    public SoundEffectSO weaponReloadingSoundEffect;

    [Space(10)]
    [Header("WEAPON OPERATING VALUES")]
    [Tooltip("Select if the weapon has infinite ammo")]
    public bool hasInfiniteAmmo = false;

    [Tooltip("Select if the weapon has infinite clip capacity")]
    public bool hasInfiniteClipCapacity = false;

    [Tooltip("The weapon capacity - shots before a reload")]
    public int weaponClipAmmoCapacity = 6;

    [Tooltip("Weapon ammo capacity - the maximum number of rounds at that can be held for this weapon")]
    public int weaponAmmoCapacity = 100;

    [Tooltip("Weapon Fire Rate - 0.2 means 5 shots a second")]
    public float weaponFireRate = 0.2f;

    [Tooltip("Weapon Precharge Time - time in seconds to hold fire button down before firing")]
    public float weaponPrechargeTime = 0f;

    [Tooltip("This is the weapon reload time in seconds")]
    public float weaponReloadTime = 0f;

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);

        if (!hasInfiniteAmmo)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);
        }

        if (!hasInfiniteClipCapacity)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity, false);
        }
    }

#endif
}
