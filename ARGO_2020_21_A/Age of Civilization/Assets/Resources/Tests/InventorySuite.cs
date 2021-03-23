using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InventorySuite
{
    GameObject inventoryObj;
    string path = "Prefabs/InventoryTestObj";

    [OneTimeSetUp]
    public void Setup()
    {
        if (inventoryObj == null)
        {
            GameObject inventoryPrefab = Resources.Load<GameObject>(path);
            inventoryObj = GameObject.Instantiate(inventoryPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity);
        }
    }

    [UnityTest]
    public IEnumerator StoringResourcesTest()
    {
        Assert.IsTrue(inventoryObj.GetComponent<PlayerInventory>().m_resources.ContainsKey("Gold"));
        yield return null;
    }

    [UnityTest]
    public IEnumerator AccessingResourcesTest()
    {
        if (inventoryObj.GetComponent<PlayerInventory>().m_resources.ContainsKey("Gold"))
        {
            int goldCount = inventoryObj.GetComponent<PlayerInventory>().m_resources["Gold"];
            inventoryObj.GetComponent<PlayerInventory>().m_resources["Gold"]++;
            Assert.AreNotEqual(goldCount, inventoryObj.GetComponent<PlayerInventory>().m_resources["Gold"]);
        }

        if (inventoryObj.GetComponent<PlayerInventory>().m_resources.ContainsKey("Iron"))
        {
            int ironCount = inventoryObj.GetComponent<PlayerInventory>().m_resources["Iron"];
            inventoryObj.GetComponent<PlayerInventory>().m_resources["Iron"]++;
            Assert.AreNotEqual(ironCount, inventoryObj.GetComponent<PlayerInventory>().m_resources["Iron"]);
        }

        yield return null;
    }
}
