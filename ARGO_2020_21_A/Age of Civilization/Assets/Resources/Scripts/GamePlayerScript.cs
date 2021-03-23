using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GamePlayerScript : NetworkBehaviour
{
    private LobbyNetworkManager m_room;
    private LobbyNetworkManager Room
    {
        get
        {
            if (m_room != null)
            {
                return m_room;
            }
            return m_room = NetworkManager.singleton as LobbyNetworkManager;
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        Room.m_gamePlayers.Add(this);
    }

    public override void OnStopClient()
    {
        Room.m_gamePlayers.Remove(this);
    }

    
}
