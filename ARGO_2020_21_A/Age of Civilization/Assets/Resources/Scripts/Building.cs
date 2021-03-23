using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField]
    List<Sprite> m_buildingSprites;
    City m_cityOwner;
    public void Initialize(Tile t_originTile, City t_city, int t_type)
    {
        m_cityOwner = t_city;
        transform.position = t_originTile.transform.position;
        transform.SetParent(t_originTile.transform);
        GetComponent<SpriteRenderer>().sprite = m_buildingSprites[t_type - 4];
    }
}
