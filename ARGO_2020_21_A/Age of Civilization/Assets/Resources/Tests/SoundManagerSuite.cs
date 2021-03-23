using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

public class SoundManagerSuite
{
    GameObject m_soundManagerObject;
    AudioManager m_soundManager;

    string m_path = "Prefabs/AudioManager";

    [OneTimeSetUp]
    public void Setup()
    {
        if (GameObject.FindObjectOfType<AudioManager>() != null)
        {
            GameObject.DestroyImmediate(GameObject.FindObjectOfType<AudioManager>());
        }

        if (m_soundManagerObject == null)
        {
            GameObject managerPrefab = Resources.Load<GameObject>(m_path);

            m_soundManagerObject = GameObject.Instantiate(managerPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity);
            m_soundManagerObject.AddComponent<AudioListener>();

            m_soundManager = m_soundManagerObject.GetComponent<AudioManager>();
        }
    }

    [UnityTest, Order(1)]
    public IEnumerator TestSoundPlaying()
    {
        string soundName = "War_Horn";

        m_soundManager.Play(soundName);

        Sound sound = Array.Find(m_soundManager.m_sounds, item => item.m_name == soundName);

        Assert.True(sound.m_source.isPlaying == true);

        yield return null;
    }

    [UnityTest, Order(2)]
    public IEnumerator TestStopSoundPlaying()
    {
        string soundName = "War_Horn";

        Sound sound = Array.Find(m_soundManager.m_sounds, item => item.m_name == soundName);

        Assert.True(sound.m_source.isPlaying == true);

        m_soundManager.Stop(soundName);

        Assert.True(sound.m_source.isPlaying == false);

        yield return null;
    }

    [UnityTest, Order(3)]
    public IEnumerator TestStopAllSoundPlaying()
    {
        string soundName = "War_Horn";
        string soundName2 = "Main_BG";

        Sound sound = Array.Find(m_soundManager.m_sounds, item => item.m_name == soundName);
        Sound sound2 = Array.Find(m_soundManager.m_sounds, item => item.m_name == soundName2);

        m_soundManager.Play(soundName);
        m_soundManager.Play(soundName2);

        Assert.True(sound.m_source.isPlaying == true);
        Assert.True(sound2.m_source.isPlaying == true);

        m_soundManager.StopAllSounds();

        Assert.True(sound.m_source.isPlaying == false);
        Assert.True(sound2.m_source.isPlaying == false);

        yield return null;
    }

    [UnityTest, Order(4)]
    public IEnumerator TestPlayMusic()
    {
        string musicName = "Main_BG";

        Sound music = Array.Find(m_soundManager.m_sounds, item => item.m_name == musicName);

        m_soundManager.Play(musicName);

        Assert.True(music.m_source.isPlaying == true);

        yield return null;
    }

    //[UnityTest, Order(5)]
    //public IEnumerator StopPreviousMusic()
    //{
    //    string musicName = "Main_BG";

    //    Sound music = Array.Find(m_soundManager.m_sounds, item => item.m_name == musicName);

    //    Assert.True(music.m_source.isPlaying == true);

    //    string musicName2 = "";

    //    m_soundManager.Play(musicName2);

    //    Sound music2 = Array.Find(m_soundManager.m_sounds, item => item.m_name == musicName2);

    //    Assert.True(music.m_source.isPlaying == false);
    //    Assert.True(music2.m_source.isPlaying == true);

    //    yield return null;
    //}

    //[UnityTest, Order(6)]
    //public IEnumerator TestPlayRandomBackgroundMusic()
    //{
    //    Sound backgroundMusic = new Sound();

    //    m_soundManager.PlayRandomBackgroundTrack();

    //    foreach (Sound music in m_soundManager.m_musicTracks)
    //    {
    //        if (music.m_source.isPlaying)
    //        {
    //            backgroundMusic = music;
    //        }
    //    }

    //    bool diffrentMusicPlayed = false;

    //    for (int i = 0; i < 30 && !diffrentMusicPlayed; i++)
    //    {
    //        m_soundManager.PlayRandomBackgroundTrack();

    //        foreach (Sound music in m_soundManager.m_musicTracks)
    //        {
    //            if (music.m_source.isPlaying)
    //            {
    //                if (music != backgroundMusic)
    //                {
    //                    diffrentMusicPlayed = true;
    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    Assert.True(diffrentMusicPlayed == true);

    //    yield return null;
    //}

    [OneTimeTearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(m_soundManagerObject);
    }
}