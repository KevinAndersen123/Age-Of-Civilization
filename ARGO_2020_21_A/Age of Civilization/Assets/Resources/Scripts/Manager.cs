using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    //changes current scene to the same scene with the name of the param t_nameOfScene
   public void ChangeScreen(string t_nameOfScene)
    {
        if(t_nameOfScene == "MainMenu")
        {
            FindObjectOfType<AudioManager>().Play("Main_BG");
            FindObjectOfType<AudioManager>().Stop("Gameplay_BG");
        }
        SceneManager.LoadSceneAsync(t_nameOfScene);
    }

    //quits the build app of the game
    public void QuitGame()
    {
        Application.Quit();
    }

}
