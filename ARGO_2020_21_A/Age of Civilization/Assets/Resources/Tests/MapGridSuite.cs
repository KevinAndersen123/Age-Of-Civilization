using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MapGridSuite
{
    MapGrid m_mapGrid;
    GameObject m_mapGridPrefab;
    GameObject m_mapGridObject;

    CombatUnit m_combatUnit;

    [OneTimeSetUp]
    public void Setup()
    {
        GameObject combatUnitPrefab = Resources.Load<GameObject>("Prefabs/UnitPrefabs/Scout");
        m_mapGridPrefab = Resources.Load<GameObject>("Prefabs/MapGrid");

        m_mapGridObject = GameObject.Instantiate(m_mapGridPrefab, Vector2.zero, Quaternion.identity);
        m_combatUnit = GameObject.Instantiate(combatUnitPrefab, Vector2.zero, Quaternion.identity).GetComponent<CombatUnit>();

        m_mapGrid = m_mapGridObject.GetComponent<MapGrid>();
    }

    [UnityTest, Order(1)]
    public IEnumerator SetWidth()
    {
        m_mapGrid.SetWidth(5);

        Assert.True(m_mapGrid.GetWidth() == 5);

        yield return null;
    }

    [UnityTest, Order(2)]
    public IEnumerator SetHeight()
    {
        m_mapGrid.SetHeight(7);

        Assert.True(m_mapGrid.GetHeight() == 7);

        yield return null;
    }

    [UnityTest, Order(3)]
    public IEnumerator GenerateMap()
    {
        Assert.True(m_mapGrid.GetWidth() == 5);
        Assert.True(m_mapGrid.GetHeight() == 7);

        m_mapGrid.CreateMap();

        Assert.True(m_mapGrid.m_grid.Count == 5);

        foreach(List<Tile> col in m_mapGrid.m_grid)
        {
            Assert.True(col.Count == 7);
        }

        yield return null;
    }

    [UnityTest, Order(4)]
    public IEnumerator TilePositions()
    {
        Assert.True(m_mapGrid.GetWidth() == 5);
        Assert.True(m_mapGrid.GetHeight() == 7);

        for(int x = 0; x < 5; x++)
        {
            for(int y = 0; y < 7; y++)
            {
                MapIndex actualIndex = m_mapGrid.PositionToMapIndex(m_mapGrid.m_grid[x][y].GetPostion());
                MapIndex expectedIndex = new MapIndex(x, y);

                Assert.True(actualIndex.m_x == expectedIndex.m_x && actualIndex.m_y == expectedIndex.m_y);
            }
        }

        yield return null;
    }

    [UnityTest, Order(5)]
    public IEnumerator TileNeighboours()
    {
        Assert.True(m_mapGrid.GetWidth() == 5);
        Assert.True(m_mapGrid.GetHeight() == 7);

        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                Assert.True(2 <= m_mapGrid.m_grid[x][y].GetNeighbours().Count && 6 >= m_mapGrid.m_grid[x][y].GetNeighbours().Count);
            }
        }

        yield return null;
    }

    [UnityTest, Order(6)]
    public IEnumerator PathfindingShortestPath()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                m_mapGrid.m_grid[x][y].SetTileType(TileType.Land);
            }
        }

        m_combatUnit.SetCurrentTile(m_mapGrid.m_grid[0][0]);
        List<Tile> path = m_mapGrid.CreatePath(m_mapGrid.m_grid[0][0], m_mapGrid.m_grid[3][3], m_combatUnit);

        List<Tile> expectedPath = new List<Tile>();
        expectedPath.Add(m_mapGrid.m_grid[3][3]);
        expectedPath.Add(m_mapGrid.m_grid[3][2]);
        expectedPath.Add(m_mapGrid.m_grid[2][2]);
        expectedPath.Add(m_mapGrid.m_grid[1][1]);
        expectedPath.Add(m_mapGrid.m_grid[0][1]);

        for(int i = 0; i < expectedPath.Count; i++)
        {
            Assert.True(path[i] == expectedPath[i]);
        }

        yield return null;
    }

    [UnityTest, Order(7)]
    public IEnumerator AvoidBlockTiles()
    {
        m_mapGrid.m_grid[1][1].SetTileType(TileType.Mountain);
        m_mapGrid.m_grid[1][2].SetTileType(TileType.Mountain);
        m_mapGrid.m_grid[1][3].SetTileType(TileType.Mountain);
        m_mapGrid.m_grid[2][2].SetTileType(TileType.Mountain);
        m_mapGrid.m_grid[2][1].SetTileType(TileType.Mountain);
        m_mapGrid.m_grid[2][3].SetTileType(TileType.Mountain);
        m_mapGrid.m_grid[3][4].SetTileType(TileType.Mountain);
        m_mapGrid.m_grid[4][4].SetTileType(TileType.Mountain);
        m_mapGrid.m_grid[3][2].SetTileType(TileType.Mountain);

        List<Tile> path = m_mapGrid.CreatePath(m_mapGrid.m_grid[0][0], m_mapGrid.m_grid[3][3], m_combatUnit);

        List<Tile> expectedPath = new List<Tile>();
        expectedPath.Add(m_mapGrid.m_grid[3][3]);
        expectedPath.Add(m_mapGrid.m_grid[4][2]);
        expectedPath.Add(m_mapGrid.m_grid[3][1]);
        expectedPath.Add(m_mapGrid.m_grid[3][0]);
        expectedPath.Add(m_mapGrid.m_grid[2][0]);
        expectedPath.Add(m_mapGrid.m_grid[1][0]);

        for (int i = 0; i < expectedPath.Count; i++)
        {
            Assert.True(path[i] == expectedPath[i]);
        }

        yield return null;
    }
}
