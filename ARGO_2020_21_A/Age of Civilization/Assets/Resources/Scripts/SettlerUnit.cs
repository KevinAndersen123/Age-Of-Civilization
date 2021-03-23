using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlerUnit : Unit
{
    [SerializeField]
    GameObject m_cityPrefab;

    [SerializeField]
    int m_minSettleDistance;

    public override void TakeDamage(int t_damage)
    {
        Debug.Log("Settler can't take damage");
    }

    public override void Kill()
    {
        List <Tile> tilesInRange = Tile.GetTilesInRange(m_currentTile, m_minSettleDistance);

        foreach(Tile tile in tilesInRange)
        {
            if(tile.GetCityInTile() != null)
            {
                return;
            }
        }

        GameObject cityObj = Instantiate(m_cityPrefab);

        cityObj.GetComponent<City>().Setup(m_currentTile, m_player);
        m_player.AddCity(cityObj);

        base.Kill();
    }
}
