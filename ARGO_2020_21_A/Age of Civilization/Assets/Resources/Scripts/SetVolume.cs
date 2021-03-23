using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider m_slider;
    public string m_volName;

    //sets the value of the slider to the saved value
    void Start()
    {
        //slider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        Initialise();
    }
    public void Initialise()
    {
        if (m_volName == "MusicVol")
        {
            m_slider.value = AudioManager.s_musicVolume;
        }
        else
        {
            m_slider.value = AudioManager.s_sfxVolume;
        }

        SetLevel();
    }


    //sets level of the slider to the log base 10 of the slider value*20.
    //saves the volume to the PlayersPrefs to save it on next load
    public void SetLevel()
    {
        float sliderValue = m_slider.value;
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);

        if (m_volName == "MusicVol")
        {
            AudioManager.s_musicVolume = sliderValue;
        }
        else
        {
            AudioManager.s_sfxVolume = sliderValue;
        }
        //PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }
}