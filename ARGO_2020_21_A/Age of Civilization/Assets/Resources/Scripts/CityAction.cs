using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityAction
{
    City m_city;
    PlayerAI m_playerAI;

    int m_randomDelay;

    public void Initilaize(PlayerAI t_playerAI, City t_city)
    {
        m_city = t_city;
        m_playerAI = t_playerAI;

        m_randomDelay = Random.Range(5, 10);
    }

    public void ProcessTurn()
    {
        switch (m_playerAI.GetCityTactic())
        {
            case CityTactic.Expand:
                Expand();
                break;
            case CityTactic.Upgrade:
                Upgrade();
                break;
            case CityTactic.Combat:
                Combat();
                break;
            case CityTactic.None:
                Combat();
                break;
            default:
                break;
        }
    }

    void Expand()
    {
        //Can not do expand logic as city code was made without considering that AI can not use buttons.
        BruteForceProduction();
    }

    void Upgrade()
    {
        //Can not do expand logic as city code was made without considering that AI can not use buttons.
        BruteForceProduction();
    }

    void Combat()
    {
        //Can not do expand logic as city code was made without considering that AI can not use buttons.
        BruteForceProduction();
    }

    void BruteForceProduction()
    {
        m_randomDelay--;
        if (m_randomDelay == 0)
        {
            m_city.CreateUnit(Random.Range(0, m_city.m_units.Length));
            m_randomDelay = Random.Range(4, 10);
        }
    }

    public City GetCity()
    {
        return m_city;
    }
}
