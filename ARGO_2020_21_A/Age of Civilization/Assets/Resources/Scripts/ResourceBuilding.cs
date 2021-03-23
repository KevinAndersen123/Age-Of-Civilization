using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBuilding : MonoBehaviour
{
    [SerializeField]
    List<Sprite> m_buildingSprites;

    ResourceTile m_resourceTile;

    City m_cityOwner;
    public void Initialize(Tile t_originTile, City t_city)
    {
        m_cityOwner = t_city;
        transform.position = t_originTile.transform.position;
        transform.SetParent(t_originTile.transform);

        m_resourceTile = t_originTile.GetResourceTile();
        GetComponent<SpriteRenderer>().sprite = m_buildingSprites[(int)m_resourceTile.GetResourceType()];
    }

    public void ProduceResource()
    {
        m_cityOwner.RecieveResource(m_resourceTile.GetResourceType(), m_resourceTile.m_resourceValue);
    }
}
