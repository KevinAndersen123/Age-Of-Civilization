using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    //plays the hover sound effect
    public void HoverSound()
    {
        FindObjectOfType<AudioManager>().Play("Hover");
    }
    //plays the click sound effect
    public void ClickSound()
    {
        FindObjectOfType<AudioManager>().Play("Select");
    }
}
