using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class CitySuite
{
    GameObject m_mapGridPrefab;
    GameObject m_playerPrefab;
    GameObject m_cityPrefab;

    GameObject m_cityObject;
    GameObject m_mapGridObject;

    City m_city;

    MapGrid m_mapGrid;

    Player m_player;

    [OneTimeSetUp]
    public void Setup()
    {
        m_mapGridPrefab = Resources.Load<GameObject>("Prefabs/MapGrid");

        m_mapGridObject = GameObject.Instantiate(m_mapGridPrefab, Vector2.zero, Quaternion.identity);

        m_mapGrid = m_mapGridObject.GetComponent<MapGrid>();

        m_playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        GameObject playerObject = GameObject.Instantiate(m_playerPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity);

        m_player = playerObject.GetComponent<Player>();

        m_cityPrefab = Resources.Load<GameObject>("Prefabs/City");
        m_cityObject = GameObject.Instantiate(m_cityPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity);
        m_city = m_cityObject.GetComponent<City>();

        m_player.Initialize(0, GameController.s_playerColours[0], m_mapGrid);

        GameplayManager.InitCityNames();
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

        m_city.Setup(m_mapGrid.m_grid[10][10], m_player);
        m_player.AddCity(m_city.gameObject);
    }

    [UnityTest, Order(1)]
    public IEnumerator CheckPopulation()
    {
        m_city.m_population = 1;
        Assert.True(m_city.m_population == 1);
        yield return null;
    }

    [UnityTest, Order(2)]
    public IEnumerator CheckBorderTiles()
    {
        foreach (BorderTile borderTile in m_city.GetBorderTiles())
        {
            Assert.True(m_city.GetOwnerID() == borderTile.m_tile.GetOwnerID());
        }

        yield return null;
    }

    [UnityTest, Order(3)]
    public IEnumerator CheckFood()
    {
        int food = 0;

        foreach (BorderTile borderTile in m_city.GetBorderTiles())
        {
            food += borderTile.m_tile.GetFood();
        }
        Assert.True(m_city.m_food == food);

        yield return null;
    }

    [UnityTest, Order(4)]
    public IEnumerator CheckProduction()
    {
        int prod = 0;

        foreach (BorderTile borderTile in m_city.GetBorderTiles())
        {
            prod += borderTile.m_tile.GetProduction();
        }

        Assert.True(m_city.m_production == prod);

        yield return null;
    }

    [UnityTest, Order(5)]
    public IEnumerator CheckBorderGrowth()
    {
        int count = m_city.GetBorderTiles().Count;

        for (int i = 0; i < 100; i++)
        {
            m_city.TurnStart();
        }

        Assert.True(count < m_city.GetBorderTiles().Count);

        yield return null;
    }

    [UnityTest, Order(6)]
    public IEnumerator CaptureCity()
    {
        GameObject playerObject2 = GameObject.Instantiate(m_playerPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity);

        Player player2 = playerObject2.GetComponent<Player>();

        GameObject cityObject2 = GameObject.Instantiate(m_cityPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity);
        City city2 = cityObject2.GetComponent<City>();

        player2.Initialize(1, GameController.s_playerColours[1], m_mapGrid);

        city2.Setup(m_mapGrid.m_grid[5][5], player2);
        player2.AddCity(city2.gameObject);

        GameObject unitPrefab = Resources.Load<GameObject>("Prefabs/UnitPrefabs/Scout");

        m_player.AddUnitToPlayer(unitPrefab, new MapIndex(4, 4));

        Assert.True(city2.GetOwnerID() == player2.GetID());

        city2.TakeDamage(city2.GetCurrentHealth());

        m_player.SelectUnit(new MapIndex(4, 4));
        m_player.SelectUnit(new MapIndex(5, 5));

        Assert.True(city2.GetOwnerID() != player2.GetID());
        Assert.True(city2.GetOwnerID() == m_player.GetID());

        Assert.True(player2.GetCities().Count == 0);
        Assert.True(m_player.GetCities().Count == 2);

        yield return null;
    }
}