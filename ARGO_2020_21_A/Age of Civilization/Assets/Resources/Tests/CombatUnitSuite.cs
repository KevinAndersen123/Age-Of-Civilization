using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CombatUnitSuite
{
    CombatUnit m_combatUnit;
    GameObject m_combatUnitPrefab;

    // create map for testing movement
    MapGrid m_mapGrid;
    GameObject m_mapGridPrefab;
    GameObject m_mapGridObject;

    Player m_player;

    GameObject m_playerPrefab;
    GameObject m_playerObject;

    [OneTimeSetUp]
    public void SetUp()
    {
        m_combatUnitPrefab = Resources.Load<GameObject>("Prefabs/UnitPrefabs/Scout");
        m_mapGridPrefab = Resources.Load<GameObject>("Prefabs/MapGrid");
        m_mapGridObject = GameObject.Instantiate(m_mapGridPrefab, Vector2.zero, Quaternion.identity);

        m_mapGrid = m_mapGridObject.GetComponent<MapGrid>();
        m_mapGrid.SetSize(10, 15);
        m_mapGrid.CreateMap();

        for (int x = 1; x < 8; x++)
        {
            for (int y = 1; y < 13; y++)
            {
                m_mapGrid.m_grid[x][y].SetTileType(TileType.Land);
            }
        }

        m_playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        m_playerObject = GameObject.Instantiate(m_playerPrefab);
        m_player = m_playerObject.GetComponent<Player>();

        m_player.Initialize(0, Color.red, m_mapGrid);
        m_player.SetResourceAmount("Gold", 10000);
        m_player.TurnStart();
        m_player.AddUnitToPlayer(m_combatUnitPrefab, new MapIndex(1, 1));
        m_combatUnit = m_player.GetUnits()[0].GetComponent<CombatUnit>();
    }

    [UnityTest, Order(1)]
    public IEnumerator MovementTest()
    {
        MapIndex startTilePos = new MapIndex(1, 1);
        Tile startTile = m_mapGrid.GetTile(startTilePos);
        m_combatUnit.SetCurrentTile(startTile);
        m_player.SelectUnit(startTilePos);
        startTilePos.m_y += 6;
        m_player.SelectUnit(startTilePos);
        m_player.TurnEnd();
        Tile movedTile = m_player.GetUnits()[0].GetComponent<Unit>().GetCurrentTile();
        Assert.True(startTile != movedTile);

        yield return null;
    }

    [UnityTest, Order(2)]
    public IEnumerator MovementCappedAtSpeed()
    {
        MapIndex startTilePos = new MapIndex(1, 1);
        m_combatUnit.SetCurrentTile(m_mapGrid.GetTile(startTilePos));

        Assert.True(m_mapGrid.GetTile(startTilePos).transform.childCount == 1);

        int speed = m_combatUnit.GetSpeed();

        m_combatUnit.SetMovementLeft(0);

        m_player.SelectUnit(startTilePos);
        m_player.SelectUnit(new MapIndex(1, 10));

        int pathlenght = m_combatUnit.GetPath().Count;

        m_player.TurnStart();
        m_player.TurnEnd();

        Assert.True(pathlenght - speed == m_combatUnit.GetPath().Count);

        yield return null;
    }

    [UnityTest, Order(3)]
    public IEnumerator ContinuesMovingUntilPathEnd()
    {
        MapIndex startTilePos = new MapIndex(1, 1);
        m_combatUnit.SetCurrentTile(m_mapGrid.GetTile(startTilePos));

        Assert.True(m_mapGrid.GetTile(startTilePos).transform.childCount == 1);

        int speed = m_combatUnit.GetSpeed();

        m_combatUnit.SetMovementLeft(0);

        m_player.SelectUnit(startTilePos);
        m_player.SelectUnit(new MapIndex(1, 10));

        while (m_combatUnit.GetPath().Count != 0)
        {
            int pathlenght = m_combatUnit.GetPath().Count;
            m_player.TurnStart();
            m_player.TurnEnd();

            if (m_combatUnit.GetPath().Count != 0)
            {
                Assert.True(pathlenght - speed == m_combatUnit.GetPath().Count);
            }
        }

        Assert.True(0 == m_combatUnit.GetPath().Count);

        yield return true;
    }

    [UnityTest, Order(4)]
    public IEnumerator MoveAndAttack()
    {
        Player player2 = GameObject.Instantiate(m_playerPrefab).GetComponent<Player>();
        player2.Initialize(1, Color.blue, m_mapGrid);
        player2.SetResourceAmount("Gold", 10000);
        player2.AddUnitToPlayer(m_combatUnitPrefab, new MapIndex(5, 1));

        Unit player2Scout = player2.GetUnits()[0].GetComponent<Unit>();

        int starthealth = player2Scout.GetCurrentHealth();

        MapIndex startTilePos = new MapIndex(1, 1);
        m_combatUnit.SetCurrentTile(m_mapGrid.GetTile(startTilePos));

        Assert.True(m_mapGrid.GetTile(startTilePos).transform.childCount == 1);

        int speed = m_combatUnit.GetSpeed();

        m_combatUnit.SetMovementLeft(0);

        m_player.SelectUnit(startTilePos);
        m_player.SelectUnit(new MapIndex(5, 1));

        int pathlenght = m_combatUnit.GetPath().Count;

        m_player.TurnStart();
        m_player.TurnEnd();

        Assert.True(pathlenght - speed == m_combatUnit.GetPath().Count);
        Assert.True(starthealth == player2Scout.GetCurrentHealth());

        m_player.TurnStart();
        m_player.TurnEnd();

        Assert.True(0 == m_combatUnit.GetPath().Count);
        Assert.True(starthealth > player2Scout.GetCurrentHealth());

        GameObject.Destroy(player2.gameObject);
        GameObject.Destroy(player2Scout.gameObject);

        yield return true;
    }


    [UnityTest, Order(5)]
    public IEnumerator UnitDeath()
    {
        Player player2 = GameObject.Instantiate(m_playerPrefab).GetComponent<Player>();
        player2.Initialize(1, Color.blue, m_mapGrid);
        player2.SetResourceAmount("Gold", 10000);
        player2.AddUnitToPlayer(m_combatUnitPrefab, new MapIndex(5, 1));

        Unit player2Scout = player2.GetUnits()[0].GetComponent<Unit>();

        int starthealth = player2Scout.GetCurrentHealth();

        player2Scout.TakeDamage(starthealth);

        yield return new WaitForSeconds(2);

        Assert.True(player2Scout == null);
        Assert.True(player2.GetUnits().Count == 0);

        GameObject.Destroy(player2.gameObject);

        yield return true;
    }

    [UnityTest, Order(6)]
    public IEnumerator UnitPreventsFurtherMovement()
    {
        MapIndex startTilePos = new MapIndex(1, 1);
        m_combatUnit.SetCurrentTile(m_mapGrid.GetTile(startTilePos));

        Assert.True(m_mapGrid.GetTile(startTilePos).transform.childCount == 1);

        m_combatUnit.SetMovementLeft(0);

        m_player.SelectUnit(startTilePos);
        m_player.SelectUnit(new MapIndex(1, 10));

        m_player.TurnStart();
        m_player.TurnEnd();

        Tile tile = m_combatUnit.GetCurrentTile();

        Player player2 = GameObject.Instantiate(m_playerPrefab).GetComponent<Player>();
        player2.Initialize(1, Color.blue, m_mapGrid);
        player2.SetResourceAmount("Gold", 10000);
        player2.AddUnitToPlayer(m_combatUnitPrefab, m_combatUnit.GetPath()[m_combatUnit.GetPath().Count - 1].GetTileID());

        m_player.TurnStart();
        m_player.TurnEnd();

        Assert.True(tile == m_combatUnit.GetCurrentTile());

        GameObject.Destroy(player2.gameObject);

        yield return true;
    }

    [UnityTest, Order(7)]
    public IEnumerator CaptureSettler()
    {
        GameObject settlerUnitPrefab = Resources.Load<GameObject>("Prefabs/UnitPrefabs/Settler");

        Player player2 = GameObject.Instantiate(m_playerPrefab).GetComponent<Player>();
        player2.Initialize(1, Color.blue, m_mapGrid);
        player2.SetResourceAmount("Gold", 10000);
        player2.AddUnitToPlayer(settlerUnitPrefab, new MapIndex(2, 1));

        m_combatUnit.SetCurrentTile(m_mapGrid.GetTile(new MapIndex(1, 1)));

        Assert.True(m_mapGrid.GetTile(new MapIndex(1, 1)) == m_combatUnit.GetCurrentTile());

        Unit settlerUnit = player2.GetUnits()[0].GetComponent<Unit>();

        Assert.True(settlerUnit.GetUnitType() == UnitType.Settler);
        Assert.True(settlerUnit.GetOwnerID() == 1);

        m_combatUnit.SetMovementLeft(0);

        m_player.SelectUnit(new MapIndex(1, 1));
        m_player.SelectUnit(new MapIndex(2, 1));

        m_player.TurnStart();
        m_player.TurnEnd();

        Assert.True(settlerUnit.GetOwnerID() == 0);

        yield return true;
    }
}
