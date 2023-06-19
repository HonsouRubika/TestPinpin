using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject temporaryAudioSourcePrefab;
    [SerializeField] GameObject temporaryAudioSourceParent;
    [SerializeField] AudioSource backgroundMusicAudioSource;

    [Header("Volume Properties")]
    [SerializeField] float globalVolume;

    [Header("BackgroundMusic")]
    [SerializeField] AudioClip forestAmbiance;

    [Header("SFX")]
    [SerializeField] List<AudioClip> cutWoodSFX;
    [SerializeField] AudioClip winSFX;
    [SerializeField] AudioClip buttonClickSFX;

    #region SFX

    public void UpgradeButtonClickSFX()
    {
        PlaySFX(buttonClickSFX);
    }

    public void CutWoodSFX(float woodHealth)
    {
        //first hit
        if(woodHealth != 0)
            PlaySFX(cutWoodSFX[Random.Range(0, 2)]);
        //last hit (tree destruction)
        else
            PlaySFX(cutWoodSFX[2]);
    }

    public void VictorySFX()
    {
        PlaySFX(winSFX);
    }

    #endregion


    #region Back

    public void PlaySFX(AudioClip sfx, float volume = 0.5f)
    {
        Instantiate(temporaryAudioSourcePrefab, temporaryAudioSourceParent.transform).GetComponent<TemporaryAudioSource>().Init(sfx, volume * globalVolume);
    }

    #endregion
}
