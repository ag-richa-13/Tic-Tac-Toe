using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Text.RegularExpressions;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    #region Initialization
    public static LobbyManager Instance { get; private set; }
    [Header("UserNamePanel")]
    [SerializeField] private GameObject UserNamePanel;
    [SerializeField] private TMP_InputField NameInputField;
    [SerializeField] private TMP_Text UserNameErrorText;
    [SerializeField] private Button DoneButton;

    [Header("LobbyPanel")]
    [SerializeField] private TMP_Text WelcomeText, ConnectionText;
    [SerializeField] private GameObject ConnectionPanel, LobbyPanel, LobbyOnlineGamePlay, PlayWithFriendPanel, PlayWithAIPanel, PlayWithOnlineFriendPanel, ErrorPrompt;
    [SerializeField] private Button PlayWithFriendButton, PlayWithAIButton, PlayOnlineButton;
    private string playerName;
    public string PlayerName => playerName;
    #endregion
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
        ErrorPrompt.SetActive(false);
        ConnectionPanel.SetActive(false); // Make sure the connection panel is hidden initially
        DoneButton.onClick.AddListener(OnClickDoneButton);
        PlayWithAIButton.onClick.AddListener(PlayWithAI);
        PlayWithFriendButton.onClick.AddListener(PlayWithFriend);
        PlayOnlineButton.onClick.AddListener(PlayWithOnlineFriend);
    }

    private void Update()
    {
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

        // Length validation
        if (UserName.Length < 3 || UserName.Length > 8)
        {
            UserNameErrorText.text = "Username must be between 3 and 8 characters!";

            return;
        }

        // Alphanumeric validation
        if (!Regex.IsMatch(UserName, @"^[a-zA-Z0-9]+$"))
        {
            UserNameErrorText.text = "Username must be alphanumeric (letters and numbers only)!";

            return;
        }

        // No spaces validation
        if (UserName.Contains(" "))
        {
            UserNameErrorText.text = "Username cannot contain spaces!";

            return;
        }

        // No special characters validation
        if (UserName.IndexOfAny(new char[] { '!', '@', '#', '$', '%', '^', '&', '*' }) != -1)
        {
            UserNameErrorText.text = "Username cannot contain special characters!";

            return;
        }

        // First character must be a letter
        if (!char.IsLetter(UserName[0]))
        {
            UserNameErrorText.text = "Username must start with a letter!";

            return;
        }

        // Set Photon Nickname and proceed
        playerName = UserName;
        PhotonNetwork.LocalPlayer.NickName = playerName;

        // Show connection panel and start connection
        ConnectionPanel.SetActive(true);
        ConnectionText.text = "Connecting to Server...";
        PhotonNetwork.ConnectUsingSettings(); // Connect to Photon server

        UserNamePanel.SetActive(false);
        WelcomeText.text = $"Welcome to lobby, {playerName}!";
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
        LobbyOnlineGamePlay.SetActive(true);

        // PlayWithOnlineFriendManager.Instance.ResetRoomState();
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

        // Once connected to Master, hide the connection panel and update the message
        ConnectionPanel.SetActive(false); // Hide connection panel
        // LobbyPanel.SetActive(true); // Show lobby panel

        // Allow the user to proceed to other actions
        PlayWithFriendButton.interactable = true;
        PlayWithAIButton.interactable = true;
        PlayOnlineButton.interactable = true;
    }
    #endregion

}


