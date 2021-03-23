using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Player : MonoBehaviour
{
    int m_id = -1;

    Color m_colour;

    [SerializeField]
    PlayerInventory m_inventory;

    [SerializeField]
    GameObject m_tileHighlight;

    public List<GameObject> m_units = new List<GameObject>();
    public List<GameObject> m_cities = new List<GameObject>();

    Unit m_selectedUnit;

    MapGrid m_mapGrid;

    [SerializeField]
    CityUI m_cityUI;

    [HideInInspector]
    public bool m_isAI = false;

    int m_militaryStrength = 0;
    int m_economicStrength = 0;

    [SerializeField]
    GameObject m_pathCanvas;

    public void Initialize(int t_id, Color t_colour, MapGrid t_mapGrid)
    {
        m_id = t_id;
        m_colour = t_colour;
        m_mapGrid = t_mapGrid;

        AddResource("Gold", 1);
        m_pathCanvas = Instantiate(m_pathCanvas);
        m_pathCanvas.name = "PathCanvas Player:" + m_id;

    }

    void Update()
    {
        if (GameplayManager.s_currentPlayerTurn == m_id)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Vector3 position = Input.mousePosition;
                    position.z = 1.0f;
                    position = Camera.main.ScreenToWorldPoint(position);

                    MapIndex index = m_mapGrid.PositionToMapIndex(new Vector2(position.x, position.y));

                    if (!m_cityUI.GetIsActive())
                    {
                        foreach (GameObject cityObj in m_cities)
                        {
                            if (cityObj.GetComponent<City>().GetOriginTile() == m_mapGrid.GetTile(index))
                            {
                                if (cityObj.GetComponent<City>().GetOriginTile() == m_mapGrid.GetTile(index))
                                {
                                    m_cityUI.SetValues(cityObj.GetComponent<City>());
                                    break;
                                }
                            }
                        }
                    }

                    SelectUnit(index);
                }            
            }

            if (m_selectedUnit != null)
            {
                if (m_selectedUnit.GetUnitType() == UnitType.Settler)
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        m_selectedUnit.Kill();
                    }
                }
            }
        }
    }

    public void TurnStart()
    {
        m_selectedUnit = null;
        ClearHighlightPath();

        for (int i = m_cities.Count - 1; i >= 0; i--)
        {
            m_cities[i].GetComponent<City>().TurnStart();
        }

        PayForUnitUpkeep();

        for (int i = m_units.Count - 1; i >= 0; i--)
        {
            m_units[i].GetComponent<Unit>().TurnStart();
        }

        CalculateEconomicStrength();
        CalculateMilitaryStrength();
    }

    public void TurnEnd()
    {
        for (int i = m_cities.Count - 1; i >= 0; i--)
        {
            m_cities[i].GetComponent<City>().TurnEnd();
        }

        for (int i = m_units.Count - 1; i >= 0; i--)
        {
            m_units[i].GetComponent<Unit>().TurnEnd();
        }

        if(m_selectedUnit != null)
        {
            m_selectedUnit.SetSelect(false);
            m_selectedUnit = null;
        }

        ClearHighlightPath();
    }

    public int GetID()
    {
        return m_id;
    }

    public Color GetColor()
    {
        return m_colour;
    }

    public bool AddResource(string t_name, int t_amount)
    {
        if (m_inventory.m_resources.ContainsKey(t_name))
        {
            m_inventory.m_resources[t_name] += t_amount;
            return true;
        }

        return false;
    }

    public bool SetResourceAmount(string t_name, int t_amount)
    {
        if (m_inventory.m_resources.ContainsKey(t_name))
        {
            m_inventory.m_resources[t_name] = t_amount;
            return true;
        }

        return false;
    }

    public bool RemoveResource(string t_name, int t_amount)
    {
        if (m_inventory.m_resources.ContainsKey(t_name))
        {
            m_inventory.m_resources[t_name] -= t_amount;
            return true;
        }

        return false;
    }

    public int GetResourceAmount(string t_name)
    {
        if (m_inventory.m_resources.ContainsKey(t_name))
        {
            return m_inventory.m_resources[t_name];
        }

        return -123456789;
    }

    public bool AddCity(GameObject t_city)
    {
        if (!m_cities.Contains(t_city))
        {
            m_cities.Add(t_city);
            return true;
        }

        return false;
    }

    public bool RemoveCity(GameObject t_city)
    {
        if (m_cities.Contains(t_city))
        {
            m_cities.Remove(t_city);
            return true;
        }

        return false;
    }

    public bool AddUnit(GameObject t_unit)
    {
        if (!m_units.Contains(t_unit))
        {
            m_units.Add(t_unit);
            return true;
        }

        return false;
    }

    public bool RemoveUnit(GameObject t_unit)
    {
        if (m_units.Contains(t_unit))
        {
            m_units.Remove(t_unit);
            return true;
        }

        return false;
    }

    public List<GameObject> GetUnits()
    {
        return m_units;
    }

    public List<GameObject> GetCities()
    {
        return m_cities;
    }

    public void SelectUnit(MapIndex t_index)
    {
        Tile clickedTile = m_mapGrid.GetTile(t_index);
        Unit clickedUnit = clickedTile.GetUnitInTile();

        if (m_selectedUnit == null)
        {
            if (clickedUnit != null)
            {
                if (clickedUnit.GetOwnerID() == m_id)
                {
                    m_selectedUnit = clickedUnit;
                    m_selectedUnit.SetSelect(true);
                    SetHighLightPath();

                    Debug.Log("Unit selected" + m_selectedUnit.name);
                }
            }
        }
        else
        {
            if (clickedUnit == null || m_selectedUnit != clickedUnit)
            {
                List<Tile> path = m_mapGrid.CreatePath(m_selectedUnit.GetCurrentTile(), clickedTile, m_selectedUnit);

                m_selectedUnit.SetPath(path, clickedTile);
                m_selectedUnit.SetSelect(false);
                m_selectedUnit = null;
                ClearHighlightPath();
            }
            else
            {
                m_selectedUnit.SetSelect(false);
                m_selectedUnit = null;
                ClearHighlightPath();
            }
        }
    }

    public void AddUnitToPlayer(GameObject t_unit, MapIndex t_tileID)
    {
        GameObject unitObj = Instantiate(t_unit);

        Unit unit = unitObj.GetComponent<Unit>();

        unit.Initialize(this);

        bool m_unitAdded = false;

        if (!m_mapGrid.GetIsOutOfBounds(t_tileID))
        {
            Tile newTile = m_mapGrid.GetTile(t_tileID);

            if (m_mapGrid.GetTile(t_tileID).AddUnitToTile(unit))
            {
                AddUnit(unitObj);
                m_unitAdded = true;
            }
            else
            {
                List<Tile> neighbours = newTile.GetNeighbours();

                foreach (Tile tile in neighbours)
                {
                    if (!tile.GetIsOccupied())
                    {
                        if (tile.AddUnitToTile(unit))
                        {
                            AddUnit(unitObj);
                            m_unitAdded = true;
                            break;
                        }
                    }
                }
            }
        }

        if (!m_unitAdded)
        {
            Debug.Log("No valid tile available. Deleting unit.");
            Destroy(unitObj);
        }
    }

    void SetHighLightPath()
    {
        ClearHighlightPath();

        if (m_selectedUnit != null)
        {
            List<Tile> path = m_selectedUnit.GetPath();
            int speed = m_selectedUnit.GetSpeed();
            GameObject tileH = m_tileHighlight;
            tileH.GetComponent<SpriteRenderer>().color = m_colour;
            int turns = 1;
            path.Reverse();
            for (int i = 0; i < path.Count; i++)
            {
                speed--;
                if (speed == 0)
                {
                    tileH.GetComponentInChildren<TextMesh>().text = "" + turns;

                    turns++;
                    speed = m_selectedUnit.GetSpeed();
                }
                else
                {

                    tileH.GetComponentInChildren<TextMesh>().text = "";

                }
                if (i == path.Count - 1)
                {
                    tileH.GetComponentInChildren<TextMesh>().text = "" + turns;
                }
                GameObject highlight = Instantiate(tileH, path[i].GetPostion(), m_tileHighlight.transform.rotation);
                highlight.name = "Player " + m_id + " Unit Path";
                highlight.transform.SetParent(m_pathCanvas.transform);
            }
            path.Reverse();
        }
    }

    void ClearHighlightPath()
    {
        for (int i = 0; i < m_pathCanvas.transform.childCount; i++)
        {
            Destroy(m_pathCanvas.transform.GetChild(i).gameObject);
        }
    }

    public bool CheckIfPlayerLost()
    {
        if (m_cities.Count == 0)
        {
            bool hasSettler = false;

            foreach (GameObject unitObj in m_units)
            {
                if (unitObj.GetComponent<Unit>().GetUnitType() == UnitType.Settler)
                {
                    hasSettler = true;
                    break;
                }
            }

            if (!hasSettler)
            {
                return true;
            }
        }

        return false;
    }

    void PayForUnitUpkeep()
    {
        int gold = GetResourceAmount("Gold");
        int priceOFUpkeep = 0;

        foreach (GameObject unit in m_units)
        {
            priceOFUpkeep += unit.GetComponent<Unit>().GetUpkeepCost();
        }

        if (priceOFUpkeep > gold)
        {
            int priceOfUnit = 0;
            // not enough gold
            for (int i = m_units.Count - 1; i >= 0; i--)
            {
                priceOfUnit = m_units[i].GetComponent<Unit>().GetUpkeepCost();
                if (priceOfUnit != 0)
                {
                    GameObject unit = m_units[i]; // use this to destroy unit after it has been removed from list
                    if (RemoveUnit(m_units[i]))
                    {
                        DestroyImmediate(unit, true);
                        priceOFUpkeep -= priceOfUnit;
                        if (priceOFUpkeep <= gold)
                        {
                            break;
                        }
                    }
                }
            }
        }

        gold -= priceOFUpkeep;

        SetResourceAmount("Gold", gold);
    }

    void CalculateMilitaryStrength()
    {
        m_militaryStrength = 0;

        for (int i = m_units.Count - 1; i >= 0; i--)
        {
            Unit unit = m_units[i].GetComponent<Unit>();

            if (unit.GetUnitType() != UnitType.Settler)
            {
                m_militaryStrength += unit.GetUnitScore();
            }
        }
    }

    void CalculateEconomicStrength()
    {
        m_economicStrength = 0;

        for (int i = m_cities.Count - 1; i >= 0; i--)
        {
            City city = m_cities[i].GetComponent<City>();

            m_economicStrength += city.GetEconomicScore();
        }
    }

    public int GetEconomicStrength()
    {
        return m_economicStrength;
    }

    public int GetMilitaryStrength()
    {
        return m_militaryStrength;
    }
}

