using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatUnitType
{
    Scout,
    Archer,
    Spearman,
    Horseman,
    ArcherHorseman,
    Catapult,
    BatteringRam,
    Warrior
}

public class CombatUnit : Unit
{
    public CombatUnitType m_combatUnitType;
    Tile m_targetTile;

    public override void TurnEnd()
    {   
        Attack();
        Move();
    }

    public override void SetPath(List<Tile> t_path, Tile t_tile)
    {
        m_path = t_path;
       
        SetTarget(t_tile);
        Attack();
        Move();
    }

    public override void Move()
    {    
        List<Vector3> smoothMovePath = new List<Vector3>();
        smoothMovePath.Add(transform.position);
        
        if (m_path.Count > 0)
        {
            bool reachedEnd = false; 
           
            while (m_movementLeft != 0 && !reachedEnd)
            {
                if (m_path[m_path.Count - 1].AddUnitToTile(this))
                {
                    smoothMovePath.Add(transform.position);
                    m_path.RemoveAt(m_path.Count - 1);
                    m_movementLeft--;
                }
                else
                {
                    m_path.Clear();
                }

                Attack();

                if (m_path.Count == 0)
                {
                    reachedEnd = true;
                    CaptureCity();
                }
            }
            if (!m_isAI)
            {
                SetMovementPositions(smoothMovePath);
            }

        }
        
    }

    void CaptureCity()
    {
        City city = m_currentTile.GetCityInTile();

        if (city != null)
        {
            if (city.GetOwnerID() != m_player.GetID())
            {
                city.Capture(m_player);
            }
        }
    }

    void SetTarget(Tile t_tile)
    {
        m_targetTile = null;

        City city = m_currentTile.GetCityInTile();
        Unit unit = m_currentTile.GetUnitInTile();

        if(city != null || unit != null)
        {
            m_targetTile = t_tile;
        }
    }

    void Attack()
    {
        if (m_targetTile != null && m_movementLeft != 0)
        {
            if (Tile.GetTilesInRange(m_currentTile, m_stats.m_attackRange).Contains(m_targetTile))
            {
                City city = m_targetTile.GetCityInTile();
                Unit unit = m_targetTile.GetUnitInTile();

                if (city != null)
                {
                    if (city.GetOwnerID() != m_player.GetID())
                    {
                        if (city.GetCurrentHealth() != 0)
                        {
                            city.TakeDamage(m_stats.m_structureDamage);
                            m_movementLeft = 0;
                            m_path.Clear();
                            
                            m_targetTile = null;
                            return;
                        }
                    }
                }

                if (unit != null)
                {
                    if (unit.GetOwnerID() != m_player.GetID())
                    {
                        if(unit.GetUnitType() == UnitType.CombatUnit)
                        {
                            unit.TakeDamage(m_stats.m_damage);
                            m_movementLeft = 0;
                            
                            m_path.Clear();
                            m_targetTile = null;
                            return;
                        }
                    }
                }
            }
        }
    }

    public CombatUnitType GetCombatUnitType()
    {
        return m_combatUnitType;
    }
}
