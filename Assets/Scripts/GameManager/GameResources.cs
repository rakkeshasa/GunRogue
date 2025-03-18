using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Audio;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if(instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }    
            return instance;
        }
    }

    [Space(10)]
    [Header("DUNGEON")]
    [Tooltip("Populate with the dungeon RoomNodeTypeListSO")]
    public RoomNodeTypeListSO roomNodeTypeList;

    [Space(10)]
    [Header("PLAYER")]
    [Tooltip("The current player scriptable object - this is used to reference the current player between scenes")]
    public CurrentPlayerSO currentPlayer;

    // 사운드 관련
    [Space(10)]
    [Header("SOUNDS")]
    [Tooltip("Populate with the sounds master mixer group")]
    public AudioMixerGroup soundsMasterMixerGroup;

    [Tooltip("Door open close sound effect")]
    public SoundEffectSO doorOpenCloseSoundEffect;

    [Tooltip("Populate with the table flip sound effect")]
    public SoundEffectSO tableFlip;

    [Tooltip("Populate with the chest open sound effect")]
    public SoundEffectSO chestOpen;

    [Tooltip("Populate with the health pickup sound effect")]
    public SoundEffectSO healthPickup;

    [Tooltip("Populate with the weapon pickup sound effect")]
    public SoundEffectSO weaponPickup;

    [Tooltip("Populate with the ammo pickup sound effect")]
    public SoundEffectSO ammoPickup;

    // 머터리얼 관련
    [Space(10)]
    [Header("MATERIALS")]
    [Tooltip("Dimmed Material")]
    public Material dimmedMaterial;

    [Tooltip("Sprite-Lit-Default Material")]
    public Material litMaterial;

    [Tooltip("Populate with the Variable Lit Shader")]
    public Shader variableLitShader;

    // 타일맵 관련
    [Space(10)]
    [Header("SPECIAL TILEMAP TILES")]
    [Tooltip("Collision thiles that the enemies can navigate to")]
    public TileBase[] enemyCantWalkCollisionTilesArray;

    [Tooltip("Preferred path tile for enemy navigation")]
    public TileBase preferredEnemyPathTile;

    // UI 관련
    [Space(10)]
    [Header("UI")]
    [Tooltip("Populate with ammo icon prefab")]
    public GameObject ammoIconPrefab;


#if UNITY_EDITOR
    // Validate the scriptable object details entered
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsMasterMixerGroup), soundsMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyCantWalkCollisionTilesArray), enemyCantWalkCollisionTilesArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
    }

#endif
}
