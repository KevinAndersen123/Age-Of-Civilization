using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayerSpawn : NetworkBehaviour
{
    [SerializeField]
    private GameObject m_playerPrefab = null;

    [SerializeField]
    private GameObject m_gameplayManager = null;

    private int m_nextIndex = 0;

    public override void OnStartServer() => LobbyNetworkManager.OnServerReadied += SpawnPlayer;

    public override void OnStartClient()
    {

    }

    [ServerCallback]
    private void OnDestroy() => LobbyNetworkManager.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        for (int i = 0; i < GameController.s_numberOfPlayers; i++)
        {
            GameObject playerInstance = Instantiate(m_playerPrefab);
            playerInstance.transform.SetParent(transform);
            playerInstance.name = "Player " + (i + 1);
            m_gameplayManager.GetComponent<GameplayManager>().m_players.Add(playerInstance.GetComponent<Player>());
            NetworkServer.Spawn(playerInstance, conn);
        }
    }
}
