using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Wood = 0,
    Iron = 1,
    Horses = 2,
    Wheat = 3
}

public class ResourceTile : MonoBehaviour
{
    [HideInInspector]
    public Tile m_parentTile;

    ResourceType m_resourceType;

    public int m_resourceValue;

    public void Setup(Tile t_parentTile, ResourceType t_resourceType)
    {
        m_parentTile = t_parentTile;
        m_parentTile.SetIsOccupied(true);
        transform.position = m_parentTile.transform.position;
        transform.SetParent(m_parentTile.transform);

        m_resourceType = t_resourceType;

        switch(m_resourceType)
        {
            case ResourceType.Wood:
                m_resourceValue = 2;
                m_parentTile.SetProduction(3);
                m_parentTile.SetFood(1);
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Resource_Wood");
                break;
            case ResourceType.Iron:
                m_resourceValue = 3;
                m_parentTile.SetProduction(3);
                m_parentTile.SetFood(0);
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Resource_Iron");
                break;
            case ResourceType.Horses:
                m_resourceValue = 3;
                m_parentTile.SetProduction(3);
                m_parentTile.SetFood(1);
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Resource_Horse");
                break;
            case ResourceType.Wheat:
                m_resourceValue = 1;
                m_parentTile.SetProduction(0);
                m_parentTile.SetFood(3);
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Resource_Food");
                break;
            default:
                break;
        }
    }

    public ResourceType GetResourceType()
    {
        return m_resourceType;
    }

    public int GetResourceValue()
    {
        return m_resourceValue;
    }
}
