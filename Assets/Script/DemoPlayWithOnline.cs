// #region Play With Online Friends Using Photon PUN 2

// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using Photon.Pun;
// using Photon.Realtime;
// using System;

// public class PlayWithOnlineFriendManager : MonoBehaviourPunCallbacks
// {
//     #region  Initialization
//     public static PlayWithOnlineFriendManager Instance { get; private set; }
//     [Header("RoomManager")]
//     [SerializeField] private TMP_InputField RoomNameInputField;  // Only to join room 
//     [SerializeField] private GameObject RoomCreatePanel, JoinRoomPanel, RoomListPanel, ErrorPrompt;
//     [SerializeField] private Button CreateButton, JoinButton, BackButton, JoinRoomButton, DoneButton;
//     [SerializeField] private TMP_Text RoomCodeText, StatusText, ErrorText, ErrorPromptText;

//     [Header("GamePanel")]
//     [SerializeField] private GameObject PlayWithOnlineFriendPanel, ConnectionPanel;
//     [SerializeField] private TMP_Text YourName, OpponentsName, Status, ConnectionText;

//     [Header("Assets for Game")]
//     [SerializeField] private GameObject ResultPopUp;
//     [SerializeField] private Button ResetButton, ExitButton;
//     [SerializeField] private Button[] CellButtons;

//     [SerializeField] private TMP_Text ResultText; // Text to show winner or draw in popup
//     [SerializeField] private Button CancelButton, PlayAgainButton;   //Buttons in Result Popup
//     private int[] boardState = new int[9]; // 0 for empty, 1 for Player, 2 for 2nd Player
//     private int turn = 0; // 0 for Player, 1 for  2nd Player
//     private string roomCode;
//     private bool isPlayerTurn;
//     private bool isReadyToCreateRoom = false;
//     #endregion

//     private void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }
//         Instance = this;
//     }

//     private void Start()
//     {
//         PlayWithOnlineFriendPanel.SetActive(true);
//         ConnectionPanel.SetActive(false);
//         RoomCreatePanel.SetActive(true);
//         JoinRoomPanel.SetActive(false);
//         RoomListPanel.SetActive(false);
//         ErrorPrompt.SetActive(false);

//         //Button Click 

//         JoinButton.onClick.AddListener(OnClickJoin);
//         CreateButton.onClick.AddListener(CreateRoom);
//         JoinRoomButton.onClick.AddListener(JoinRoom);
//         DoneButton.onClick.AddListener(OnClickDoneButton);
//         BackButton.onClick.AddListener(BackToCreateRoom);

//         ExitButton.onClick.AddListener(() =>
//         {

//             // Write logic for Exiting the current room for both the player
//             LobbyManager.Instance.BackToLobby();
//         });
//     }

//     #region Room Create & Join
//     public void CreateRoom() { }
//     public void OnClickJoin()
//     {
//         RoomCreatePanel.SetActive(false);
//         JoinRoomPanel.SetActive(true);
//     }
//     public void BackToCreateRoom()
//     {
//         JoinRoomPanel.SetActive(false);
//         RoomCreatePanel.SetActive(true);
//     }

//     public void JoinRoom() { }
//     public void OnClickDoneButton()
//     {
//         RoomListPanel.SetActive(false);
//         // YourName.text = LobbyManager.Instance.PlayerName;
//         //Status.text = isPlayerTurn ? "Your Turn" : "Opponent's Turn";
//     }
//     #endregion

//     #region Photon Callbacks
//     public override void OnConnectedToMaster()
//     {
//         base.OnConnectedToMaster();
//     }
//     public override void OnJoinedRoom()
//     {
//         base.OnJoinedRoom();
//     }

//     public override void OnPlayerEnteredRoom(Player newPlayer)
//     {
//         base.OnPlayerEnteredRoom(newPlayer);

//     }
//     public override void OnJoinRoomFailed(short returnCode, string message)
//     {
//         base.OnJoinRoomFailed(returnCode, message);
//     }
//     public override void OnCreateRoomFailed(short returnCode, string message)
//     {
//         base.OnCreateRoomFailed(returnCode, message);
//     }

//     public override void OnPlayerLeftRoom(Player otherPlayer)
//     {
//         base.OnPlayerLeftRoom(otherPlayer);
//     }

//     public override void OnDisconnected(DisconnectCause cause)
//     {
//         base.OnDisconnected(cause);
//     }

//     #endregion
// }
// #endregion