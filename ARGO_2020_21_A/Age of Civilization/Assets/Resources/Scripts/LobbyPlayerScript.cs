using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyPlayerScript : NetworkBehaviour
{
    [SerializeField]
    private LobbyScript m_lobbyScript;

    [SerializeField]
    private GameObject m_lobbyUI = null;

    [SerializeField]
    private GameObject m_seedUI = null;

    [SerializeField]
    private Button m_startButton = null;

    [SerializeField]
    private List<Text> m_playerReadyTexts = new List<Text>();

    [SerializeField]
    private List<GameObject> m_playerUIObjects = new List<GameObject>();

    [SyncVar(hook = nameof(HandleReadyStatus))]
    public bool m_isReady = false;

    [SyncVar(hook = nameof(HandleSeedName))]
    public string m_seed = string.Empty;

    private bool m_isHost;
    public bool IsHost
    {
        set
        {
            m_isHost = value;
            m_startButton.gameObject.SetActive(value);
        }
    }

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

    public void HandleReadyStatus(bool oldValue, bool newValue) => UpdateUI();
    public void HandleSeedName(string oldValue, string newValue) => UpdateUI();

    private void Start()
    {
        if (!m_isHost)
        {
            m_startButton.gameObject.SetActive(false);
            m_seedUI.gameObject.SetActive(false);
        }
    }

    public override void OnStartAuthority()
    {
        m_lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.m_roomPlayers.Add(this);

        UpdateUI();
    }

    public override void OnStopClient()
    {
        Room.m_roomPlayers.Remove(this);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.m_roomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateUI();

                    break;
                }
            }
            return;
        }

        for (int i = 0; i < Room.m_roomPlayers.Count; i++)
        {
            m_playerReadyTexts[i].text = Room.m_roomPlayers[i].m_isReady ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
        }

        switch (Room.m_roomPlayers.Count)
        {
            case 1:
                m_lobbyScript.m_activeCivs = 1;
                m_playerUIObjects[0].gameObject.SetActive(true);
                m_playerUIObjects[1].gameObject.SetActive(false);
                m_playerUIObjects[2].gameObject.SetActive(false);
                m_playerUIObjects[3].gameObject.SetActive(false);
                break;
            case 2:
                m_lobbyScript.m_activeCivs = 2;
                m_playerUIObjects[0].gameObject.SetActive(true);
                m_playerUIObjects[1].gameObject.SetActive(true);
                m_playerUIObjects[2].gameObject.SetActive(false);
                m_playerUIObjects[3].gameObject.SetActive(false);
                break;
            case 3:
                m_lobbyScript.m_activeCivs = 3;
                m_playerUIObjects[0].gameObject.SetActive(true);
                m_playerUIObjects[1].gameObject.SetActive(true);
                m_playerUIObjects[2].gameObject.SetActive(true);
                m_playerUIObjects[3].gameObject.SetActive(false);
                break;
            case 4:
                m_lobbyScript.m_activeCivs = 4;
                m_playerUIObjects[0].gameObject.SetActive(true);
                m_playerUIObjects[1].gameObject.SetActive(true);
                m_playerUIObjects[2].gameObject.SetActive(true);
                m_playerUIObjects[3].gameObject.SetActive(true);
                break;
        }
    }

    public void HandleReadyToStart(bool ready)
    {
        if (!m_isHost)
        {
            m_startButton.interactable = false;
            return;
        }
        m_startButton.interactable = ready;
    }

    //(ignoreAuthority = true)
    [Command]
    public void CmdReadyUp()
    {
        GetComponent<NetworkIdentity>().AssignClientAuthority(GetComponent<NetworkIdentity>().connectionToClient);
        m_isReady = !m_isReady;

        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if (Room.m_roomPlayers[0].connectionToClient != connectionToClient) 
        { 
            return; 
        }

        Room.StartGame();
    }
}
