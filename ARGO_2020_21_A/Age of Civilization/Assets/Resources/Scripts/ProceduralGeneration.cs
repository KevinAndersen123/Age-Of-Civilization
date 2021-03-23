using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GenerationSize
{ 
    Small,
    Medium,
    Large
}

public class ProceduralGeneration
{
    static int s_landChance = 60;
    static int s_mountainChance = 37;
    static int s_desertChance = 36;
    static int s_borderSize = 2;
    static int s_easeIterations = 12;

    public const int s_MAX_SEED_LENGTH = 6;
    
    public static string s_seed = "Jimbo";

    public const string s_VALID_SEED_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
    
    static System.Random s_seedRandom;

    public static GenerationSize s_generationSize = GenerationSize.Small;

    public static Vector2Int s_smallSize = new Vector2Int(50, 40); 
    public static Vector2Int s_mediumSize = new Vector2Int(70, 60); 
    public static Vector2Int s_largeSize = new Vector2Int(90, 80);

    public static int s_resourceCountSmall = 9;
    public static int s_resourceCountMedium = 15;
    public static int s_resourceCountLarge = 25;

    public static void GenerateMap(MapGrid t_mapGrid)
    {
        Debug.Log(s_seed);
        s_seedRandom = new System.Random(s_seed.GetHashCode());

        switch (s_generationSize)
        {
            case GenerationSize.Small:
                t_mapGrid.SetSize(s_smallSize.x, s_smallSize.y);
                break;
            case GenerationSize.Medium:
                t_mapGrid.SetSize(s_mediumSize.x, s_mediumSize.y);
                break;
            case GenerationSize.Large:
                t_mapGrid.SetSize(s_largeSize.x, s_largeSize.y);
                break;
            default:
                break;
        }

        t_mapGrid.CreateMap();

        CreateLand(t_mapGrid);
        CreateMountains(t_mapGrid);
        CreateDesert(t_mapGrid);
        PlaceResources(t_mapGrid);
    }

    static void CreateLand(MapGrid t_mapGrid)
    {
        for (int x = s_borderSize; x < t_mapGrid.GetWidth() - s_borderSize; x++)
        {
            for (int y = s_borderSize; y < t_mapGrid.GetHeight() - s_borderSize; y++)
            {
                int value = s_seedRandom.Next(0, 100);

                if (value < s_landChance)
                {
                    t_mapGrid.m_grid[x][y].SetTileType(TileType.Land);
                    t_mapGrid.m_grid[x][y].SetSprite("Land");
                }
            }
        }

        for (int i = 0; i < s_easeIterations; i++)
        {
            SmoothLand(t_mapGrid);
        }
    }

    static void CreateMountains(MapGrid t_mapGrid)
    {
        for (int x = s_borderSize; x < t_mapGrid.GetWidth() - s_borderSize; x++)
        {
            for (int y = s_borderSize; y < t_mapGrid.GetHeight() - s_borderSize; y++)
            {
                if (t_mapGrid.m_grid[x][y].GetTileType() == TileType.Land)
                {
                    int value = s_seedRandom.Next(0, 100);

                    if (value < s_mountainChance)
                    {
                        t_mapGrid.m_grid[x][y].SetTileType(TileType.Mountain);
                        t_mapGrid.m_grid[x][y].SetSprite("Mountain");
                    }
                }
            }
        }

        for (int i = 0; i < s_easeIterations; i++)
        {
            SmoothMountains(t_mapGrid);
        }
    }

    static void CreateDesert(MapGrid t_mapGrid)
    {
        for (int x = s_borderSize; x < t_mapGrid.GetWidth() - s_borderSize; x++)
        {
            for (int y = s_borderSize; y < t_mapGrid.GetHeight() - s_borderSize; y++)
            {
                if (t_mapGrid.m_grid[x][y].GetTileType() == TileType.Land)
                {
                    int value = s_seedRandom.Next(0, 100);

                    if (value < s_desertChance)
                    {
                        t_mapGrid.m_grid[x][y].SetTileType(TileType.Sand);
                        t_mapGrid.m_grid[x][y].SetSprite("Sand");
                    }
                }
            }
        }

        for (int i = 0; i < s_easeIterations; i++)
        {
            SmoothDesert(t_mapGrid);
        }
    }

        static void SmoothLand(MapGrid t_mapGrid)
    {
        for (int x = 0; x < t_mapGrid.GetWidth(); x++)
        {
            for (int y = 0; y < t_mapGrid.GetHeight(); y++)
            {
                int neighbourLandTiles = GetSurroundingTypeCount(t_mapGrid.m_grid[x][y], TileType.Land);

                if (neighbourLandTiles > 3)
                {
                    t_mapGrid.m_grid[x][y].SetTileType(TileType.Land);
                    t_mapGrid.m_grid[x][y].SetSprite("Land");
                }
                else if(neighbourLandTiles < 3)
                {
                    t_mapGrid.m_grid[x][y].SetTileType(TileType.Water);
                    t_mapGrid.m_grid[x][y].SetSprite("Water");
                }
            }
        }
    }

    static void SmoothMountains(MapGrid t_mapGrid)
    {
        for (int x = s_borderSize; x < t_mapGrid.GetWidth() - s_borderSize; x++)
        {
            for (int y = s_borderSize; y < t_mapGrid.GetHeight() - s_borderSize; y++)
            {
                if (t_mapGrid.m_grid[x][y].GetTileType() != TileType.Water)
                {
                    int neighbourLandTiles = GetSurroundingTypeCount(t_mapGrid.m_grid[x][y], TileType.Mountain);

                    if (neighbourLandTiles > 4)
                    {
                        t_mapGrid.m_grid[x][y].SetTileType(TileType.Mountain);
                        t_mapGrid.m_grid[x][y].SetSprite("Mountain");
                    }
                    else if (neighbourLandTiles < 2)
                    {
                        t_mapGrid.m_grid[x][y].SetTileType(TileType.Land);
                        t_mapGrid.m_grid[x][y].SetSprite("Land");
                    }
                }
            }
        }
    }

    static void SmoothDesert(MapGrid t_mapGrid)
    {
        for (int x = s_borderSize; x < t_mapGrid.GetWidth() - s_borderSize; x++)
        {
            for (int y = s_borderSize; y < t_mapGrid.GetHeight() - s_borderSize; y++)
            {
                if (t_mapGrid.m_grid[x][y].GetTileType() != TileType.Water && t_mapGrid.m_grid[x][y].GetTileType() != TileType.Mountain)
                {
                    int neighbourLandTiles = GetSurroundingTypeCount(t_mapGrid.m_grid[x][y], TileType.Sand);

                    if (neighbourLandTiles > 3)
                    {
                        t_mapGrid.m_grid[x][y].SetTileType(TileType.Sand);
                        t_mapGrid.m_grid[x][y].SetSprite("Sand");
                    }
                    else if (neighbourLandTiles < 2)
                    {
                        t_mapGrid.m_grid[x][y].SetTileType(TileType.Land);
                        t_mapGrid.m_grid[x][y].SetSprite("Land");
                    }
                }
            }
        }
    }

    static int GetSurroundingTypeCount(Tile t_tile, TileType t_type)
    {
        int count = 0;

        foreach(Tile tile in t_tile.GetNeighbours())
        {
            if(tile.GetTileType() == t_type)
            {
                count++;
            }
        }

        return count;
    }

    public static void SetSeed(string t_seed)
    {
        s_seed = t_seed;
    }

    public static string GetSeed()
    {
        return s_seed;
    }

    public static void SetGenerationSize(GenerationSize t_generationSize)
    {
        s_generationSize = t_generationSize;
    }

    public static void GenerateRandomSeed()
    {
        s_seed = "";

        int count = 0;

        while (count < s_MAX_SEED_LENGTH)
        {
            char c = s_VALID_SEED_CHARS[UnityEngine.Random.Range(0, s_VALID_SEED_CHARS.Length)];
            s_seed += c;

            count++;
        }
    }

    static void PlaceResources(MapGrid t_mapGrid)
    {
        GameObject resourcePrefab = Resources.Load<GameObject>("Prefabs/ResourceTile");

        int maxResourceCount;

        switch (s_generationSize)
        {
            case GenerationSize.Small:
                maxResourceCount = s_resourceCountSmall;
                break;
            case GenerationSize.Medium:
                maxResourceCount = s_resourceCountMedium;
                break;
            case GenerationSize.Large:
                maxResourceCount = s_resourceCountLarge;
                break;
            default:
                maxResourceCount = 0;
                break;
        }

        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            int count = 0;

            while (count != maxResourceCount)
            {
                int x = s_seedRandom.Next(s_borderSize, t_mapGrid.GetWidth() - s_borderSize);
                int y = s_seedRandom.Next(s_borderSize, t_mapGrid.GetHeight() - s_borderSize);

                if(t_mapGrid.m_grid[x][y].GetTileType() == TileType.Land)
                {
                    if (t_mapGrid.m_grid[x][y].GetResourceTile() == null)
                    {
                        bool tooClose = false;

                        foreach(Tile neighbourTile in t_mapGrid.m_grid[x][y].GetNeighbours())
                        {
                            if(neighbourTile.GetResourceTile() != null)
                            {
                                tooClose = true;
                            }
                        }

                        if(!tooClose)
                        {
                            GameObject resource = UnityEngine.Object.Instantiate(resourcePrefab);

                            resource.GetComponent<ResourceTile>().Setup(t_mapGrid.m_grid[x][y], type);

                            t_mapGrid.m_grid[x][y].SetResourceTile(resource.GetComponent<ResourceTile>());

                            count++;
                        }
                    }
                }
            }
        }
    }
}
