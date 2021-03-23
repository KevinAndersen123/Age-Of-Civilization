using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TileSuite
{
    Tile m_tile;
    GameObject m_tilePrefab;
    GameObject m_tileObject;

    [OneTimeSetUp]
    public void Setup()
    {
        m_tilePrefab = Resources.Load<GameObject>("Prefabs/Tile");

        m_tileObject = GameObject.Instantiate(m_tilePrefab, Vector2.zero, Quaternion.identity);

        m_tile = m_tileObject.GetComponent<Tile>();
    }

    [UnityTest, Order(1)]
    public IEnumerator ChangePositon()
    {
        Assert.True(m_tile.GetPostion() == Vector2.zero);

        m_tile.SetPosition(new Vector2(1, 0));

        Assert.True(m_tile.GetPostion() == new Vector2(1, 0));
        Assert.True(m_tileObject.transform.position == new Vector3(1, 0, 0));

        yield return null;
    }

    [UnityTest, Order(2)]
    public IEnumerator ChangeTileType()
    {
        Assert.True(m_tile.GetTileType() == TileType.Water);

        m_tile.SetTileType(TileType.Land);

        Assert.True(m_tile.GetTileType() == TileType.Land);

        yield return null;
    }

    [UnityTest, Order(3)]
    public IEnumerator ChangeSprite()
    {
        Sprite landSprite = Resources.Load<Sprite>("Sprites/land");
        Sprite waterSprite = Resources.Load<Sprite>("Sprites/water");

        Assert.True(m_tileObject.GetComponent<SpriteRenderer>().sprite == waterSprite);

        m_tile.SetSprite("land");

        Assert.True(m_tileObject.GetComponent<SpriteRenderer>().sprite == landSprite);

        yield return null;
    }

    [UnityTest, Order(4)]
    public IEnumerator AddNeighbour()
    {
        GameObject tileObject2 = GameObject.Instantiate(m_tilePrefab, Vector2.zero, Quaternion.identity);

        Tile tile2 = tileObject2.GetComponent<Tile>();

        Assert.True(tile2.GetPostion() == Vector2.zero);

        Assert.False(tile2.IsInNeighbours(m_tile));
        Assert.False(m_tile.IsInNeighbours(tile2));

        tile2.AddNeighbour(m_tile);

        Assert.True(tile2.IsInNeighbours(m_tile));
        Assert.False(m_tile.IsInNeighbours(tile2));

        yield return null;
    }

    [UnityTest, Order(5)]
    public IEnumerator CheckProductionAndFood()
    {
        Assert.True(m_tile.GetProduction() == 2);
        Assert.True(m_tile.GetFood() == 2);

        m_tile.SetTileType(TileType.Water);

        Assert.True(m_tile.GetProduction() == 0);
        Assert.True(m_tile.GetFood() == 0);

        yield return null;
    }

    [UnityTest, Order(6)]
    public IEnumerator CheckResourceTileSetup()
    {
        m_tile.SetTileType(TileType.Land);

        GameObject resourcePrefab = Resources.Load<GameObject>("Prefabs/ResourceTile");

        GameObject resourceObject = GameObject.Instantiate(resourcePrefab);

        ResourceTile resource = resourceObject.GetComponent<ResourceTile>();

        resource.Setup(m_tile, ResourceType.Wheat);

        Assert.True(m_tile.transform.position == resource.transform.position);
        Assert.True(resource.transform.parent == m_tile.transform);
        Assert.True(resource.GetResourceType() == ResourceType.Wheat);
        Assert.True(m_tile.GetProduction() == 0);
        Assert.True(m_tile.GetFood() == 3);

        yield return null;
    }
}
