using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour
{
    [Tooltip("MovementDetailsSO scriptable object cotaining movement details such as speed")]
    [SerializeField]
    private MovementDetailsSO movementDetails;

    private int currentWeaponIndex = 1;
    private Player player;
    private bool leftMouseDownPreviousFrame = false;
    private float moveSpeed;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isPlayerRolling = false;
    private float playerRollCooldownTimer = 0f;

    private void Awake()
    {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();

        SetStartingWeapon();
        SetPlayerAnimationSpeed();
    }

    private void SetStartingWeapon()
    {
        int index = 1;
        foreach(Weapon weapon in player.weaponList)
        {
            if(weapon.weaponDetails == player.playerDetails.startingWeapon)
            {
                SetWeaponByIndex(index);
                break;
            }
            index++;
        }
    }

    private void SetPlayerAnimationSpeed()
    {
        // �̼��� �������� �ִϸ��̼� ��� �ӵ��� ������
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

    private void Update()
    {
        if (isPlayerRolling) 
            return;

        MovementInput();
        WeaponInput();

        PlayerRoolCooldownTimer();
    }

    private void MovementInput()
    {
        // �̵� ���� �Է�
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);

        // �Է¿� ����� ���⺤�� ����  
        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        // �밢�� �̵��� �� �������� ������
        if(horizontalMovement != 0f && verticalMovement != 0f)
        {
            direction *= 0.7f;
        }

        if (direction != Vector2.zero)
        {
            if(!rightMouseButtonDown)
            {
                player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
            }
            else if(playerRollCooldownTimer <= 0f)
            {
                PlayerRoll((Vector3)direction);
            }
        }
        else
        {
            player.idleEvent.CallIdleEvent();
        }
    }

    private void PlayerRoll(Vector3 direction)
    {
        playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
    }

    private IEnumerator PlayerRollRoutine(Vector3 direction)
    {
        float minDistance = 0.2f;

        isPlayerRolling = true;
        Vector3 targetPosition = player.transform.position + (Vector3)direction * movementDetails.rollDistance;

        while(Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, movementDetails.rollSpeed, direction, isPlayerRolling);

            yield return waitForFixedUpdate;
        }

        isPlayerRolling = false;
        playerRollCooldownTimer = movementDetails.rollCooldownTime;
        player.transform.position = targetPosition;
    }

    private void PlayerRoolCooldownTimer()
    {
        if(playerRollCooldownTimer >= 0f)
        {
            playerRollCooldownTimer -= Time.deltaTime;
        }
    }

    private void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;

        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);

        FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);

        SwitchWeaponInput();

        ReloadWeaponInput();
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        // ���콺 ��ǥ
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();
        
        // �ѱ� -> ���콺 ����
        weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());

        // �÷��̾� -> ���콺 ����
        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

        // �̺�Ʈ �߻���Ű��
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees, AimDirection playerAimDirection)
    {
        if (Input.GetMouseButton(0))
        {
            player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame,playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);

            leftMouseDownPreviousFrame = true;
        }
        else
        {
            leftMouseDownPreviousFrame = false;
        }
    }

    private void SwitchWeaponInput()
    {
        if (Input.mouseScrollDelta.y < 0f)
        {
            PreviousWeapon();
        }

        if(Input.mouseScrollDelta.y > 0f)
        {
            NextWeapon();
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetWeaponByIndex(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetWeaponByIndex(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetWeaponByIndex(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetWeaponByIndex(4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetWeaponByIndex(5);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SetWeaponByIndex(6);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SetWeaponByIndex(7);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SetWeaponByIndex(8);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SetWeaponByIndex(9);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SetWeaponByIndex(10);
        }

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            SetCurrentWeaponToFirstInTheList();
        }
    }   

    private void SetWeaponByIndex(int weaponIndex)
    {
        if(weaponIndex - 1 < player.weaponList.Count)
        {
            currentWeaponIndex = weaponIndex;
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[weaponIndex - 1]);
        }
    }

    private void NextWeapon()
    {
        currentWeaponIndex++;
        if (currentWeaponIndex > player.weaponList.Count)
        {
            currentWeaponIndex = 1;
        }

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void PreviousWeapon()
    {
        currentWeaponIndex--;
        if (currentWeaponIndex < 1)
        {
            currentWeaponIndex = player.weaponList.Count;
        }

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void ReloadWeaponInput()
    {
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();
        if (currentWeapon.isWeaponReloading)
            return;

        // ���� ź���� źâ �뷮���� ���� ���
        if (currentWeapon.weaponRemainingAmmo < currentWeapon.weaponDetails.weaponClipAmmoCapacity && !currentWeapon.weaponDetails.hasInfiniteAmmo)
            return;

        // �Ѿ��� źâ�� ������ ���
        if(currentWeapon.weaponClipRemainingAmmo == currentWeapon.weaponDetails.weaponClipAmmoCapacity)
            return;

        if(Input.GetKeyDown(KeyCode.R))
        {
            player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �����ٰ� ���� �ε����� �ڷ�ƾ �ߴ�
        StopPlayerRollRoutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        StopPlayerRollRoutine();
    }

    private void StopPlayerRollRoutine()
    {
        if(playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);
            isPlayerRolling = false;
        }
    }

    private void SetCurrentWeaponToFirstInTheList()
    {
        List<Weapon> tempWeaponList = new List<Weapon>();

        // ���� ����
        Weapon currentWeapon = player.weaponList[currentWeaponIndex - 1];
        currentWeapon.weaponListPosition = 1;
        tempWeaponList.Add(currentWeapon);
        
        // ������ ����
        int index = 2;
        foreach (Weapon weapon in player.weaponList)
        {
            if (weapon == currentWeapon) continue;

            tempWeaponList.Add(weapon);
            weapon.weaponListPosition = index;
            index++;
        }

        player.weaponList = tempWeaponList;
        currentWeaponIndex = 1;

        SetWeaponByIndex(currentWeaponIndex);
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }

#endif
}
