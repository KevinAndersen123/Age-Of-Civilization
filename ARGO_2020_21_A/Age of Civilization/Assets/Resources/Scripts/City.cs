using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BorderTile
{
    public Tile m_tile;

    public GameObject m_object;
}

public class City : MonoBehaviour
{
    Tile m_originTile;
    public List<BorderTile> m_borderTiles = new List<BorderTile>();

    List<Tile> m_availableTiles = new List<Tile>();

    List<Building> m_buildings = new List<Building>();

    List<ResourceBuilding> m_resourceBuildings = new List<ResourceBuilding>();

    [HideInInspector]
    public List<Tile> m_resourceTiles;

    public string m_name = "";

    public int m_population = 1;
    public int m_food = 0;
    public int m_production = 0;
    public int m_gold = 0;
    public int m_nextPopulation = 0;
    public int m_nextExpansion = 0;

    public int m_borderExpansionCount = 0;
    public int m_maxFood = 15;
    public int m_foodBasket = 0;
    public int m_foodTurns = 1;
    public int m_queueCounter = 0;

    public string m_typeToBuild = "";
    public int m_typeToBuildID = 0;
    public int m_maxTimeToBuild = 0;

    int m_currentHealth = 100;
    int m_maxHealth = 100;
    int m_baseHealth = 100;

    float m_maxHealthWidth;

    [SerializeField]
    GameObject m_borderTileImage;

    [SerializeField]
    GameObject m_healthbar;

    [SerializeField]
    GameObject m_particleEffect;
    [SerializeField]
    Text m_text;

    [SerializeField]
    CityUI m_cityUI;

    Player m_owner;
    
    public bool m_itemInQueue = false;
    public bool m_enabledBarracks = false;
    public bool m_enabledWorkshop = false;
    public bool m_enabledStables = false;

    public string m_itemInqueueName = "";

    [SerializeField]
    public GameObject m_resourceBuildingPrefab;

    [SerializeField]
    public GameObject m_buildingPrefab;

    public GameObject[] m_units;

    int m_economicScore = 0;

    public void Setup(Tile t_originTile, Player t_player)
    {
        m_maxHealthWidth = m_healthbar.GetComponent<RectTransform>().sizeDelta.x;

        m_owner = t_player;
        m_originTile = t_originTile;

        if(m_originTile.GetResourceTile() != null )
        {
            Destroy(m_originTile.GetResourceTile().gameObject);
        }
        GetComponent<SpriteRenderer>().color = m_owner.GetColor();
        m_originTile.SetIsOccupied(true);
        transform.position = m_originTile.transform.position;
        transform.SetParent(m_originTile.transform);
        Instantiate(m_particleEffect, transform.position, transform.rotation);
        m_text.text = m_name = GameplayManager.GetCityName(m_owner.GetID());

        CreateStartBorder();
        CalculateEconomicScore();
    }

    void CreateStartBorder()
    {
        m_availableTiles = Tile.GetTilesInRange(m_originTile, 3);
        m_availableTiles.Reverse();

        for (int i = 0; i < 7; i++)
        {
            int lastIndex = m_availableTiles.Count - 1;

            SetupBorderTile(m_availableTiles[lastIndex]);
            m_availableTiles.RemoveAt(lastIndex);
        }

        m_nextExpansion = (m_borderTiles.Count + 3) - m_population;
    }

    BorderTile CreateBoarderTile(Tile t_tile)
    {
        if (t_tile.GetOwnerID() == -1)
        {
            BorderTile borderTile = new BorderTile();

            borderTile.m_tile = t_tile;
            borderTile.m_tile.SetOwnerID(GetOwnerID());
            borderTile.m_object = Instantiate(m_borderTileImage, t_tile.GetPostion(), t_tile.transform.rotation);

            Color borderColor = m_owner.GetColor();

            borderTile.m_object.GetComponent<SpriteRenderer>().color = borderColor;

            return borderTile;
        }

        return null;
    }

    bool SetupBorderTile(Tile t_tile)
    {
        BorderTile borderTile = CreateBoarderTile(t_tile);

        if (borderTile != null)
        {
            borderTile.m_object.transform.SetParent(transform);
            m_food += borderTile.m_tile.GetFood();
            m_production += borderTile.m_tile.GetProduction();
            m_borderTiles.Add(borderTile);

            if (borderTile.m_tile.GetResourceTile() != null)
            {
                m_resourceTiles.Add(borderTile.m_tile);
            }

            return true;
        }

        return false;
    }

    public void TurnStart()
    {
        PopulationGrowth();
        BorderExpansion();

        m_owner.AddResource("Gold", CalculateGoldProduction());
        
        foreach (ResourceBuilding resourceBuilding in m_resourceBuildings)
        {
            resourceBuilding.ProduceResource();
        }

        CalculateEconomicScore();
        UpdateHealth();
        RegenHealth();
    }

    public void TurnEnd()
    {
        if (m_itemInQueue)
        {
            if (m_queueCounter < m_maxTimeToBuild)
            {
                m_queueCounter++;
            }
            else
            {
                BuildItem();
                m_queueCounter = 0;
            }
        }
    }

    void BorderExpansion()
    {
        m_borderExpansionCount++;

        if (m_availableTiles.Count != 0)
        {
            if (m_borderExpansionCount >= m_nextExpansion)
            {
                bool expanded = false;

                while (m_availableTiles.Count != 0 && !expanded)
                {
                    int lastIndex = m_availableTiles.Count - 1;

                    if (SetupBorderTile(m_availableTiles[lastIndex]))
                    {
                        expanded = true;
                    }

                    m_availableTiles.RemoveAt(lastIndex);
                }

                m_nextExpansion = (m_borderTiles.Count + 3) - m_population;
                m_borderExpansionCount = 0;
            }
        }
    }

    public void PopulationGrowth()
    {
        int m_foodConsumtion = m_population * 2;
        int surplus = m_food - m_foodConsumtion;

        if (surplus > 0)
        {
            if (m_foodBasket < m_maxFood)
            {
                m_foodBasket += surplus;

                if (m_foodBasket >= m_maxFood)
                {
                    m_population++;
                    m_foodBasket -= m_maxFood;
                    m_maxFood += 3;
                }
            }

            m_foodTurns = (int)Mathf.Max(1, Mathf.Ceil((m_maxFood - m_foodBasket) / surplus));
        }

        else
        {
            m_foodTurns = -1;
        }
    }

    public List<BorderTile> GetBorderTiles()
    {
        return m_borderTiles;
    }

    public int GetOwnerID()
    {
        return m_owner.GetID();
    }

    void UpdateHealth()
    {
        float percentage = m_currentHealth / (float)m_maxHealth;

        m_maxHealth = m_baseHealth + 20 * m_population;

        m_currentHealth = (int)(m_maxHealth * percentage);

        Vector2 size = m_healthbar.GetComponent<RectTransform>().sizeDelta;
        size.x = m_maxHealthWidth * percentage;

        m_healthbar.GetComponent<RectTransform>().sizeDelta = size;
    }

    void RegenHealth()
    {
        m_currentHealth += (int)(m_maxHealth * 0.05f);

        if(m_currentHealth > m_maxHealth)
        {
            m_currentHealth = m_maxHealth;
        }

        float percentage = m_currentHealth / (float)m_maxHealth;

        Vector2 size = m_healthbar.GetComponent<RectTransform>().sizeDelta;
        size.x = m_maxHealthWidth * percentage;

        m_healthbar.GetComponent<RectTransform>().sizeDelta = size;
    }

    public void TakeDamage(int t_takeDamage)
    {
        m_currentHealth -= t_takeDamage;

        if (m_currentHealth < 0)
        {
            m_currentHealth = 0;
        }

        float percentage = m_currentHealth / (float)m_maxHealth;

        Vector2 size = m_healthbar.GetComponent<RectTransform>().sizeDelta;
        size.x = m_maxHealthWidth * percentage;

        m_healthbar.GetComponent<RectTransform>().sizeDelta = size;
    }

    public int GetCurrentHealth()
    {
        return m_currentHealth;
    }

    public void Capture(Player t_player)
    {
        m_owner.RemoveCity(gameObject);
        m_owner = t_player;
        m_owner.AddCity(gameObject);
        GetComponent<SpriteRenderer>().color = m_owner.GetColor();
        Color borderColor = m_owner.GetColor();

        foreach (BorderTile borderTile in m_borderTiles)
        {
            borderTile.m_object.GetComponent<SpriteRenderer>().color = borderColor;
            borderTile.m_tile.SetOwnerID(GetOwnerID());
        }
    }

    public void RecieveResource(ResourceType t_resourceType, int t_resourceAmount)
    {
        switch (t_resourceType)
        {
            case ResourceType.Wood:
                m_owner.AddResource("Wood", t_resourceAmount);
                break;
            case ResourceType.Iron:
                m_owner.AddResource("Iron", t_resourceAmount);
                break;
            case ResourceType.Horses:
                m_owner.AddResource("Horses", t_resourceAmount);
                break;
            case ResourceType.Wheat:
                m_food += t_resourceAmount;
                break;
        }
    }
    public void AddResourceBuilding(ResourceBuilding t_resourceBuilding)
    {
        m_resourceBuildings.Add(t_resourceBuilding);
    }

    public void AddBuilding(Building t_building)
    {
        m_buildings.Add(t_building);
    }

    public Tile GetOriginTile()
    {
        return m_originTile;
    }

    public void RemoveResource(string t_resourceType, int t_amount)
    {
        switch (t_resourceType)
        {
            case "Wood":
                m_owner.RemoveResource(t_resourceType, t_amount);
                break;
            case "Iron":
                m_owner.RemoveResource(t_resourceType, t_amount);
                break;
            case "Horses":
                m_owner.RemoveResource(t_resourceType, t_amount);
                break;
            case "Wheat":
                m_food -= t_amount;
                break;
        }
    }

    public Player GetOwner()
    {
        return m_owner;
    }

    public void BuildItem()
    {
        switch (m_typeToBuild)
        {
            case "Building":
                CreateBuilding(m_typeToBuildID);
                m_itemInQueue = false;
                break;
            case "Resource Building":
                CreateResourceBuilding(m_typeToBuildID);
                m_itemInQueue = false;
                break;
            case "Unit":
                CreateUnit(m_typeToBuildID);
                m_itemInQueue = false;
                break;
        }
    }

    public void CreateBuilding(int t_type)
    {
        int numOfTries = 30;

        for (int i = 0; i < numOfTries; i++)
        {
            int index = Random.Range(0, m_borderTiles.Count);

            if ((m_borderTiles[index].m_tile.GetTileType() != TileType.Water && m_borderTiles[index].m_tile.GetTileType() != TileType.Mountain) && m_borderTiles[index].m_tile.GetIsOccupied() == false)
            {
                Building building = Instantiate(m_buildingPrefab).GetComponent<Building>();
                building.GetComponent<Building>().Initialize(m_borderTiles[index].m_tile, this, t_type);
                m_borderTiles[index].m_tile.SetIsOccupied(true);
                AddBuilding(building);
                EnableUnitButton(t_type);
                return;
            }
        }
    }

    public void CreateResourceBuilding(int t_type)
    {
        int numOfTries = 30;

        for (int i = 0; i < numOfTries; i++)
        {
            int index = Random.Range(0,m_resourceTiles.Count);

            if (m_resourceTiles[index].GetResourceTile().GetResourceType() == (ResourceType)t_type)
            {
                ResourceBuilding resourceBuilding = Instantiate(m_resourceBuildingPrefab).GetComponent<ResourceBuilding>();
                resourceBuilding.GetComponent<ResourceBuilding>().Initialize(m_resourceTiles[index], this);
                AddResourceBuilding(resourceBuilding);
                if (m_resourceTiles[index].GetResourceTile().GetResourceType() == ResourceType.Horses)
                {
                    m_enabledStables = true;
                }
                m_resourceTiles.RemoveAt(index);
                return;
            }
        }
    }

    public void CreateUnit(int t_unitID)
    {
        int numOfTries = 30;

        for (int i = 0; i < numOfTries; i++)
        {
            int index = Random.Range(0, m_originTile.GetNeighbours().Count);
            if ((m_originTile.GetNeighbours()[index].GetTileType() != TileType.Water && m_originTile.GetNeighbours()[index].GetTileType() != TileType.Mountain))
            {
                m_owner.AddUnitToPlayer(m_units[t_unitID], m_originTile.GetNeighbours()[index].GetTileID());
                return;
            }
        }
    }

    public void SetBuildItemStats(string t_type, int t_typeID, GameObject t_queueDisplay)
    {
        m_itemInQueue = true;
        m_typeToBuild = t_type;
        m_typeToBuildID = t_typeID;
        FindObjectOfType<CityUI>().ToggleButtons(false);

        switch (m_typeToBuild)
        {
            case "Building":
                m_itemInqueueName = m_owner.GetComponentInChildren<CityUI>().m_buildingButtons[t_typeID - 4].name;
                m_maxTimeToBuild = m_owner.GetComponentInChildren<CityUI>().m_buildingButtons[t_typeID - 4].GetComponent<QueueButton>().GetTimeToBuild();
                t_queueDisplay.GetComponent<QueueDisplay>().SetText(m_maxTimeToBuild, m_itemInqueueName, m_queueCounter);
                break;
            case "Resource Building":
                m_maxTimeToBuild = m_owner.GetComponentInChildren<CityUI>().m_resourceButtons[t_typeID].GetComponent<QueueButton>().GetTimeToBuild();
                m_itemInqueueName = m_owner.GetComponentInChildren<CityUI>().m_resourceButtons[t_typeID].name;
                t_queueDisplay.GetComponent<QueueDisplay>().SetText(m_maxTimeToBuild, m_itemInqueueName, m_queueCounter);
                break;
            case "Unit":
                m_itemInqueueName = m_owner.GetComponentInChildren<CityUI>().m_unitButtons[t_typeID].name;
                m_maxTimeToBuild = m_owner.GetComponentInChildren<CityUI>().m_unitButtons[t_typeID].GetComponent<QueueButton>().GetTimeToBuild();
                t_queueDisplay.GetComponent<QueueDisplay>().SetText(m_maxTimeToBuild, m_itemInqueueName, m_queueCounter);
                break;
        }
    }

    public void EnableUnitButton(int t_type)
    {
        if (t_type == 4)
        {
            m_enabledBarracks = true;
        }
        else if (t_type == 5)
        {
            m_enabledWorkshop = true;
        }
    }

    public int CalculateGoldProduction()
    {
        return Mathf.CeilToInt((m_production * 0.2f) * 8 / m_population);
    }

    void CalculateEconomicScore()
    {
        m_economicScore = 20;

        m_economicScore += m_population * 5;
        m_economicScore += m_production * 2;

        m_economicScore += m_resourceBuildings.Count * 40;

        if(m_enabledBarracks)
        {
            m_economicScore += 30;
        }

        if (m_enabledWorkshop)
        {
            m_economicScore += 30;
        }

        if (m_enabledStables)
        {
            m_economicScore += 30;
        }
    }

    public int GetEconomicScore()
    {
        return m_economicScore;
    }
}

