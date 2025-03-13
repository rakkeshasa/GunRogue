using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Space(10)]
    [Header("MATERIALS")]
    [Tooltip("Dimmed Material")]
    public Material dimmedMaterial;

    [Tooltip("Sprite-Lit-Default Material")]
    public Material litMaterial;

    [Tooltip("Populate with the Variable Lit Shader")]
    public Shader variableLitShader;

    // UI ฐüทร
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
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
    }

#endif
}
