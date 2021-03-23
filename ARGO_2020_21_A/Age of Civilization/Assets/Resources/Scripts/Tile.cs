using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Mountain,
    Land,
    Sand,
    Water
}

public class Tile : MonoBehaviour
{
    public const float m_WEIGHT_COST = 2.61f;

    float m_pathCost = 0.0f;
    float m_heuristicCost = 0.0f;

    bool m_isMarked = false;

    bool m_isOccupied = false;

    Vector2 m_position = Vector2.zero;

    TileType m_tileType = TileType.Water;
   
    MapIndex m_tileID;

    ResourceTile m_resource = null;

    SpriteRenderer m_spriteRenderer;

    Tile m_previous;

    List<Tile> m_neighbours = new List<Tile>();
    
    int m_production = 0;
    int m_food = 0;
    int m_ownerID = -1;

    void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        m_position = transform.position;
    }

    public Vector2 GetPostion()
    {
        return m_position;
    }

    public void SetPosition(Vector2 t_position)
    {
        m_position = t_position;

        transform.position = m_position;
    }

    public TileType GetTileType()
    {
        return m_tileType;
    }

    public void SetTileType(TileType t_tileType)
    {
        m_tileType = t_tileType;

        switch (m_tileType)
        {
            case TileType.Mountain:
                m_production = 0;
                m_food = 0;
                break;
            case TileType.Land:
                m_production = 2;
                m_food = 2;
                break;
            case TileType.Sand:
                m_production = 1;
                m_food = 0;
                break;
            case TileType.Water:
                m_production = 0;
                m_food = 0;
                break;
            default:
                break;
        }
    }

    public void SetSprite(string t_spriteName)
    {
        Sprite sprite = Resources.Load<Sprite>("Sprites/" + t_spriteName);

        m_spriteRenderer.sprite = sprite;
    }

    public void AddNeighbour(Tile t_tile)
    {
        m_neighbours.Add(t_tile);
    }

    public bool IsInNeighbours(Tile t_tile)
    {
        return m_neighbours.Contains(t_tile);
    }

    public List<Tile> GetNeighbours()
    {
        return m_neighbours;
    }

    public void SetPathCost(float t_pathCost)
    {
        m_pathCost = t_pathCost;
    }

    public float GetPathCost()
    {
        return m_pathCost;
    }

    public void SetHeuristicCost(float t_heuristicCost)
    {
        m_heuristicCost = t_heuristicCost;
    }

    public float GetHeuristicCost()
    {
        return m_heuristicCost;
    }

    public void SetIsMarked(bool t_isMarked)
    {
        m_isMarked = t_isMarked;
    }

    public bool GetIsMarked()
    {
        return m_isMarked;
    }
    public void SetPrevious(Tile t_tile)
    {
        m_previous = t_tile;
    }

    public Tile GetPrevious()
    {
        return m_previous;
    }

    public void SetProduction(int t_production)
    {
        m_production = t_production;
    }

    public int GetProduction()
    {
        return m_production;
    }

    public void SetFood(int t_food)
    {
        m_food = t_food;
    }

    public int GetFood()
    {
        return m_food;
    }

    public void SetOwnerID(int t_ownerID)
    {
        m_ownerID = t_ownerID;
    }

    public int GetOwnerID()
    {
        return m_ownerID;
    }

    public ResourceTile GetResourceTile()
    {
        return m_resource;
    }

    public void SetResourceTile(ResourceTile t_resource)
    {
        m_resource = t_resource;
    }

    public void SetTileID(MapIndex t_tileID)
    {
        m_tileID = t_tileID;
    }

    public MapIndex GetTileID()
    {
        return m_tileID;
    }

    public void RemoveUnitFromTile(Unit t_unit)
    {
        t_unit.transform.SetParent(null);

        Unit unit = GetComponentInChildren<Unit>();

        if(unit != null)
        {
            unit.transform.position = transform.position;
        }
    }

    public bool AddUnitToTile(Unit t_unit)
    {
        if (t_unit.GetTraversableTiles().Contains(m_tileType))
        {
            Unit[] units = GetComponentsInChildren<Unit>();

            if(units.Length != 0 && units.Length != 2)
            {
                foreach (Unit unit in units)
                {
                    if (unit.GetOwnerID() == t_unit.GetOwnerID())
                    {
                        if (unit.GetUnitType() != t_unit.GetUnitType())
                        {
                            unit.transform.position = transform.position - Vector3.left * 0.6f;
                            t_unit.transform.position = transform.position + Vector3.left * 0.6f;
                            t_unit.SetCurrentTile(this);
                            return true;
                        }
                    }
                    else
                    {
                        if (unit.GetUnitType() == UnitType.Settler && t_unit.GetUnitType() == UnitType.CombatUnit)
                        {
                            //Settler Unit has gotten captured by an enemy combat unit.
                            unit.GetPlayer().RemoveUnit(unit.gameObject);
                            unit.Initialize(t_unit.GetPlayer());
                            unit.transform.position = transform.position - Vector3.left * 0.6f;
                            t_unit.transform.position = transform.position + Vector3.left * 0.6f;
                            t_unit.SetCurrentTile(this);
                            return true;
                        }
                    }
                }
            }
            else if(units.Length == 0)
            {
                t_unit.transform.position = transform.position;
                t_unit.SetCurrentTile(this);
                return true;
            }
        }

        return false;
    }

    public Unit GetUnitInTile()
    {
        return GetComponentInChildren<Unit>();
    }

    public City GetCityInTile()
    {
        return GetComponentInChildren<City>();
    }

    public bool GetIsTraversable(Unit t_unit)
    {
        if(t_unit.GetTraversableTiles().Contains(m_tileType))
        {
            City city = GetComponentInChildren<City>();

            if(city != null)
            {
                if(city.GetOwnerID() != t_unit.GetOwnerID())
                {
                    if(city.GetCurrentHealth() != 0)
                    {
                        return false;
                    }
                }
            }

            Unit[] units = GetComponentsInChildren<Unit>();

            foreach(Unit unit in units)
            {
                if(unit.GetOwnerID() == t_unit.GetOwnerID())
                {
                    if(unit.GetUnitType() == t_unit.GetUnitType())
                    {
                        return false;
                    }
                }
                else
                {
                    if (unit.GetUnitType() != UnitType.Settler || t_unit.GetUnitType() != UnitType.CombatUnit)
                    {
                        return false;
                    }
                }
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    static public List<Tile> GetTilesInRange(Tile t_startTile, int t_range)
    {
        List<Tile> tilesInRange = new List<Tile>();
        Stack<Tile> curLayer = new Stack<Tile>();
        Stack<Tile> nextLayer;
        
        curLayer.Push(t_startTile);

        while (t_range != 0)
        {
            t_range--;
            nextLayer = new Stack<Tile>();

            while (curLayer.Count != 0)
            {
                Tile tile = curLayer.Pop();
                tilesInRange.Add(tile);

                foreach (Tile neigbhourTile in tile.GetNeighbours())
                {
                    if (!tilesInRange.Contains(neigbhourTile) && !curLayer.Contains(neigbhourTile) && !nextLayer.Contains(neigbhourTile))
                    {
                        nextLayer.Push(neigbhourTile);
                    }
                }
            }

            curLayer = nextLayer;
        }

        while (curLayer.Count != 0)
        {
            tilesInRange.Add(curLayer.Pop());
        }

        return tilesInRange;
    }

    public void SetIsOccupied(bool t_val)
    {
        m_isOccupied = t_val;
    }

    public bool GetIsOccupied()
    {
        return m_isOccupied;
    }
    public static float CalculateTotalCost(float t_heuristic, float t_path)
    {
        return t_heuristic + t_path;
    }
}
