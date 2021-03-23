using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameplayManager : MonoBehaviour
{
    [HideInInspector]
    public int m_numberOfPlayers;
    
    [HideInInspector]
    public static int s_currentPlayerTurn = 0;

    public List<Player> m_players = new List<Player>();
    List<PlayerAI> m_playerAIs = new List<PlayerAI>();

    MapGrid m_mapGrid;

    GameEndData gameEndData = new GameEndData();

    [SerializeField]
    CameraController m_camera;

    [HideInInspector]
    public float m_timer = 0.0f;

    [HideInInspector]
    public int m_time = 0;

    [HideInInspector]
    public int m_turnAmount = 0;

    [SerializeField]
    GameObject m_playerPrefab;

    [SerializeField]
    GameObject m_playerAIPrefab;

    [SerializeField]
    GameObject[] m_unitPrefabs;

    [SerializeField]
    Text m_currentTurnText;

    [SerializeField]
    GameObject m_currentTurnBG;

    [SerializeField]
    HudValues m_hudValues;

    public static List<List<string>> s_cityNames = new List<List<string>>();

    static public void InitCityNames()
    {
        s_cityNames.Add(new List<string> { "Oslo", "Bergen", "Tromsø", "Stavanger", "Kristiansand", "Trondheim", "Alesund", "Lillehammer", "Kattegat", "Drammen" });
        s_cityNames.Add(new List<string> { "Stockholm", "Gothenburg", "Umeå", "Lund", "Uppsala", "Linköping", "Helsingborg", "Visby", "Västerås", "Karlstad"});
        s_cityNames.Add(new List<string> { "Copenhagen", "Aarhus", "Aalborg", "Odense", "Helsingør", "Billund","Ribe","Skagen", "Esbjerg", "Kolding"});
        s_cityNames.Add(new List<string> { "Reykjavík", "Akureyri", " Hafnarfjordur", "Gardabær", "Húsavík", "Vik", "Selfoss", "Egilsstaðir", "Höfn", "Akranes"});
    }

    static public string GetCityName(int t_listIndex)
    {
        int index = Random.Range(0, s_cityNames[t_listIndex].Count);

        string cityName = s_cityNames[t_listIndex][index];

        s_cityNames[t_listIndex].Remove(cityName);

        return cityName;
    }

    // Start is called before the first frame update
    void Awake()
    {
        s_currentPlayerTurn = 0;
        InitCityNames();
        m_numberOfPlayers = GameController.GetNumberOfPlayers();
        m_mapGrid = FindObjectOfType<MapGrid>();
        ProceduralGeneration.GenerateMap(m_mapGrid);

        InitPlayers();
        CreateStartUnits();
        SetTurnStartUI();
        m_players[s_currentPlayerTurn].TurnStart();

        if (m_camera != null)
        {
            m_camera.SetPosition(m_players[s_currentPlayerTurn].GetUnits()[0].transform.position);
        }
    }

    private void Update()
    {
        m_hudValues.SetHudValues(m_players[0]);
        UpdateTime();

        if (m_players[0].CheckIfPlayerLost())
        {
            SetGameEndData();

            GameController.LoadScene("PostScene");
        }
        else
        {
            bool hasPlayerWon = true;

            for (int i = 1; i < m_numberOfPlayers; i++)
            {
                if(!m_players[i].CheckIfPlayerLost())
                {
                    hasPlayerWon = false;
                    break;
                }
            }

            if(hasPlayerWon)
            {
                SetGameEndData();
                GameController.LoadScene("PostScene");
            }
        }
    }

    void SetTurnStartUI()
    {
        m_currentTurnText.text = "Player " + (s_currentPlayerTurn + 1) + "'s turn";

        Color color = m_players[s_currentPlayerTurn].GetColor();
        color.a = 1.0f;
        m_currentTurnBG.GetComponent<Image>().color = color;
    }

    public void EndTurn()
    {
        if(!GameController.s_isAIList[s_currentPlayerTurn])
        {
            ChangeCurrentTurn();
        }
    }

    public void ChangeCurrentTurn()
    {
        m_players[s_currentPlayerTurn].TurnEnd();

        bool nextPlayer = true;

        while(nextPlayer)
        {
            m_turnAmount++;
            s_currentPlayerTurn = (s_currentPlayerTurn + 1) % m_numberOfPlayers;

            if(!m_players[s_currentPlayerTurn].CheckIfPlayerLost())
            {
                nextPlayer = false;
            }
        }

        m_players[s_currentPlayerTurn].TurnStart();
        SetTurnStartUI();

        foreach(PlayerAI playerAI in m_playerAIs)
        {
            if(playerAI.GetPlayer() == m_players[s_currentPlayerTurn])
            {
                StartCoroutine(playerAI.DoTurn());
                return;
            }
        }
    }

    void InitPlayers()
    {
        for (int i = 0; i < m_numberOfPlayers; i++)
        {
            if(GameController.s_isAIList[i])
            {
                GameObject playerAI = Instantiate(m_playerAIPrefab);
                playerAI.transform.SetParent(transform);
                playerAI.GetComponent<PlayerAI>().Initialize(i, GameController.s_playerColours[i], m_mapGrid);
                playerAI.GetComponent<PlayerAI>().SetGameplayManager(this);
                playerAI.name = "Player AI " + (i + 1);
                m_playerAIs.Add(playerAI.GetComponent<PlayerAI>());
                playerAI.GetComponent<PlayerAI>().GetPlayer().m_isAI = true;
                m_players.Add(playerAI.GetComponent<PlayerAI>().GetPlayer());
            }
            else
            {
                GameObject player = Instantiate(m_playerPrefab);
                player.transform.SetParent(transform);
                player.GetComponent<Player>().Initialize(i, GameController.s_playerColours[i], m_mapGrid);
                player.name = "Player " + (i + 1);
                m_players.Add(player.GetComponent<Player>());
            }
        }

        foreach(PlayerAI playerAI in m_playerAIs)
        {
            foreach(Player player in m_players)
            {
                if(playerAI.GetPlayer() != player)
                {
                    playerAI.AddOtherPlayer(player);
                }
            }
        }
    }

    private void CreateStartUnits()
    {
        foreach (Player player in m_players)
        {
            bool spawned = false;
            
            while (!spawned)
            {
                int x = Random.Range(2, m_mapGrid.GetWidth() - 4);
                int y = Random.Range(2, m_mapGrid.GetHeight() - 4);

                MapIndex indexPos = new MapIndex(x, y);

                if (m_mapGrid.GetTile(indexPos).GetTileType() != TileType.Mountain && m_mapGrid.GetTile(indexPos).GetTileType() != TileType.Water)
                {
                    spawned = true;
                    player.AddUnitToPlayer(m_unitPrefabs[0], indexPos);
                    player.AddUnitToPlayer(m_unitPrefabs[7], indexPos);
                    player.AddUnitToPlayer(m_unitPrefabs[3], indexPos);

                    
                }
            }
        }
    }

    private void UpdateTime()
    {
        m_timer += Time.deltaTime;
        m_time = (int)(m_timer % 60);
    }

    private void SetGameEndData()
    {
        gameEndData.device_id = SystemInfo.deviceUniqueIdentifier;
        gameEndData.time = m_time;
        gameEndData.turnAmount = m_turnAmount;

        foreach (Player p in m_players)
        {
            foreach (GameObject city in p.m_cities)
            {
                gameEndData.citiesControlled++;
            }
            foreach (GameObject unit in p.m_units)
            {
                gameEndData.unitsControlled++;
            }
        }

        GetMostFrequentUnit();

        StartCoroutine(AnalyticsManager.PostMethod(JsonUtility.ToJson(gameEndData)));
    }

    private void GetMostFrequentUnit()
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        int highest = 0;
        string name = "";

        foreach (Player p in m_players)
        {
            foreach (GameObject unit in p.m_units)
            {
                if (unit.GetComponent<Unit>().GetUnitType() == UnitType.Settler)
                {
                    if (!dict.ContainsKey(unit.GetComponent<Unit>().GetUnitType().ToString()))
                    {
                        dict[unit.GetComponent<Unit>().GetUnitType().ToString()] = 0;
                    }
                    dict[unit.GetComponent<Unit>().GetUnitType().ToString()]++;
                }
                else 
                {
                    if (!dict.ContainsKey(unit.GetComponent<CombatUnit>().GetCombatUnitType().ToString()))
                    {
                        dict[unit.GetComponent<CombatUnit>().GetCombatUnitType().ToString()] = 0;
                    }
                    dict[unit.GetComponent<CombatUnit>().GetCombatUnitType().ToString()]++;
                }
            }
        }

        foreach (var i in dict)
        {
            if (i.Value > highest)
            {
                highest = i.Value;
                name = i.Key;
            }
        }

        gameEndData.mostFrequentUnit = name;
        gameEndData.MFUnitCount = highest;
    }
}
