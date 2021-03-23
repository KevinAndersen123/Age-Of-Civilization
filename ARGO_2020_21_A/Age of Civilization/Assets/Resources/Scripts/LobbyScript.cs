using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ColorOptions
{
    public string m_name;

    [HideInInspector]
    public int m_ownerID = -1;

    public Color m_colour;
}

public class LobbyScript : MonoBehaviour
{
    const int s_MAX_CIVS = 4;

    GameStartData gameStartData = new GameStartData();

    public int m_activeCivs = 0;

    [SerializeField]
    Dropdown m_civCountDropdown;

    [SerializeField]
    Dropdown m_mapSizeDropdown;
    
    public InputField m_seedInput;

    [SerializeField]
    List<GameObject> m_civObjects;

    [SerializeField]
    List<ColorOptions> m_colourOptions;

    public List<Dropdown> m_colourDropdowns;

    public List<Dropdown> m_playerTypeDropdowns;

    void Awake()
    {
        for (int i = 0; i < s_MAX_CIVS; i++)
        {
            if (i <= m_civCountDropdown.value + 1)
            {
                ActivateCiv(i);
            }
            else
            {
                DeactivateCiv(i);
            }
        }

        RandomizeSeed();
    }

    public void SetActiveCivs()
    {
        for(int i = 0; i < s_MAX_CIVS; i++)
        {
            if(i < m_civCountDropdown.value + 2)
            {
                if(m_civObjects[i].activeSelf == false)
                {
                    ActivateCiv(i);
                }
            }          
            else
            {
                DeactivateCiv(i);
            }
        }

        SetColourDropdowns();
    }

    public void ActivateCiv(int t_id)
    {
        m_civObjects[t_id].SetActive(true);

        bool colourSet = false;

        for (int i = 0; i < m_colourOptions.Count && !colourSet; i++)
        {
            if(m_colourOptions[i].m_ownerID == -1)
            {
                m_colourOptions[i].m_ownerID = t_id;
                colourSet = true;
            }
        }
    }

    public void DeactivateCiv(int t_id)
    {
        m_civObjects[t_id].SetActive(false);

        bool colourUnset = false;

        for (int i = 0; i < m_colourOptions.Count && !colourUnset; i++)
        {
            if (m_colourOptions[i].m_ownerID == t_id)
            {
                m_colourOptions[i].m_ownerID = -1;
                colourUnset = true;
            }
        }

        SetColourDropdowns();
    }

    public void CivColourChange(int t_civID)
    {
        for(int i = 0; i < m_colourOptions.Count; i++)
        {
            if(m_colourOptions[i].m_ownerID == t_civID)
            {
                m_colourOptions[i].m_ownerID = -1;
            }
            else if(m_colourOptions[i].m_name == m_colourDropdowns[t_civID].options[m_colourDropdowns[t_civID].value].text)
            {
                m_colourOptions[i].m_ownerID = t_civID;
            }
        }

        SetColourDropdowns();
    }

    public void SetColourDropdowns()
    {
        for(int i = 0; i < m_colourDropdowns.Count; i++)
        {
            m_colourDropdowns[i].ClearOptions();

            List<string> options = new List<string>();

            foreach(ColorOptions colourOption in m_colourOptions)
            {
                if(colourOption.m_ownerID == i)
                {
                    options.Add(colourOption.m_name);
                }
            }

            foreach (ColorOptions colourOption in m_colourOptions)
            {
                if (colourOption.m_ownerID == -1)
                {
                    options.Add(colourOption.m_name);
                }
            }

            m_colourDropdowns[i].AddOptions(options);
            m_colourDropdowns[i].value = 0;
        }
    }

    public void SetMapSize()
    {
        switch (m_mapSizeDropdown.value)
        {
            case 0:
                ProceduralGeneration.s_generationSize = GenerationSize.Small;
                break;
            case 1:
                ProceduralGeneration.s_generationSize = GenerationSize.Medium;
                break;
            case 2:
                ProceduralGeneration.s_generationSize = GenerationSize.Large;
                break;
        }
    }

    public void RandomizeSeed()
    {
        ProceduralGeneration.GenerateRandomSeed();

        m_seedInput.text = ProceduralGeneration.s_seed;
    }

    public void GoToGameplay()
    {
        m_activeCivs = m_civCountDropdown.value + 2;

        List<Color> colours = new List<Color>();
        List<string> colourNames = new List<string>();

        for (int i = 0; i < m_activeCivs; i++)
        {
            foreach (ColorOptions colourOption in m_colourOptions)
            {
                if (colourOption.m_ownerID == i)
                {
                    colours.Add(colourOption.m_colour);
                    colourNames.Add(colourOption.m_name);
                }
            }
        }

        List<bool> isAIList = new List<bool>();

        isAIList.Add(false);

        for (int i = 0; i < m_activeCivs - 1; i++)
        {
            if (m_playerTypeDropdowns[i].value == 0)
            {
                isAIList.Add(false);
            }
            else if (m_playerTypeDropdowns[i].value == 1)
            {
                isAIList.Add(true);
            }
        }

        GameController.s_playerColours = colours;
        GameController.s_numberOfPlayers = m_activeCivs;
        GameController.s_isAIList = isAIList;

        SetGameStartData();

        StartCoroutine(AnalyticsManager.PostMethod(JsonUtility.ToJson(gameStartData)));
        FindObjectOfType<AudioManager>().Stop("Main_BG");
        FindObjectOfType<AudioManager>().Play("Gameplay_BG");
        GameController.LoadScene("Gameplay");
    }

    public void SetGameStartData()
    {
        gameStartData.device_id = SystemInfo.deviceUniqueIdentifier;
        gameStartData.seed = ProceduralGeneration.s_seed;
        gameStartData.noOfPlayers = GameController.s_numberOfPlayers;

        for (int i = 0; i < m_activeCivs; i++)
        {
            foreach (ColorOptions colourOption in m_colourOptions)
            {
                if (colourOption.m_ownerID == i)
                {
                    gameStartData.playerColours.Add(colourOption.m_name);
                }
            }
        }

        gameStartData.playerTypes.Add("Player");

        for (int i = 0; i < m_activeCivs - 1; i++)
        {
            if (m_playerTypeDropdowns[i].value == 0)
            {
                gameStartData.playerTypes.Add("Player");
            }
            else if (m_playerTypeDropdowns[i].value == 1)
            {
                gameStartData.playerTypes.Add("AI");
            }
        }
    }
}
