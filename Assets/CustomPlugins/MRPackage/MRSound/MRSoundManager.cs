using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MRSoundManager : MonoBehaviour
{
	AudioSource audioSource;
	public List<MRSound> sounds;
	public static MRSoundManager Instance = null;

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

		this.audioSource = this.GetComponent<AudioSource>();
		this.SetAudioLister();
	}

	public bool IsSoundEnabled()
	{
		if (PlayerPrefs.GetInt("IsSoundEnabled", 1) == 1)
			return true;
		return false;
	}

	public void SetSound(bool enabled)
	{
		if (enabled)
			PlayerPrefs.SetInt("IsSoundEnabled", 1);
		else
			PlayerPrefs.SetInt("IsSoundEnabled", 0);
		this.SetAudioLister();
	}
	
	void SetAudioLister() {
		AudioListener.pause = !MRSoundManager.Instance.IsSoundEnabled();
	}

	public void Play(SoundType soundType, float pitch = 1)
	{
		MRSound sound = this.sounds.Find(p => p.type == soundType);
		this.PlaySound(sound, pitch);
	}

	public void Play(int soundType)
	{
		MRSound sound = this.sounds.Find(p => p.type == (SoundType)soundType);
		this.PlaySound(sound);
	}

	void PlaySound(MRSound sound, float pitch = 1)
	{
		if (sound != null)
		{
			if (pitch == 1)
				this.audioSource.pitch = sound.pitch;
			else
				this.audioSource.pitch = pitch;

			this.audioSource.PlayOneShot(sound.clips[Random.Range(0, sound.clips.Count)], sound.volume);
		}
	}
}
