using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Dictionary<string, int> m_resources;

    void Awake()
    {
        m_resources = new Dictionary<string, int>();
        m_resources.Add("Iron", 0);
        m_resources.Add("Horses", 0);
        m_resources.Add("Wood", 80);
        m_resources.Add("Gold", 100);
        m_resources.Add("Diamonds", 0);
        m_resources.Add("Grapes", 0);
        m_resources.Add("Oranges", 0);
    }
}
