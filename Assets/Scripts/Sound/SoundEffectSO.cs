using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Objects/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
    [Space(10)]
    [Header("SOUND EFFECT DETAILS")]
    [Tooltip("The name for the sound effect")]
    public string soundEffectName;

    [Tooltip("The prefab for the sound effect")]
    public GameObject soundPrefab;

    [Tooltip("The audio clip for the sound effect")]
    public AudioClip soundEffectClip;

    [Tooltip("The minimum pitch variation for the sound effect.  A random pitch variation will be generated between the minimum and maximum values.  A random pitch variation makes sound effects sound more natural.")]
    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMin = 0.8f;

    [Tooltip("The maximum pitch variation for the sound effect.  A random pitch variation will be generated between the minimum and maximum values.  A random pitch variation makes sound effects sound more natural.")]
    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMax = 1.2f;

    [Tooltip("The sound effect volume.")]
    [Range(0f, 1f)]
    public float soundEffectVolume = 1f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(soundEffectName), soundEffectName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundPrefab), soundPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundEffectClip), soundEffectClip);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(soundEffectPitchRandomVariationMin), soundEffectPitchRandomVariationMin, nameof(soundEffectPitchRandomVariationMax), soundEffectPitchRandomVariationMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(soundEffectVolume), soundEffectVolume, true);
    }
#endif
}
