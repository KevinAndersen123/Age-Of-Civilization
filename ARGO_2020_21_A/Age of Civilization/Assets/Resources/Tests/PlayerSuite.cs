using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerSuite
{
    GameObject m_playerObj;
    GameObject m_mapGridPrefab;

    Player m_playerScript;
    MapGrid m_mapGrid;
    string path = "Prefabs/Player";
    
    [OneTimeSetUp]
    public void Setup()
    {
        m_mapGridPrefab = Resources.Load<GameObject>("Prefabs/MapGrid");

        GameObject mapGridObject = GameObject.Instantiate(m_mapGridPrefab, Vector2.zero, Quaternion.identity);

        m_mapGrid = mapGridObject.GetComponent<MapGrid>();

        m_mapGrid.SetHeight(20);
        m_mapGrid.SetWidth(20);
        m_mapGrid.CreateMap();

        for (int x = 0; x < 20; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                m_mapGrid.m_grid[x][y].SetTileType(TileType.Land);
            }
        }

        if (m_playerObj == null)
        {
            GameObject playerPrefab = Resources.Load<GameObject>(path);
            m_playerObj = GameObject.Instantiate(playerPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity);
            m_playerScript = m_playerObj.GetComponent<Player>();
            m_playerScript.Initialize(1, Color.blue, m_mapGrid);
        }

        GameObject unitPrefab = Resources.Load<GameObject>("Prefabs/UnitPrefabs/Scout");

        m_playerScript.AddUnitToPlayer(unitPrefab, new MapIndex(1, 1));
    }

    [UnityTest, Order(0)]
    public IEnumerator UnitTakesUpkeep()
    {   
        m_playerScript.SetResourceAmount("Gold", 1);
        Assert.True(m_playerScript.GetResourceAmount("Gold") == 1);
        int unitCost = m_playerScript.GetUnits()[0].GetComponent<Unit>().GetUpkeepCost();
        Assert.True(unitCost == 1);
        m_playerScript.TurnStart();
        Assert.True(m_playerScript.GetResourceAmount("Gold") == 0);
        yield return null;
    }

    [UnityTest, Order(1)]
    public IEnumerator UnitRemovedWhenUpkeepNotMet()
    {
        m_playerScript.SetResourceAmount("Gold", 0);
        Assert.True(m_playerScript.GetResourceAmount("Gold") == 0);
        Assert.True(m_playerScript.GetUnits().Count == 1);

        m_playerScript.TurnStart();

        Assert.True(m_playerScript.GetUnits().Count == 0);
        Assert.True(m_playerScript.GetResourceAmount("Gold") == 0);
        yield return null;
    }


    [UnityTest, Order(2)]
    public IEnumerator MultipleUnitUpkeep()
    {
        GameObject unitPrefab = Resources.Load<GameObject>("Prefabs/UnitPrefabs/Scout");

        m_playerScript.AddUnitToPlayer(unitPrefab, new MapIndex(1, 1));
        unitPrefab = Resources.Load<GameObject>("Prefabs/UnitPrefabs/Spearman");
        m_playerScript.AddUnitToPlayer(unitPrefab, new MapIndex(1, 1));
        m_playerScript.SetResourceAmount("Gold", 1);
        Assert.True(m_playerScript.GetResourceAmount("Gold") == 1);
        Assert.True(m_playerScript.GetUnits().Count == 2);

        m_playerScript.TurnStart();
        Assert.True(m_playerScript.GetUnits().Count == 1);
        Assert.True(m_playerScript.GetResourceAmount("Gold") == 0);

        yield return null;
    }
}
