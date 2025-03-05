using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임 오브젝트에 1개의 컴포넌트만 존재하도록 함
[DisallowMultipleComponent]
public class GameManager : SingletonMonobehavior<GameManager>
{
    [Space(10)]
    [Header("DUNGEON LEVELS")]
    [Tooltip("Populate with the dungeon level scriptable objects")]
    [SerializeField]
    private List<DungeonLevelSO> dungeonLevelList;

    [Tooltip("Populate with the starting dungeon level for testing, first level = 0")]
    [SerializeField]
    private int currentDungeonLevelListIndex = 0;

    [HideInInspector]
    public GameState gameState;

    private void Start()
    {
        gameState = GameState.gameStarted;
    }

    private void Update()
    {
        HandleGameState();

        if(Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.gameStarted;
        }
    }

    private void HandleGameState()
    {
        switch(gameState)
        {
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonLevelListIndex);
                gameState = GameState.playingLevel;
                break;
        }
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        bool dungeonBuiltSucessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if(!dungeonBuiltSucessfully)
        {
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
        }
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }

#endif
}
