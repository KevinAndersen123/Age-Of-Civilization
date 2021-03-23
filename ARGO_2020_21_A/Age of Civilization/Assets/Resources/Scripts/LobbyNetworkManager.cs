using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class LobbyNetworkManager : NetworkManager
{
    [Scene]
    [SerializeField]
    private string menuScene = string.Empty;

    [Header("Room Prefab")]
    [SerializeField]
    private LobbyPlayerScript roomPlayerPrefab = null;

    [Header("Game Prefab")]
    [SerializeField]
    private GamePlayerScript gamePlayerPrefab = null;
    [SerializeField]
    private GameObject m_playerSpawner = null;

    public List<LobbyPlayerScript> m_roomPlayers { get; } = new List<LobbyPlayerScript>();
    public List<GamePlayerScript> m_gamePlayers { get; } = new List<GamePlayerScript>();

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;

    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }   

    public override void OnStartClient()
    {
        var prefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in prefabs)
        {
            ClientScene.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().path != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            bool isHost = m_roomPlayers.Count == 0;

            LobbyPlayerScript roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsHost = isHost;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            roomPlayerInstance.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<LobbyPlayerScript>();

            m_roomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("MP Gameplay"))
        {
            for (int i = m_roomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = m_roomPlayers[i].connectionToClient;
                var gamePlayerInstance = Instantiate(gamePlayerPrefab);

                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject, true);
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnStopServer()
    {
        OnServerStopped?.Invoke();

        m_roomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in m_roomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        foreach (var player in m_roomPlayers)
        {
            if (!player.m_isReady) 
            { 
                return false; 
            }
        }

        return true;
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            if (!IsReadyToStart()) 
            {
                return; 
            }

            ServerChangeScene("MP Gameplay");
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith("MP Gameplay"))
        {
            GameObject playerSpawnSystemInstance = Instantiate(m_playerSpawner);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}
