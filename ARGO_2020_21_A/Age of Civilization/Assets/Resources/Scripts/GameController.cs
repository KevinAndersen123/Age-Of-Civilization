using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController s_instance;

    static float s_lastUpdateTime;

    public static float s_timeElapsed;

    static bool s_updateTimer = true;

    public static string s_currentSceneName = "";

    public static int s_numberOfPlayers = 4;

    public static List<bool> s_isAIList = new List<bool> { false, false, false, false };

    public static List<Color> s_playerColours = new List<Color>{Color.red, Color.green, Color.blue, Color.yellow};

    void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            s_instance = this;

            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        s_currentSceneName = SceneManager.GetActiveScene().path;

        SetUpdateTimer(true);
    }

    void Update()
    {
        if (s_updateTimer)
        {
            UpdateTime();
        }
    }

    /// <summary>
    /// Pauses the game by setting the time scale to 0 and the state is set to paused.
    /// </summary>
    public static void PauseGame()
    {
        Time.timeScale = 0;
    }

    /// <summary>
    /// Unpauses the game by setting the time scale to 1 and the state is set to unpaused.
    /// </summary>
    public static void UnpauseGame()
    {
        Time.timeScale = 1;
    }

    /// <summary>
    /// Updates the time remaning before the player runs out of time
    /// and the level restarts.
    /// </summary>
    static void UpdateTime()
    {
        s_lastUpdateTime = Time.time;
        s_timeElapsed += s_lastUpdateTime;
    }

    public static void LoadScene(string t_sceneName)
    {
        SceneManager.LoadScene(t_sceneName);
    }

    public static void ReloadCurrentScene()
    {
        SceneManager.LoadScene(s_currentSceneName);
    }

    public static void SetUpdateTimer(bool t_updateTimer)
    {
        s_updateTimer = t_updateTimer;
    }

    public static int GetNumberOfPlayers()
    {
        return s_numberOfPlayers;
    }

    public static void SetNumberOfPlayers(int newNumberOfPlayers)
    {
        s_numberOfPlayers = newNumberOfPlayers;
    }
}
