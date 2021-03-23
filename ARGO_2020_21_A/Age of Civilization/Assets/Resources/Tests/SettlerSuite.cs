using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SettlerSuite
{
    GameObject m_playerPrefab;
    GameObject m_settlerUnitPrefab;
    GameObject m_mapGridPrefab;

    SettlerUnit m_settlerUnit;

    MapGrid m_mapGrid;

    Player m_player;

    [OneTimeSetUp]
    public void SetUp()
    {
        m_settlerUnitPrefab = Resources.Load<GameObject>("Prefabs/UnitPrefabs/Settler");
        m_mapGridPrefab = Resources.Load<GameObject>("Prefabs/MapGrid");
        m_playerPrefab = Resources.Load<GameObject>("Prefabs/Player");

        GameObject mapGridObject = GameObject.Instantiate(m_mapGridPrefab, Vector2.zero, Quaternion.identity);
        m_mapGrid = mapGridObject.GetComponent<MapGrid>();
        m_mapGrid.SetSize(10, 15);
        m_mapGrid.CreateMap();

        for (int x = 1; x < 8; x++)
        {
            for (int y = 1; y < 13; y++)
            {
                m_mapGrid.m_grid[x][y].SetTileType(TileType.Land);
            }
        }

        GameObject playerObject = GameObject.Instantiate(m_playerPrefab);
        m_player = playerObject.GetComponent<Player>();

        m_player.Initialize(0, Color.red, m_mapGrid);
        m_player.TurnStart();
        m_player.AddUnitToPlayer(m_settlerUnitPrefab, new MapIndex(1, 1));
        m_settlerUnit = m_player.GetUnits()[m_player.GetUnits().Count - 1].GetComponent<SettlerUnit>();

        GameplayManager.InitCityNames();
    }

    [UnityTest, Order(1)]
    public IEnumerator MovementTest()
    {
        Tile startTile = m_mapGrid.GetTile(new MapIndex(1, 1));

        m_settlerUnit.SetCurrentTile(startTile);
        m_player.SelectUnit(new MapIndex(1, 1));
        m_player.SelectUnit(new MapIndex(3, 1));

        m_player.TurnEnd();

        Tile endTile = m_settlerUnit.GetCurrentTile();

        Assert.True(startTile != endTile);
        Assert.True(endTile == m_mapGrid.GetTile(new MapIndex(3, 1)));

        yield return null;
    }

    [UnityTest, Order(2)]
    public IEnumerator SettlerAndCombatInSameTile()
    {
        GameObject combatUnitPrefab = Resources.Load<GameObject>("Prefabs/UnitPrefabs/Scout");

        m_player.AddUnitToPlayer(combatUnitPrefab, m_settlerUnit.GetCurrentTile().GetTileID());

        CombatUnit combatUnit = m_player.GetUnits()[m_player.GetUnits().Count - 1].GetComponent<CombatUnit>();

        Assert.True(m_settlerUnit.GetCurrentTile() == combatUnit.GetCurrentTile());

        yield return null;
    }

    [UnityTest, Order(3)]
    public IEnumerator OneSettlerPerTile()
    {
        m_player.AddUnitToPlayer(m_settlerUnitPrefab, new MapIndex(2, 1));

        SettlerUnit settlerUnit2 = m_player.GetUnits()[m_player.GetUnits().Count - 1].GetComponent<SettlerUnit>();

        Assert.True(settlerUnit2.GetCurrentTile() == m_mapGrid.GetTile(new MapIndex(2, 1)));

        m_player.SelectUnit(new MapIndex(2, 1));
        m_player.SelectUnit(m_settlerUnit.GetCurrentTile().GetTileID());

        Assert.True(m_settlerUnit.GetCurrentTile() != settlerUnit2.GetCurrentTile());

        yield return null;
    }

    [UnityTest, Order(4)]
    public IEnumerator NoDamageTaken()
    {
        int curHealth = m_settlerUnit.GetCurrentHealth();

        Assert.True(curHealth != 0);

        m_settlerUnit.TakeDamage(10000);

        Assert.True(curHealth == m_settlerUnit.GetCurrentHealth());

        yield return null;
    }

    [UnityTest, Order(5)]
    public IEnumerator SettleDown()
    {
        m_settlerUnit.Kill();

        yield return new WaitForSeconds(1);

        Assert.True(m_settlerUnit == null);
        Assert.True(m_player.GetCities().Count == 1);

        yield return null;
    }

    [UnityTest, Order(6)]
    public IEnumerator TooCloseToSettle()
    {
        int cityCount = m_player.GetCities().Count;

        m_player.AddUnitToPlayer(m_settlerUnitPrefab, new MapIndex(3, 1));

        SettlerUnit settlerUnit2 = m_player.GetUnits()[m_player.GetUnits().Count - 1].GetComponent<SettlerUnit>();

        settlerUnit2.Kill();

        yield return new WaitForSeconds(1);

        Assert.True(settlerUnit2 != null);
        Assert.True(m_player.GetCities().Count == cityCount);

        yield return null;
    }
}
