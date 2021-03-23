using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitTactic
{ 
    None,
    Defend,
    Attack
}

public enum CityTactic
{ 
    None,
    Upgrade,
    Expand,
    Combat
}

public class BehaviourAI
{
    PlayerAI m_playerAI;
    Player m_player;
    Player m_targetPlayer;

    MapGrid m_mapGrid;

    UnitTactic m_unitTactic = UnitTactic.None;

    CityTactic m_cityTactic = CityTactic.None;

    public void Initialize(PlayerAI t_playerAI)
    {
        m_playerAI = t_playerAI;
        m_player = m_playerAI.GetPlayer();
        m_mapGrid = m_playerAI.m_mapGrid;
    }

    public void DoCalculations()
    {
        CalculateUnitTactic();
        CalculateCityTactic();
    }

    void CalculateUnitTactic()
    {
        int ourStrength = m_player.GetMilitaryStrength();

        //Check if we have any units to do anything with.
        if(ourStrength != 0)
        {
            //Unit used to masure the distance to things.
            Unit unitForMeasuring = m_player.GetUnits()[0].GetComponent<Unit>();

            UnitTactic curTactic = m_unitTactic;
            Player curTargetPlayer = null;
            float score = 0.0f;

            foreach (Player player in m_playerAI.m_otherPlayers)
            {
                if(player.CheckIfPlayerLost())
                {
                    continue;
                }

                int distToClosestCity = int.MaxValue;
                int militaryPower = player.GetMilitaryStrength();

                foreach(GameObject ourCityObj in m_player.GetCities())
                {
                    City ourCity = ourCityObj.GetComponent<City>();

                    foreach (GameObject cityObj in player.GetCities())
                    {
                        City city = cityObj.GetComponent<City>();
                        int distToCity = m_mapGrid.CreatePath(ourCity.GetOriginTile(), city.GetOriginTile(), unitForMeasuring).Count;

                        if(distToClosestCity > distToCity)
                        {
                            distToClosestCity = distToCity;
                        }
                    }
                }

                //The player has no cities to attack them so we ignore them.
                if(distToClosestCity != int.MaxValue)
                {
                    float militaryPowerWeak = FuzzyLogic.FuzzyGradeDown(militaryPower, ourStrength + 20, ourStrength / 2);
                    float militaryPowerSame = FuzzyLogic.FuzzyTriangle(militaryPower, ourStrength, ourStrength * 1.25f, ourStrength * 1.5f);
                    float militaryPowerStrong = FuzzyLogic.FuzzyGradeUp(militaryPower, ourStrength * 1.4f, ourStrength * 2.0f);

                    float closeDistance = FuzzyLogic.FuzzyGradeDown(distToClosestCity, 8, 0);
                    float farDistance = FuzzyLogic.FuzzyGradeUp(distToClosestCity, 7, 15);

                    float defendValue = FuzzyLogic.FuzzyOR(
                        FuzzyLogic.FuzzyOR(
                            FuzzyLogic.FuzzyAND(militaryPowerWeak, farDistance), 
                            FuzzyLogic.FuzzyAND(militaryPowerSame, farDistance)),
                        FuzzyLogic.FuzzyNOT(
                            FuzzyLogic.FuzzyAND(militaryPowerStrong, closeDistance)));

                    float attackValue = FuzzyLogic.FuzzyOR(
                        FuzzyLogic.FuzzyOR(
                            FuzzyLogic.FuzzyNOT(
                                FuzzyLogic.FuzzyAND(militaryPowerWeak, farDistance)), 
                            FuzzyLogic.FuzzyNOT(
                                FuzzyLogic.FuzzyAND(militaryPowerSame, farDistance))), 
                            FuzzyLogic.FuzzyAND(militaryPowerStrong, closeDistance));

                    if(defendValue > attackValue)
                    {
                        if(score < defendValue)
                        {
                            score = defendValue;
                            curTargetPlayer = null;
                            curTactic = UnitTactic.Defend;
                        }
                    }
                    else
                    {
                        if (score < attackValue)
                        {
                            score = attackValue;
                            curTargetPlayer = player;
                            curTactic = UnitTactic.Attack;
                        }
                    }
                }
            }

            m_unitTactic = curTactic;
            m_targetPlayer = curTargetPlayer;
        }
        else
        {
            m_unitTactic = UnitTactic.None;
            m_targetPlayer = null;
        }
    }

    void CalculateCityTactic()
    {
        Player strongestMilitaryPlayer = m_player;
        Player weakestEconomicPlayer = m_player;
        Player mostCityPlayer = m_player;

        foreach(Player player in m_playerAI.m_otherPlayers)
        {
            if(player.GetMilitaryStrength() > strongestMilitaryPlayer.GetMilitaryStrength())
            {
                strongestMilitaryPlayer = player;
            }

            if(player.GetEconomicStrength() < strongestMilitaryPlayer.GetEconomicStrength())
            {
                weakestEconomicPlayer = player;
            }

            if(player.GetCities().Count > mostCityPlayer.GetCities().Count)
            {
                mostCityPlayer = player;
            }
        }

        if(strongestMilitaryPlayer == m_player)
        {
            if(mostCityPlayer == m_player)
            {
                m_cityTactic = CityTactic.Upgrade;
            }
            else
            {
                m_cityTactic = CityTactic.Expand;
            }
        }
        else if(weakestEconomicPlayer == m_player)
        {
            if(mostCityPlayer != m_player)
            {
                m_cityTactic = CityTactic.Expand;
            }
            else
            {
                m_cityTactic = CityTactic.Combat;
            }
        }
        else
        {
            m_cityTactic = CityTactic.Combat;
        }
    }

    public UnitTactic GetUnitTactic()
    {
        return m_unitTactic;
    }

    public CityTactic GetCityTactic()
    {
        return m_cityTactic;
    }

    public Player GetTargetPlayer()
    {
        return m_targetPlayer;
    }
}
