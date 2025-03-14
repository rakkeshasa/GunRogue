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
        // ���� �浹 ���� X (�ݶ��̴��� ���� ��ó�� ����)
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
        // �θ� ���� ������Ʈ�� ��Ȱ��ȭ�Ǹ�(�÷��̾ �濡�� ����� �ָ� �̵��ϸ�) �ִϸ����� ���°� �ʱ�ȭ ��.
        // ���� ���� ���¸� ����ϱ� ���� ���¸� ���� ����
        animator.SetBool(Settings.open, isOpen);
    }

    public void OpenDoor()
    {
        // �������� ���� ��츸 ó��
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpened = true;

            // ���� ���� �� �̻� ���� �ʴ´�
            doorCollider.enabled = false; 
            doorTrigger.enabled = false;

            animator.SetBool(Settings.open, true);
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.doorOpenCloseSoundEffect);
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
