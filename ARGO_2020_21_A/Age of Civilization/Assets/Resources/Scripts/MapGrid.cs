using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct MapIndex
{
    public int m_x;     //X component of the map index position.
    public int m_y;     //Y component of the map index position.

    public MapIndex(int t_xIndex, int t_yIndex)
    {
        m_x = t_xIndex;
        m_y = t_yIndex;
    }

    public MapIndex(MapIndex t_mapIndex)
    {
        m_x = t_mapIndex.m_x;
        m_y = t_mapIndex.m_y;
    }
}

public class MapGrid : MonoBehaviour
{
    [SerializeField]
    GameObject m_tilePrefab;

    [SerializeField]
    [Range(1,100)]
    int m_width = 1;

    [SerializeField]
    [Range(1, 100)]
    int m_height = 1;

    [HideInInspector]
    public List<List<Tile>> m_grid = new List<List<Tile>>();

    [SerializeField]
    public float m_tileWidth = 2.61f;
    
    [SerializeField]
    public float m_tileHeight = 3.0f;

    /// <summary>
    /// Sets the size of the width and hight of the 2D tile grid.
    /// </summary>
    /// <param name="t_width">Sets the width of the 2D tile grid</param>
    /// <param name="t_hight">Sets the hight of the 2D tile grid</param>
    public void SetSize(int t_width, int t_height)
    {
        m_width = t_width;
        m_height = t_height;
    }

    /// <summary>
    /// Sets the width of the 2D tile grid.
    /// </summary>
    /// <param name="t_width">Sets the width of the 2D tile grid</param>
    public void SetWidth(int t_width)
    {
        m_width = t_width;
    }

    /// <summary>
    /// Sets the hight of the 2D tile grid.
    /// </summary>
    /// <param name="t_height">Sets the hight of the 2D tile grid</param>
    public void SetHeight(int t_height)
    {
        m_height = t_height;
    }

    /// <summary>
    /// Getter method for the width of the 2D tile grid.
    /// </summary>
    /// <returns>The width of the 2D tile grid</returns>
    public int GetWidth()
    {
        return m_width;
    }

    /// <summary>
    /// Getter method for the hight of the 2D tile grid.
    /// </summary>
    /// <returns>The hight of th 2D tile grid</returns>
    public int GetHeight()
    {
        return m_height;
    }

    /// <summary>
    /// Creates a 2D tile grid adding each time the tile component
    /// to the 2D tile grid.
    /// </summary>
    public void CreateMap()
    {
        m_grid = new List<List<Tile>>();

        for (int x = 0; x < m_width; x++)
        {
            List<Tile> col = new List<Tile>();

            for (int y = 0; y < m_height; y++)
            {
                col.Add(CreateTileObject(new MapIndex(x, y)));
            }
            m_grid.Add(col);
        }

        SetUpNeighbours();

        CameraController camera = FindObjectOfType<CameraController>();

        if(camera != null)
        {
            camera.Setup(m_width, m_height, m_tileWidth, m_tileHeight);
            camera.PlaceCameraAtWorldCenter();
        }
    }

    void SetUpNeighbours()
    {
        List<MapIndex> neighbourOffsets;

        for(int x = 0; x < m_width; x++)
        {
            for(int y = 0; y < m_height; y++)
            {
                neighbourOffsets = new List<MapIndex>();

                neighbourOffsets.Add(new MapIndex(-1, 0));
                neighbourOffsets.Add(new MapIndex(1, 0));

                if(y % 2 != 0)
                {
                    neighbourOffsets.Add(new MapIndex(0, 1));
                    neighbourOffsets.Add(new MapIndex(1, 1));
                    neighbourOffsets.Add(new MapIndex(0, -1));
                    neighbourOffsets.Add(new MapIndex(1, -1));
                }
                else
                {
                    neighbourOffsets.Add(new MapIndex(-1, 1));
                    neighbourOffsets.Add(new MapIndex(0, 1));
                    neighbourOffsets.Add(new MapIndex(-1, -1));
                    neighbourOffsets.Add(new MapIndex(0, -1));
                }

                foreach(MapIndex offSetIndex in neighbourOffsets)
                {
                    MapIndex neighbourIndex = new MapIndex(x + offSetIndex.m_x, y + offSetIndex.m_y);

                    if(!GetIsOutOfBounds(neighbourIndex))
                    {
                        m_grid[x][y].AddNeighbour(m_grid[neighbourIndex.m_x][neighbourIndex.m_y]);
                    }
                }
            }
        }
    }

    public bool GetIsOutOfBounds(MapIndex t_mapIndex)
    {
        if (t_mapIndex.m_x >= 0 && t_mapIndex.m_x < m_width)
        {
            if (t_mapIndex.m_y >= 0 && t_mapIndex.m_y < m_height)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Creates a tile gameobject and sets it's name to the passed in
    /// map index value and its position is set accordingly. The object is then attached as a child to this gameobject.
    /// </summary>
    /// <param name="t_mapIndex">The index postion of the tile within the 2D grid</param>
    /// <returns>The tile component of the tile object</returns>
    Tile CreateTileObject(MapIndex t_mapIndex)
    {
        Vector3 position = new Vector3(m_tileWidth / 2 + t_mapIndex.m_x * m_tileWidth, m_tileHeight / 4 + t_mapIndex.m_y * (m_tileHeight * 0.75f), 0);

        if(t_mapIndex.m_y % 2 != 0)
        {
            position.x += m_tileWidth / 2;
        }

        GameObject tileObject = GameObject.Instantiate(m_tilePrefab, position, Quaternion.identity); ;

        //Set the name of the tile object to the id of the tile.
        tileObject.name = "Tile[" + t_mapIndex.m_x + ", " + t_mapIndex.m_y + "]";
        tileObject.transform.parent = transform;

        Tile newTile = tileObject.GetComponent<Tile>();
        newTile.SetTileID(t_mapIndex);

        return newTile;
    }

    /// <summary>
    /// Converts the positon within the game world into a map index position.
    /// It is assumed that the bottom left corner of the map is at the position (0,0).
    /// It also assumes that the negative position result in negative map index.
    /// </summary>
    /// <param name="t_position">The positon on the x and y axis in the game world</param>
    /// <returns>The map index for the passsed in position</returns>
    public MapIndex PositionToMapIndex(Vector2 t_position)
    {
        MapIndex indexPos = new MapIndex();

        indexPos.m_y = (int)(t_position.y / (m_tileHeight * 0.75f));

        if (t_position.y < 0)
        {
            indexPos.m_y -= 1;
        }

        bool isOdd = (indexPos.m_y % 2 != 0);

        if (isOdd)
        {
            indexPos.m_x = (int)((t_position.x - m_tileWidth / 2) / m_tileWidth);

            if (t_position.x - m_tileWidth / 2 < 0)
            {
                indexPos.m_x -= 1;
            }
        }
        else
        {
            indexPos.m_x = (int)(t_position.x / m_tileWidth);

            if (t_position.x < 0)
            {
                indexPos.m_x -= 1;
            }
        }

        float relY = t_position.y - (indexPos.m_y * (m_tileHeight * 0.75f));

        if(relY > m_tileHeight / 2)
        {
            relY -= m_tileHeight / 2;
            float relX;

            if (isOdd)
            {
                relX = (t_position.x - (indexPos.m_x * m_tileWidth)) - m_tileWidth / 2;
            }
            else
            {
                relX = t_position.x - (indexPos.m_x * m_tileWidth);
            }

            float slope = (m_tileHeight / 4) / (m_tileWidth / 2);

            if(relX < m_tileWidth / 2)
            {
                if (relY > (slope * relX))
                {
                    indexPos.m_y++;

                    if (!isOdd)
                    {
                        indexPos.m_x--;
                    }
                }
            }
            else
            {
                relX -= m_tileWidth / 2;

                if (relY > (-slope * relX) + m_tileHeight / 4) //Right Edge
                {
                    indexPos.m_y++;

                    if (isOdd)
                    {
                        indexPos.m_x++;
                    }
                }
            }

        }
        
        return indexPos;
    }

    /// <summary>
    /// Converts the map index into world position
    /// which is at the centre of the tile at the passed in map index.
    /// </summary>
    /// <param name="t_mapIndex">The index position within the map</param>
    /// <returns>The world position of the map index at the centre of the tile</returns>
    public Vector2 MapIndexToPosition(MapIndex t_mapIndex)
    {
        Vector3 position = new Vector3(m_tileWidth / 2 + t_mapIndex.m_x * m_tileWidth, m_tileHeight / 4 + t_mapIndex.m_y * (m_tileHeight * 0.75f), 1);

        if (t_mapIndex.m_y % 2 != 0)
        {
            position.x += m_tileWidth / 2;
        }

        return position;
    }

    public List<Tile> CreatePath(Tile t_start, Tile t_dest, Unit t_unit)
    {
        //The resulting path from one past the start tile to destination tile.
        List<Tile> path = new List<Tile>();

        //We use a list as a priority queue that we reorder after the element is added.
        List<Tile> pq = new List<Tile>();

        for (int col = 0; col < m_width; col++)
        {
            for (int row = 0; row < m_height; row++)
            {
                m_grid[col][row].SetIsMarked(false);
                m_grid[col][row].SetPrevious(null);
                m_grid[col][row].SetPathCost(float.MaxValue);
                m_grid[col][row].SetHeuristicCost((t_dest.GetPostion() - m_grid[col][row].GetPostion()).magnitude);
            }
        }

        t_start.SetPathCost(0);
        pq.Add(t_start);
        t_start.SetIsMarked(true);

        while(pq.Count != 0 && pq[0] != t_dest)
        {
            foreach(Tile tile in pq[0].GetNeighbours())
            {
                if(tile != pq[0].GetPrevious())
                {
                    if(tile.GetIsTraversable(t_unit) || t_dest == tile)
                    {
                        float childCost = Tile.m_WEIGHT_COST + pq[0].GetPathCost();

                        if (childCost < tile.GetPathCost())
                        {
                            tile.SetPathCost(childCost);
                            tile.SetPrevious(pq[0]);
                        }

                        if (tile.GetIsMarked() == false)
                        {
                            pq.Add(tile);
                            tile.SetIsMarked(true);
                        }
                    }
                }
            }

            pq.Remove(pq[0]);
            pq = pq.OrderBy(o => Tile.CalculateTotalCost(o.GetHeuristicCost(), o.GetPathCost())).ToList();
        }

        if(t_dest.GetPrevious() != null)
        {
            Tile pathTile = t_dest;

            while (pathTile != t_start && pathTile != null)
            {
                path.Add(pathTile);
                pathTile = pathTile.GetPrevious();
            }
        }

        return path;
    }

    // same as above just accepts tile
    public bool GetIsTileEmpty(Tile t_tile)
    {
        if (t_tile.transform.childCount != 0)
        {
            for (int i = 0; i < t_tile.transform.childCount; i++)
            {
                if (t_tile.transform.GetChild(i).tag == "CombatUnit")
                {
                    return false; // child is a combat unit
                }
            }  
        }

        return true;
    }

    public Tile GetTile(MapIndex t_mapIndex)
    {
        return m_grid[t_mapIndex.m_x][t_mapIndex.m_y];
    }
}
