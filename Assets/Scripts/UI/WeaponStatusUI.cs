using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatusUI : MonoBehaviour
{
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    [Tooltip("Populate with image component on the child WeaponImage gameobject")]
    [SerializeField] 
    private Image weaponImage;

    [Tooltip("Populate with the Transform from the child AmmoHolder gameobject")]
    [SerializeField] private Transform ammoHolderTransform;

    [Tooltip("Populate with the TextMeshPro-Text component on the child ReloadText gameobject")]
    [SerializeField] 
    private TextMeshProUGUI reloadText;

    [Tooltip("Populate with the TextMeshPro-Text component on the child AmmoRemainingText gameobject")]
    [SerializeField] 
    private TextMeshProUGUI ammoRemainingText;

    [Tooltip("Populate with the TextMeshPro-Text component on the child WeaponNameText gameobject")]
    [SerializeField] 
    private TextMeshProUGUI weaponNameText;

    [Tooltip("Populate with the RectTransform of the child gameobject ReloadBar")]
    [SerializeField] 
    private Transform reloadBar;

    [Tooltip("Populate with the Image component of the child gameobject BarImage")]
    [SerializeField] 
    private Image barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake()
    {
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        // Subscribe to set active weapon event
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;

        // Subscribe to weapon fired event
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;

        // Subscribe to reload weapon event
        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnWeaponReload;

        // Subscribe to weapon reloaded event
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from set active weapon event
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;

        // Unsubscribe from weapon fired event
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;

        // Unsubscribe from reload weapon event
        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnWeaponReload;

        // Unsubscribe from weapon reloaded event
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start()
    {
        // Update active weapon status on the UI
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetActiveWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponFired(weaponFiredEventArgs.weapon);
    }


    private void WeaponFired(Weapon weapon)
    {
        // UI 업데이트
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

    private void ReloadWeaponEvent_OnWeaponReload(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    private void WeaponReloaded(Weapon weapon)
    {
        // 장착된 총이 현재 총인지
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadBar();
        }
    }

    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        // 재장전 중이면 재장전 UI 업데이트
        if (weapon.isWeaponReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar();
        }

        UpdateReloadText(weapon);
    }

    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
    {
        weaponImage.sprite = weaponDetails.weaponSprite;
    }

    private void UpdateActiveWeaponName(Weapon weapon)
    {
        weaponNameText.text = "(" + weapon.weaponListPosition + ") " + weapon.weaponDetails.weaponName.ToUpper();
    }

    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            ammoRemainingText.text = "INFINITE AMMO";
        }
        else
        {
            ammoRemainingText.text = weapon.weaponRemainingAmmo.ToString() + " / " + weapon.weaponDetails.weaponAmmoCapacity.ToString();
        }
    }

    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++)
        {
            // 총알 아이콘 프리팹 인스턴스화
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);
            // 남은 총알을 총기UI 오른쪽에 위쪽부터 하나씩 부착
            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);
            ammoIconList.Add(ammoIcon);
        }
    }

    private void ClearAmmoLoadedIcons()
    {
        foreach (GameObject ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        // UI의 재장전 바 업데이트
        if (weapon.weaponDetails.hasInfiniteClipCapacity)
            return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
    {
        // 재장전 바 애니메이션 코루틴
        barImage.color = Color.red;

        // 재장전이 완료될때까지 코루틴 반복
        while (currentWeapon.isWeaponReloading)
        {
            // 재장전까지 몇 초 남았는지 비율 계산
            float barFill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetails.weaponReloadTime;
            // 재장전 바 비율 조정하기
            reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);
            yield return null;
        }
    }

    private void ResetWeaponReloadBar()
    {
        // 재장전 완료 후 재장전 바 세팅
        StopReloadWeaponCoroutine();

        barImage.color = Color.green;
        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void StopReloadWeaponCoroutine()
    {
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }
    }

    private void UpdateReloadText(Weapon weapon)
    {
        // 'RELOAD' 텍스트가 깜빡이도록 하는 함수
        if ((!weapon.weaponDetails.hasInfiniteClipCapacity) && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
        {
            barImage.color = Color.red;
            StopBlinkingReloadTextCoroutine();

            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());
        }
        else
        {
            StopBlinkingReloadText();
        }
    }

    private IEnumerator StartBlinkingReloadTextRoutine()
    {
        while (true)
        {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            // 깜빡이기 위해
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();

        reloadText.text = "";
    }

    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkingReloadTextCoroutine != null)
        {
            StopCoroutine(blinkingReloadTextCoroutine);
        }
    }


#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }

#endif
}
