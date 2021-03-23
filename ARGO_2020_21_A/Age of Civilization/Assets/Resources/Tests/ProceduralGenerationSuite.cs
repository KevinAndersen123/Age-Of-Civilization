using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;
public class ProceduralGenerationSuite
{
    GameObject m_mapGridPrefab;
    GameObject m_mapGridObject;
    MapGrid m_mapGrid;

    [OneTimeSetUp]
    public void Setup()
    {
        m_mapGridPrefab = Resources.Load<GameObject>("Prefabs/MapGrid");
        m_mapGridObject = GameObject.Instantiate(m_mapGridPrefab, Vector2.zero, Quaternion.identity);

        m_mapGrid = m_mapGridObject.GetComponent<MapGrid>();
    }

    [UnityTest, Order(1)]
    public IEnumerator CustomSeed()
    {
        ProceduralGeneration.SetSeed("Test");
        ProceduralGeneration.GenerateMap(m_mapGrid);

        GameObject mapGridObject2 = GameObject.Instantiate(m_mapGridPrefab, Vector2.zero, Quaternion.identity);

        MapGrid mapGrid2 = mapGridObject2.GetComponent<MapGrid>();
        ProceduralGeneration.GenerateMap(mapGrid2);

        for (int x = 0; x < m_mapGrid.GetWidth(); x++)
        {
            for (int y = 0; y < m_mapGrid.GetHeight(); y++)
            {
                Assert.True(m_mapGrid.m_grid[x][y].GetTileType() == mapGrid2.m_grid[x][y].GetTileType());
            }
        }

        GameObject.Destroy(mapGridObject2);

        yield return null;
    }

    [UnityTest, Order(2)]
    public IEnumerator RandomSeed()
    {
        ProceduralGeneration.GenerateRandomSeed();
        ProceduralGeneration.GenerateMap(m_mapGrid);

        string seed1 = ProceduralGeneration.GetSeed();

        GameObject mapGridObject2 = GameObject.Instantiate(m_mapGridPrefab, Vector2.zero, Quaternion.identity);

        MapGrid mapGrid2 = mapGridObject2.GetComponent<MapGrid>();
        ProceduralGeneration.GenerateRandomSeed();
        ProceduralGeneration.GenerateMap(mapGrid2);

        string seed2 = ProceduralGeneration.GetSeed();

        Assert.True(seed1 != seed2);

        bool isIdentical = true;

        for (int x = 0; x < m_mapGrid.GetWidth() && isIdentical; x++)
        {
            for (int y = 0; y < m_mapGrid.GetHeight() && isIdentical; y++)
            {
                if (m_mapGrid.m_grid[x][y].GetTileType() != mapGrid2.m_grid[x][y].GetTileType())
                {
                    isIdentical = false;
                }
            }
        }

        Assert.True(isIdentical == false);

        GameObject.Destroy(mapGridObject2);

        yield return null;
    }

    [UnityTest, Order(3)]
    public IEnumerator SizeVariation()
    {
        ProceduralGeneration.SetGenerationSize(GenerationSize.Small);
        ProceduralGeneration.GenerateMap(m_mapGrid);

        Assert.True(m_mapGrid.GetWidth() == ProceduralGeneration.s_smallSize.x);
        Assert.True(m_mapGrid.GetHeight() == ProceduralGeneration.s_smallSize.y);

        ProceduralGeneration.SetGenerationSize(GenerationSize.Medium);
        ProceduralGeneration.GenerateMap(m_mapGrid);

        Assert.True(m_mapGrid.GetWidth() == ProceduralGeneration.s_mediumSize.x);
        Assert.True(m_mapGrid.GetHeight() == ProceduralGeneration.s_mediumSize.y);

        ProceduralGeneration.SetGenerationSize(GenerationSize.Large);
        ProceduralGeneration.GenerateMap(m_mapGrid);

        Assert.True(m_mapGrid.GetWidth() == ProceduralGeneration.s_largeSize.x);
        Assert.True(m_mapGrid.GetHeight() == ProceduralGeneration.s_largeSize.y);

        yield return null;
    }

    [UnityTest, Order(4)]
    public IEnumerator ResourcePlacment()
    {
        ProceduralGeneration.SetGenerationSize(GenerationSize.Small);
        ProceduralGeneration.GenerateMap(m_mapGrid);

        List<ResourceTile> resourceTiles = new List<ResourceTile>();

        for (int x = 0; x < m_mapGrid.GetWidth(); x++)
        {
            for (int y = 0; y < m_mapGrid.GetHeight(); y++)
            {
                if (m_mapGrid.m_grid[x][y].GetResourceTile() != null)
                {
                    resourceTiles.Add(m_mapGrid.m_grid[x][y].GetResourceTile());
                }
            }
        }

        int count;

        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            count = 0;

            foreach (ResourceTile resourceTile in resourceTiles)
            {
                if(resourceTile.GetResourceType() == type)
                {
                    count++;
                }
            }

            Assert.True(count == ProceduralGeneration.s_resourceCountSmall);
        }

        yield return null;
    }
}
