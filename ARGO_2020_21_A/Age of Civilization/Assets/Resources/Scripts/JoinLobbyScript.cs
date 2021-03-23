using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyScript : MonoBehaviour
{
    [SerializeField]
    private LobbyNetworkManager m_networkManager = null;

    [Header("UI")]
    [SerializeField]
    private GameObject m_ipInputPanel;

    [SerializeField]
    private InputField m_ipInputField;

    [SerializeField]
    private Button m_joinButton;

    private void OnEnable()
    {
        LobbyNetworkManager.OnClientConnected += HandleClientConnected;
        LobbyNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        LobbyNetworkManager.OnClientConnected -= HandleClientConnected;
        LobbyNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        string ipAddress = m_ipInputField.text;

        m_networkManager.networkAddress = ipAddress;
        m_networkManager.StartClient();

        m_joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        m_joinButton.interactable = true;

        gameObject.SetActive(false);
        m_ipInputPanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        m_joinButton.interactable = true;
    }
}
