using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{
    [Space(10)]
    [Header("MOVEMENT DETAILS")]
    [Tooltip("The minimum move speed. The GetMoveSpeed method calculates a random value between the minimum and maximum")]
    public float minMoveSpeed = 8f;

    [Tooltip("The maximum move speed. The GetMoveSpeed method calculates a random value between the minimum and maximum")]
    public float maxMoveSpeed = 8f;

    [Tooltip("If there is a roll movement- this is the roll speed")]
    public float rollSpeed;

    [Tooltip("If there is a roll movement - this is the roll distance")]
    public float rollDistance;
    
    [Tooltip("If there is a roll movement - this is the cooldown time in seconds between roll actions")]
    public float rollCooldownTime;
    

    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);
        if (rollDistance != 0f || rollSpeed != 0 || rollCooldownTime != 0)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }
    }

#endif
}
