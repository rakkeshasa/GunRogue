using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{

    [Space(10)]
    [Header("OBJECT REFERENCES")]
    [Tooltip("Populate this with the BoxCollider2D component on the DoorCollider gameobject")]
    [SerializeField] 
    private BoxCollider2D doorCollider;

    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;
    private Animator animator;

    private void Awake()
    {
        // 물리 충돌 감지 X (콜라이더가 없는 것처럼 동작)
        doorCollider.enabled = false;

        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon)
        {
            OpenDoor();
        }
    }

    private void OnEnable()
    {
        // 부모 게임 오브젝트가 비활성화되면(플레이어가 방에서 충분히 멀리 이동하면) 애니메이터 상태가 초기화 됨.
        // 따라서 문의 상태를 기억하기 위해 상태를 따로 저장
        animator.SetBool(Settings.open, isOpen);
    }

    public void OpenDoor()
    {
        // 열려있지 않은 경우만 처리
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpened = true;

            // 열린 문은 더 이상 막지 않는다
            doorCollider.enabled = false; 
            doorTrigger.enabled = false;

            animator.SetBool(Settings.open, true);

            // play sound effect
            // SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.doorOpenCloseSoundEffect);
        }
    }

    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        animator.SetBool(Settings.open, false);
    }

    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if (previouslyOpened == true)
        {
            isOpen = false;
            OpenDoor();
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif
    #endregion
}
