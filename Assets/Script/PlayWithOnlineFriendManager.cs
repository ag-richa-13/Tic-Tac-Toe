#region Play With Online Friends Using Photon PUN 2

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PlayWithOnlineFriendManager : MonoBehaviourPunCallbacks
{
    #region Initialization
    public static PlayWithOnlineFriendManager Instance { get; private set; }
    [Header("RoomManager")]
    [SerializeField] private TMP_InputField RoomNameInputField;
    [SerializeField] private GameObject RoomCreatePanel, JoinRoomPanel, RoomListPanel, ErrorPrompt;
    [SerializeField] private Button CreateButton, JoinButton, BackButton, JoinRoomButton, DoneButton;
    [SerializeField] private TMP_Text RoomCodeText, StatusText, ErrorPromptText;

    [Header("GamePanel")]
    [SerializeField] private GameObject PlayWithOnlineFriendPanel, ConnectionPanel;
    [SerializeField] private TMP_Text YourName, OpponentsName, Status, ConnectionText;

    [Header("Assets for Game")]
    [SerializeField] private GameObject ResultPopUp;
    [SerializeField] private Button ResetButton, ExitButton;
    [SerializeField] private Button[] CellButtons;

    [SerializeField] private TMP_Text ResultText;
    [SerializeField] private Button CancelButton, PlayAgainButton;

    private string roomCode;
    private bool isMasterClient;
    private bool isPlayerTurn;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PlayWithOnlineFriendPanel.SetActive(true);
        ConnectionPanel.SetActive(false);
        RoomCreatePanel.SetActive(true);
        JoinRoomPanel.SetActive(false);
        RoomListPanel.SetActive(false);
        ErrorPrompt.SetActive(false);

        CreateButton.onClick.AddListener(CreateRoom);
        JoinButton.onClick.AddListener(OnClickJoin);
        JoinRoomButton.onClick.AddListener(JoinRoom);
        BackButton.onClick.AddListener(BackToCreateRoom);

        ExitButton.onClick.AddListener(LeaveRoomAndExit);
    }

    #region Room Create & Join
    public void CreateRoom()
    {
        roomCode = GenerateRoomCode();
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(roomCode, options, TypedLobby.Default);
        Debug.Log($"Room {roomCode} created");
        ShowConnectionPanel($"Creating room {roomCode}...");
    }

    private string GenerateRoomCode()
    {
        return UnityEngine.Random.Range(1000, 9999).ToString(); // Random 4-digit code
    }

    public void OnClickJoin()
    {
        RoomCreatePanel.SetActive(false);
        JoinRoomPanel.SetActive(true);
    }

    public void BackToCreateRoom()
    {
        JoinRoomPanel.SetActive(false);
        RoomCreatePanel.SetActive(true);
    }

    public void JoinRoom()
    {
        string roomName = RoomNameInputField.text;
        if (string.IsNullOrEmpty(roomName))
        {
            ShowError("Room code cannot be empty!");
            return;
        }

        PhotonNetwork.JoinRoom(roomName);
        ShowConnectionPanel($"Joining room {roomName}...");
    }
    #endregion

    #region Photon Callbacks
    public override void OnJoinedRoom()
    {
        ConnectionPanel.SetActive(false);
        RoomCreatePanel.SetActive(false);
        JoinRoomPanel.SetActive(false);

        isMasterClient = PhotonNetwork.IsMasterClient;
        YourName.text = PhotonNetwork.LocalPlayer.NickName;
        OpponentsName.text = "Waiting for opponent...";
        Status.text = "Waiting for opponent...";

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            SetupGame();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            OpponentsName.text = newPlayer.NickName;
            SetupGame();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            ShowError("Opponent left the game. Returning to lobby...");
            LeaveRoomAndExit();
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ShowError($"Failed to create room: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ShowError($"Failed to join room: {message}");
    }
    #endregion

    #region Game Management
    private void SetupGame()
    {
        isPlayerTurn = isMasterClient;
        Status.text = isPlayerTurn ? "Your Turn" : "Opponent's Turn";
        OpponentsName.text = PhotonNetwork.PlayerListOthers[0].NickName;
    }

    private void ShowConnectionPanel(string message)
    {
        ConnectionPanel.SetActive(true);
        ConnectionText.text = message;
    }

    private void ShowError(string message)
    {
        ErrorPrompt.SetActive(true);
        ErrorPromptText.text = message;
        Invoke(nameof(HideError), 3f); // Automatically hide after 3 seconds
    }

    private void HideError()
    {
        ErrorPrompt.SetActive(false);
    }

    public void LeaveRoomAndExit()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        LobbyManager.Instance.BackToLobby();
    }
    #endregion
}
#endregion
