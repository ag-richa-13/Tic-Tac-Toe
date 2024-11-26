using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager Instance { get; private set; }
    [Header("UserNamePanel")]
    [SerializeField] private GameObject UserNamePanel;
    [SerializeField] private TMP_InputField NameInputField;
    [SerializeField] private TMP_Text ErrorText;
    [SerializeField] private Button DoneButton;

    [Header("LobbyPanel")]
    [SerializeField] private TMP_Text WelcomeText;
    [SerializeField] private GameObject LobbyPanel, PlayWithFriendPanel, PlayWithAIPanel, PlayWithOnlineFriendPanel;
    [SerializeField] private Button PlayWithFriendButton, PlayWithAIButton, PlayWithOnlineFriendButton;
    private string playerName;
    public string PlayerName => playerName;

    #region Lobby Actions
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
        UserNamePanel.SetActive(true);
        PlayWithAIPanel.SetActive(false);
        PlayWithFriendPanel.SetActive(false);
        PlayWithOnlineFriendPanel.SetActive(false);

        DoneButton.onClick.AddListener(OnClickDoneButton);
        PlayWithAIButton.onClick.AddListener(PlayWithAI);
        PlayWithFriendButton.onClick.AddListener(PlayWithFriend);
        PlayWithOnlineFriendButton.onClick.AddListener(PlayWithOnlineFriend);
    }

    private void Update()
    {

        // Debug.Log("Network State: " + PhotonNetwork.NetworkClientState);
        // Detect back button press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (LobbyPanel.activeSelf)
            {
                // If in lobby, close the app
                Application.Quit();
            }
            else
            {
                // If in game, go back to the lobby
                BackToLobby();
            }
        }

    }

    public void OnClickDoneButton()
    {
        string UserName = NameInputField.text;
        if (!string.IsNullOrEmpty(UserName))
        {
            playerName = UserName; // Assign the input username
        }
        else
        {
            // Generate a random username if none is provided
            playerName = "Player_" + Random.Range(100, 999);
        }

        PhotonNetwork.LocalPlayer.NickName = playerName; // Set Photon Nickname
        PhotonNetwork.ConnectUsingSettings(); // Connect to Photon server
        UserNamePanel.SetActive(false); // Hide username panel
        WelcomeText.text = $"Welcome to lobby, {playerName}!"; // Show welcome message
    }


    public void PlayWithAI()
    {
        LobbyPanel.SetActive(false);
        PlayWithAIPanel.SetActive(true);
    }

    public void PlayWithFriend()
    {
        LobbyPanel.SetActive(false);
        PlayWithFriendPanel.SetActive(true);
    }
    public void BackToLobby()
    {
        PlayWithAIManager.Instance?.ResetGame();
        PlayWithFriendManager.Instance?.ResetGame();
        PlayWithFriendPanel.SetActive(false);
        PlayWithAIPanel.SetActive(false);
        PlayWithOnlineFriendPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }
    public void PlayWithOnlineFriend()
    {
        LobbyPanel.SetActive(false);
        PlayWithOnlineFriendPanel.SetActive(true);

        PlayWithOnlineFriendManager.Instance.ResetRoomState();
    }

    #endregion

    #region Photon Callback
    public override void OnConnected()
    {
        Debug.Log($"Connected to server");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(playerName + " connected to Master");
    }
    #endregion
}
