using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettlerActionType
{
    Move,
    Settle,
    Hide,
}

public struct SettlerAction
{
    public Unit m_unit;
    public Tile m_targetTile;

    public SettlerActionType m_unitActionType;
}

public class PlayerAI : MonoBehaviour
{
    [SerializeField]
    Player m_player;

    int m_id = -1;

    public MapGrid m_mapGrid;

    public List<Player> m_otherPlayers = new List<Player>();

    List<SettlerAction> m_unitActions = new List<SettlerAction>();
    List<CombatUnitAction> m_combatActions = new List<CombatUnitAction>();
    List<CityAction> m_cityActions = new List<CityAction>();

    BehaviourAI m_behaviour = new BehaviourAI();

    GameplayManager m_gameplayManager;

    public void Initialize(int t_id, Color t_colour, MapGrid t_mapGrid)
    {
        m_id = t_id;

        m_mapGrid = t_mapGrid;
        m_player.Initialize(m_id, t_colour, t_mapGrid);
        m_behaviour.Initialize(this);
    }

    public void AddOtherPlayer(Player t_otherPlayer)
    {
        m_otherPlayers.Add(t_otherPlayer);
    }

    public void SetGameplayManager(GameplayManager t_gameplayManager)
    {
        m_gameplayManager = t_gameplayManager;
    }

    public IEnumerator DoTurn()
    {
        yield return new WaitForSeconds(0.5f);

        m_behaviour.DoCalculations();

        CleanUpActions();

        List<GameObject> cities = m_player.GetCities();

        for (int i = cities.Count - 1; i >= 0; i--)
        {
            City city = cities[i].GetComponent<City>();

            ProcessCity(city);
        }

        List<GameObject> units = m_player.GetUnits();

        for (int i = units.Count - 1; i >= 0; i--)
        {
            Unit unit = units[i].GetComponent<Unit>();

            if (unit.GetUnitType() == UnitType.Settler)
            {
                ProcessSettler(unit);
            }
            else
            {
                ProcessCombatUnit(unit.GetComponent<CombatUnit>());
            }
        }

        foreach(CombatUnitAction combatUnitAction in m_combatActions)
        {
            combatUnitAction.ProcessTurn();
        }

        foreach(CityAction cityAction in m_cityActions)
        {
            cityAction.ProcessTurn();
        }

        if(m_gameplayManager != null)
        {
            m_gameplayManager.ChangeCurrentTurn();
        }
    }

    void ProcessSettler(Unit t_settler)
    {
        if(t_settler.GetPath().Count == 0)
        {
            foreach(SettlerAction unitAction in m_unitActions)
            {
                if(unitAction.m_unit == t_settler)
                {
                    if(t_settler.GetCurrentTile() == unitAction.m_targetTile &&
                        unitAction.m_unitActionType == SettlerActionType.Settle)
                    {
                        t_settler.Kill();
                        m_unitActions.Remove(unitAction);
                        return;
                    }
                    else
                    {
                        m_unitActions.Remove(unitAction);
                        break;
                    }
                }
            }

            Tile destTile = CalculateBestSettleTile(t_settler);

            SettlerAction action;
            action.m_unit = t_settler;
            action.m_targetTile = destTile;
            action.m_unitActionType = SettlerActionType.Settle;

            t_settler.SetPath(m_mapGrid.CreatePath(t_settler.GetCurrentTile(), destTile, t_settler), destTile);

            m_unitActions.Add(action);
        }
    }

    void ProcessCity(City t_city)
    {
        bool cityActionFound = false;

        for (int i = 0; i < m_cityActions.Count && !cityActionFound; i++)
        {
            if (m_cityActions[i].GetCity() == t_city)
            {
                cityActionFound = true;
            }
        }

        if (!cityActionFound)
        {
            CityAction cityAction = new CityAction();
            cityAction.Initilaize(this, t_city);
            m_cityActions.Add(cityAction);
        }
    }

    void ProcessCombatUnit(CombatUnit t_combatUnit)
    {
        bool combatActionFound = false;

        for(int i = 0; i < m_combatActions.Count && !combatActionFound; i++)
        {
            if(m_combatActions[i].m_unit == t_combatUnit)
            {
                combatActionFound = true;
            }
        }

        if(!combatActionFound)
        {
            CombatUnitAction combatUnitAction = new CombatUnitAction();
            combatUnitAction.Initilaize(this, t_combatUnit);
            m_combatActions.Add(combatUnitAction);
        }
    }

    void CleanUpActions()
    {
        int actionCount = m_combatActions.Count;

        for (int i = actionCount - 1; i >= 0; i--)
        {
            if(m_combatActions[i].m_unit == null)
            {
                m_combatActions.RemoveAt(i);
            }
        }

        actionCount = m_cityActions.Count;

        for (int i = actionCount - 1; i >= 0; i--)
        {
            if (m_cityActions[i].GetCity().GetOwnerID() != m_id)
            {
                m_cityActions.RemoveAt(i);
            }
        }
    }

    public List<Unit> GetActiveSettlers()
    {
        List<Unit> settlers = new List<Unit>();

        foreach(GameObject unitObj in m_player.GetUnits())
        {
            if(unitObj.GetComponent<Unit>().GetUnitType() == UnitType.Settler)
            {
                settlers.Add(unitObj.GetComponent<Unit>());
            }
        }

        return settlers;
    }

    public Tile CalculateBestSettleTile(Unit t_settler)
    {
        Tile startTile = t_settler.GetCurrentTile();
        List<Tile> tiles = Tile.GetTilesInRange(startTile, 6);

        int bestValue = -10000;       
        Tile bestTile = null;

        int unitSpeed = t_settler.GetSpeed();

        foreach (Tile tile in tiles)
        {
            int curValue = 0;

            if (tile.GetIsTraversable(t_settler))
            {
                int dist = m_mapGrid.CreatePath(t_settler.GetCurrentTile(), tile, t_settler).Count;

                curValue -= 40 * (int)Mathf.Ceil((float)dist / unitSpeed);

                List<Tile> nearbyTiles = Tile.GetTilesInRange(tile, 2);
                nearbyTiles.Remove(tile);

                foreach (Tile nearbyTile in nearbyTiles)
                {
                    if(nearbyTile.GetCityInTile() != null)
                    {
                        curValue = -10000;
                        break;
                    }
                    else
                    {
                        curValue += nearbyTile.GetProduction();
                        curValue += nearbyTile.GetFood();
                        
                        if(nearbyTile.GetResourceTile() != null)
                        {
                            curValue += 100;
                        }
                    }
                }
            }
            else
            {
                curValue = -10000;
            }

            if(curValue > bestValue)
            {
                bestTile = tile;
                bestValue = curValue;
            }
        }

        return bestTile;
    }

    public int GetID()
    {
        return m_id;
    }

    public Player GetPlayer()
    {
        return m_player;
    }

    public UnitTactic GetUnitTactic()
    {
        return m_behaviour.GetUnitTactic();
    }

    public CityTactic GetCityTactic()
    {
        return m_behaviour.GetCityTactic();
    }

    public Player GetTargetPlayer()
    {
        return m_behaviour.GetTargetPlayer();
    }
}
