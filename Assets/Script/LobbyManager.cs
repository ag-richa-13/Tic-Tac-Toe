using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private GameObject GameWithAIPanel;
    [SerializeField] private GameObject GameWithFriendPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional, keeps LobbyManager persistent
    }
    private void Start()
    {
        LobbyPanel.SetActive(true);
        GameWithFriendPanel.SetActive(false);
        GameWithAIPanel.SetActive(false);
    }

    public void ShowGameWithAI()
    {
        LobbyPanel.SetActive(false);
        GameWithAIPanel.SetActive(true);
    }

    public void ShowGameWithFriend()
    {
        LobbyPanel.SetActive(false);
        GameWithFriendPanel.SetActive(true);
    }

    public void BackToLobby()
    {
        GameWithAIPanel.SetActive(false);
        GameWithFriendPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }
}
