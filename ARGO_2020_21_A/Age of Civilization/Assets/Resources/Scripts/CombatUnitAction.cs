using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUnitAction 
{
    public CombatUnit m_unit = null;
    public GameObject m_targetObj = null;
    public Tile m_targetTile = null;

    PlayerAI m_playerAI;

    public void Initilaize(PlayerAI t_playerAI, CombatUnit t_combatUnit)
    {
        m_playerAI = t_playerAI;
        m_unit = t_combatUnit;
    }

    public void ProcessTurn()
    {
        //We attack the unit until it is killed evenif it means chasing it accross the map.
        if (m_targetObj != null && m_targetObj.tag == "CombatUnit")
        {
            Unit targetUnit = m_targetObj.GetComponent<Unit>();
            Tile targetTile = targetUnit.GetCurrentTile();

            m_unit.SetPath(m_playerAI.m_mapGrid.CreatePath(m_unit.GetCurrentTile(), targetTile, m_unit), targetTile);

            m_targetTile = targetTile;
        }
        else
        {
            switch (m_playerAI.GetUnitTactic())
            {
                case UnitTactic.Attack:
                    AttackCity();
                    break;
                case UnitTactic.Defend:
                    DefendCity();
                    break;
                case UnitTactic.None:
                    //We defend a city even if we do not have a tactic as there is nothing else to do.
                    DefendCity();
                    break;
                default:
                    break;
            }
        }
    }

    void AttackCity()
    {
        bool newTargetCity = true;

        if (m_targetObj != null)
        {
            City targetCity = m_targetObj.GetComponent<City>();

            if (targetCity.GetOwnerID() != m_playerAI.GetID())
            {
                newTargetCity = false;
            }
        }

        if (newTargetCity)
        {
            if (m_playerAI.GetTargetPlayer() != null)
            {
                List<GameObject> targetCities = m_playerAI.GetTargetPlayer().GetCities();

                City targetCity = null;

                foreach (GameObject city in targetCities)
                {
                    if (targetCity == null)
                    {
                        targetCity = city.GetComponent<City>();
                    }
                    else if (city.GetComponent<City>().GetCurrentHealth() < targetCity.GetCurrentHealth())
                    {
                        targetCity = city.GetComponent<City>();
                    }
                }

                if (targetCity != null)
                {
                    m_targetObj = targetCity.gameObject;
                    m_targetTile = targetCity.GetOriginTile();
                }
            }
        }

        if (m_targetObj != null)
        {
            List<Tile> nearbyTiles = Tile.GetTilesInRange(m_unit.GetCurrentTile(), m_unit.GetVisionRange());

            foreach (Tile tile in nearbyTiles)
            {
                Unit unit = tile.GetUnitInTile();

                if (unit != null)
                {
                    if (unit.GetUnitType() == UnitType.CombatUnit)
                    {
                        if (unit.GetOwnerID() != m_playerAI.GetID())
                        {
                            m_targetObj = unit.gameObject;
                            m_targetTile = unit.GetCurrentTile();
                            break;
                        }
                    }
                }
            }

            m_unit.SetPath(m_playerAI.m_mapGrid.CreatePath(m_unit.GetCurrentTile(), m_targetTile, m_unit), m_targetTile);
        }
    }

    void DefendCity()
    {
        bool newTargetCity = true;

        if (m_targetObj != null)
        {
            City targetCity = m_targetObj.GetComponent<City>();

            if (targetCity.GetOwnerID() == m_playerAI.GetID())
            {
                newTargetCity = false;
            }
        }

        if (newTargetCity)
        {
            List<GameObject> targetCities = m_playerAI.GetPlayer().GetCities();

            City targetCity = null;

            foreach (GameObject city in targetCities)
            {
                if (targetCity == null)
                {
                    targetCity = city.GetComponent<City>();
                    break;
                }
            }

            if (targetCity != null)
            {
                m_targetObj = targetCity.gameObject;
                m_targetTile = targetCity.GetOriginTile();
            }
        }

        if (m_targetObj != null)
        {
            List<BorderTile> borderTiles = m_targetObj.GetComponent<City>().GetBorderTiles();

            m_targetTile = borderTiles[Random.Range(0, borderTiles.Count)].m_tile;

            List<Tile> nearbyTiles = Tile.GetTilesInRange(m_unit.GetCurrentTile(), m_unit.GetVisionRange());

            foreach (Tile tile in nearbyTiles)
            {
                Unit unit = tile.GetUnitInTile();

                if (unit != null)
                {
                    if (unit.GetUnitType() == UnitType.CombatUnit)
                    {
                        if (unit.GetOwnerID() != m_playerAI.GetID())
                        {
                            m_targetObj = unit.gameObject;
                            m_targetTile = unit.GetCurrentTile();
                            break;
                        }
                    }
                }
            }

            m_unit.SetPath(m_playerAI.m_mapGrid.CreatePath(m_unit.GetCurrentTile(), m_targetTile, m_unit), m_targetTile);
        }
    }
}
