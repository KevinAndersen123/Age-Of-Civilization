using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameplayManagerSuite
{
    GameplayManager m_gpManager;
    GameObject m_gpManagerPrefab;
    GameObject m_gpManagerObject;

    GameObject m_controllerObj;

    [OneTimeSetUp]
    public void Setup()
    {
        if (m_controllerObj == null)
        {
            GameObject controllerPrefab = Resources.Load<GameObject>("Prefabs/GameController");
            m_controllerObj = GameObject.Instantiate(controllerPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity);
        }

        GameObject mapPrefab = Resources.Load<GameObject>("Prefabs/MapGrid");

        GameObject.Instantiate(mapPrefab);

        m_gpManagerPrefab = Resources.Load<GameObject>("Prefabs/GameplayManager");

        m_gpManagerObject = GameObject.Instantiate(m_gpManagerPrefab, Vector2.zero, Quaternion.identity);

        m_gpManager = m_gpManagerObject.GetComponent<GameplayManager>();
    }

    [UnityTest, Order(1)]
    public IEnumerator NumberOfPlayersSet()
    {
        Assert.True(m_gpManager.m_numberOfPlayers == GameController.GetNumberOfPlayers());
        yield return true;
    }

    [UnityTest, Order(2)]
    public IEnumerator EndTurnIncrementsTurnID()
    {
        int numOfPlayers = GameController.GetNumberOfPlayers();
        int currentTurnID = GameplayManager.s_currentPlayerTurn = 0;
        m_gpManager.ChangeCurrentTurn();
        Assert.True(GameplayManager.s_currentPlayerTurn == currentTurnID + 1);

        yield return true;
    }

    [UnityTest, Order(3)]
    public IEnumerator TurnGoesBackToPlayerOne()
    {
        int numOfPlayers = GameController.GetNumberOfPlayers();
        GameplayManager.s_currentPlayerTurn = numOfPlayers - 1;
        Assert.True(GameplayManager.s_currentPlayerTurn == GameController.GetNumberOfPlayers() - 1);

        m_gpManager.ChangeCurrentTurn();
        Assert.True(GameplayManager.s_currentPlayerTurn == 0);
        yield return true;
    }

    [UnityTest, Order(4)]
    public IEnumerator LoopThroughAllPlayers()
    {
        int numOfPlayers = GameController.GetNumberOfPlayers();
        GameplayManager.s_currentPlayerTurn = 0;
        
        for (int i = 0; i < numOfPlayers; i++)
        {           
            Assert.True(GameplayManager.s_currentPlayerTurn == i);
            m_gpManager.ChangeCurrentTurn();
        }

        Assert.True(GameplayManager.s_currentPlayerTurn == 0); 
        yield return true;
    }

}
