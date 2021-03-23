using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager m_instance;

	//public AudioMixerGroup mixerGroup;

	public Sound[] m_sounds;

	public static float s_sfxVolume;
	public static float s_musicVolume;
	void Awake()
	{
        //makes sure there is only one audio manager
		if (m_instance != null)
		{
			DestroyImmediate(gameObject);
			return;
		}
		else
		{
			m_instance = this;
			s_musicVolume = 0.5f;
			s_sfxVolume = 0.5f;
			DontDestroyOnLoad(gameObject);
		}

        //makes a new sound object and sets its varables
		foreach (Sound s in m_sounds)
		{
			s.m_source = gameObject.AddComponent<AudioSource>();
			s.m_source.clip = s.m_clip;
			s.m_source.loop = s.m_loop;

			s.m_source.outputAudioMixerGroup = s.m_mixerGroup;
		}
	}
	void Start()
    {
		Play("Main_BG");
		Play("WarHorn");
	}

    /// <summary>
    /// plays the sound based on the string that is passed in, it searches for the sound name with the same name as the string
    /// </summary>
    /// <param name="sound"></param> name of the sound
	public void Play(string sound)
	{
		Sound s = Array.Find(m_sounds, item => item.m_name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}
		s.m_source.volume = s.m_volume * (1f + UnityEngine.Random.Range(-s.m_volumeVariance / 2f, s.m_volumeVariance / 2f));

		s.m_source.Play();
	}
	public void Stop(string sound)
    {
		Sound s = Array.Find(m_sounds, item => item.m_name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}
		s.m_source.Stop();
	}
	public void StopAllSounds()
    {
		foreach(Sound s in m_sounds)
        {
			s.m_source.Stop();
        }
    }
}
