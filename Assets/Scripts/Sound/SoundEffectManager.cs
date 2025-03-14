using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonobehaviour<SoundEffectManager>
{
    // 사운드를 틀어주는 매니저
    public int soundsVolume = 8;

    private void Start()
    {
        if (PlayerPrefs.HasKey("soundsVolume"))
        {
            soundsVolume = PlayerPrefs.GetInt("soundsVolume");
        }

        SetSoundsVolume(soundsVolume);
    }

    private void OnDisable()
    {
        // playerprefs에 볼륨 세팅 저장
        PlayerPrefs.SetInt("soundsVolume", soundsVolume);
    }

    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        // 오브젝트 풀에 있는 사운드 오브젝트를 통해 재생
        SoundEffect sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffect.soundPrefab, Vector3.zero, Quaternion.identity);
        sound.SetSound(soundEffect);
        sound.gameObject.SetActive(true);
        StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));
    }


    private IEnumerator DisableSound(SoundEffect sound, float soundDuration)
    {
        // 사운드가 끝나면 비활성화(오디오소스 정지)
        yield return new WaitForSeconds(soundDuration);
        sound.gameObject.SetActive(false);
    }

    public void IncreaseSoundsVolume()
    {
        int maxSoundsVolume = 20;

        if (soundsVolume >= maxSoundsVolume) return;

        soundsVolume += 1;

        SetSoundsVolume(soundsVolume); ;
    }

    public void DecreaseSoundsVolume()
    {
        if (soundsVolume == 0) return;

        soundsVolume -= 1;

        SetSoundsVolume(soundsVolume);
    }

    private void SetSoundsVolume(int soundsVolume)
    {
        float muteDecibels = -80f;

        if (soundsVolume == 0)
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", HelperUtilities.LinearToDecibels(soundsVolume));
        }
    }
}
