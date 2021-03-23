using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class GameControllerSuite
{
    GameObject controllerObj;
    string path = "Prefabs/GameController";

    [OneTimeSetUp]
    public void Setup()
    {
        if (controllerObj == null)
        {
            GameObject controllerPrefab = Resources.Load<GameObject>(path);
            controllerObj = GameObject.Instantiate(controllerPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity);
        }
    }

    [UnityTest, Order(0)]
    public IEnumerator PersistingObject()
    {
        Debug.ClearDeveloperConsole();
        GameObject controllerPrefab = Resources.Load<GameObject>(path);
        GameObject controllerObject2 = GameObject.Instantiate(controllerPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Assert.True(controllerObject2 == null);
    }

    [UnityTest, Order(1)]
    public IEnumerator UpdatingTimer()
    {
        Debug.ClearDeveloperConsole();
        GameController.LoadScene("Gameplay");
        float timer = GameController.s_timeElapsed;
        GameController.SetUpdateTimer(true);
        yield return new WaitForSeconds(3);
        Assert.AreNotEqual(timer, GameController.s_timeElapsed);
    }
}
