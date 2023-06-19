using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryAudioSource : MonoBehaviour
{
	[SerializeField] AudioSource _audioSource;

	public void Init(AudioClip clip, float volume)
	{
		_audioSource.clip = clip;
		_audioSource.volume = volume;
		_audioSource.Play();

		StartCoroutine(LifeTimeClock(_audioSource.clip.length));
	}

	IEnumerator LifeTimeClock(float clipLength)
    {
		yield return new WaitForSeconds(clipLength);
		Destroy(gameObject);
	}
}
