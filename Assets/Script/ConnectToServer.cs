// #region PlayWithOnlineFriend Method


// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using Photon.Pun;
// using Photon.Realtime;

// public class PlayWithOnlineFriendManager : MonoBehaviourPunCallbacks
// {
//     #region Game UI

//     #region  Initialization
//     public static PlayWithOnlineFriendManager Instance { get; private set; }
//     [Header("RoomManager")]
//     [SerializeField] private TMP_InputField RoomNameInputField;
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
//     private string pendingRoomCode = null;
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
//         RoomCreatePanel.SetActive(true);
//         JoinRoomPanel.SetActive(false);
//         RoomListPanel.SetActive(false);
//         ErrorPrompt.SetActive(false);
//         ConnectionPanel.SetActive(false);

//         JoinButton.onClick.AddListener(OnClickJoin);
//         CreateButton.onClick.AddListener(CreateRoom);
//         JoinRoomButton.onClick.AddListener(JoinRoom);
//         DoneButton.onClick.AddListener(OnClickDoneButton);
//         BackButton.onClick.AddListener(BackToCreateRoom);

//         // Initialize board state
//         foreach (Button cellButton in CellButtons)
//         {
//             cellButton.onClick.AddListener(() => onClickCellButton(cellButton));
//         }
//         ResultPopUp.SetActive(false);
//         ExitButton.onClick.AddListener(() =>
//         {
//             if (PhotonNetwork.IsMasterClient)
//             {
//                 PhotonNetwork.Disconnect();
//             }
//             Debug.Log("Exit button clicked. Leaving the room...");
//             ConnectionPanel.SetActive(true);
//             ConnectionText.text = "Connecting to Master Server...";
//             PhotonNetwork.ConnectUsingSettings(); // Connect to the Photon Master Server

//             LobbyManager.Instance.BackToLobby();
//         });
//         ResetButton.onClick.AddListener(ResetGame);
//         PlayAgainButton.onClick.AddListener(PlayAgain);
//         CancelButton.onClick.AddListener(ClosePopUp);
//         ResetGame();
//     }

//     #region Room Create & Join
//     public void CreateRoom()
//     {
//         if (!PhotonNetwork.IsConnectedAndReady)
//         {
//             Debug.Log("Photon not ready. Waiting for connection...");
//             isReadyToCreateRoom = true;
//             pendingRoomCode = Random.Range(1000, 9999).ToString();
//             ConnectionPanel.SetActive(true);
//             ConnectionText.text = "Connecting to Master Server...";
//             PhotonNetwork.ConnectUsingSettings(); // Connect to Master Server
//             return;
//         }

//         // Generate random room code
//         roomCode = Random.Range(1000, 9999).ToString();

//         // Create Photon Room
//         RoomOptions options = new RoomOptions { MaxPlayers = 2 };
//         PhotonNetwork.CreateRoom(roomCode, options);

//         // Update UI
//         Debug.Log($"Room Created with code: {roomCode}");
//         RoomCodeText.text = "Room Code: " + roomCode;
//         StatusText.text = "Waiting for a friend to join...";
//         RoomCreatePanel.SetActive(false);
//         RoomListPanel.SetActive(true);
//     }

//     public void OnClickDoneButton()
//     {
//         // Transition to Game Panel when both players are ready
//         RoomListPanel.SetActive(false);
//         PlayWithOnlineFriendPanel.SetActive(true);
//         YourName.text = LobbyManager.Instance.PlayerName;
//         Status.text = isPlayerTurn ? "Your Turn" : "Opponent's Turn";
//     }

//     public void OnClickJoin()
//     {
//         JoinRoomPanel.SetActive(true);
//         RoomCreatePanel.SetActive(false);
//     }

//     public void BackToCreateRoom()
//     {
//         RoomCreatePanel.SetActive(true);
//         JoinRoomPanel.SetActive(false);
//     }


//     public void JoinRoom()
//     {
//         if (!PhotonNetwork.IsConnectedAndReady)
//         {
//             Debug.LogError("Photon is not connected to the master server.");
//             ConnectionPanel.SetActive(true);
//             ConnectionText.text = "Connecting to Master Server...";
//             PhotonNetwork.ConnectUsingSettings(); // Connect to Master Server if not connected
//             return;
//         }

//         string enteredRoomCode = RoomNameInputField.text;
//         if (!string.IsNullOrEmpty(enteredRoomCode))
//         {
//             PhotonNetwork.JoinRoom(enteredRoomCode);
//         }
//         else
//         {
//             ErrorText.text = "Please enter a valid room code!";
//         }
//     }



//     public void ResetRoomState()
//     {
//         // Reset all room-related states
//         roomCode = string.Empty;
//         isPlayerTurn = false;

//         // Reset UI elements related to room state
//         RoomCreatePanel.SetActive(true); // Show room creation options
//         JoinRoomPanel.SetActive(false);  // Hide join room panel
//         RoomListPanel.SetActive(false);  // Hide room list panel

//         // StatusText.text = "Create or Join a Room"; // Reset status text
//         RoomNameInputField.text = "";
//         // RoomCodeText.text = "";  // Clear room code
//         ErrorText.text = "";  // Clear any errors
//         YourName.text = "";
//         OpponentsName.text = "";
//         Status.text = "";

//         // Reset board state, game UI, and any previous player data
//         ResetGame();  // This method is used to reset the gameplay state (if applicable)
//     }

//     #endregion
//     #region Gameplay Methods
//     public void onClickCellButton(Button clickedButton)
//     {
//         int index = System.Array.IndexOf(CellButtons, clickedButton);

//         // Ignore click if cell is already filled
//         if (boardState[index] != 0)
//             return;

//         // Update board state
//         boardState[index] = turn == 0 ? 1 : 2;

//         // Show X or O based on turn
//         Transform child = clickedButton.transform.GetChild(turn);
//         if (child != null)
//         {
//             child.gameObject.SetActive(true);
//         }

//         // Check for winner or draw
//         if (CheckWinner())
//         {
//             ShowResult(turn == 0 ? "Player X Wins!" : "Player O Wins!");
//             // if (turn == 0)
//             //     scoreX++;
//             // else
//             //     scoreO++;

//             // UpdateScore();
//             // return;
//         }
//         else if (IsDraw())
//         {
//             ShowResult("It's a Draw!");
//             return;
//         }

//         // Switch turn
//         turn = (turn + 1) % 2;
//     }

//     private bool CheckWinner()
//     {
//         int[,] winConditions = new int[,]
//         {
//             { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, // Rows
//             { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, // Columns
//             { 0, 4, 8 }, { 2, 4, 6 }              // Diagonals
//         };

//         for (int i = 0; i < winConditions.GetLength(0); i++)
//         {
//             int a = winConditions[i, 0];
//             int b = winConditions[i, 1];
//             int c = winConditions[i, 2];

//             if (boardState[a] != 0 && boardState[a] == boardState[b] && boardState[b] == boardState[c])
//             {
//                 return true; // Winner found
//             }
//         }

//         return false; // No winner
//     }

//     private bool IsDraw()
//     {
//         foreach (int state in boardState)
//         {
//             if (state == 0)
//                 return false; // Empty cell found, not a draw
//         }

//         return true; // No empty cells, it's a draw
//     }

//     private void ShowResult(string resultMessage)
//     {
//         ResultText.text = resultMessage;
//         ResultPopUp.SetActive(true);

//         // Disable all cell buttons to prevent further moves
//         foreach (Button cellButton in CellButtons)
//         {
//             cellButton.interactable = false;
//         }
//     }



//     public void ResetGame()
//     {
//         // Reset board state
//         for (int i = 0; i < boardState.Length; i++)
//         {
//             boardState[i] = 0;
//             for (int j = 0; j < CellButtons[i].transform.childCount; j++)
//             {
//                 CellButtons[i].transform.GetChild(j).gameObject.SetActive(false); // Deactivate children
//             }
//             CellButtons[i].interactable = true; // Make buttons clickable again
//         }

//         turn = 0; // Reset turn to Player X
//         ResultPopUp.SetActive(false); // Hide result popup
//     }

//     public void PlayAgain()
//     {
//         ResetGame();
//     }

//     public void ClosePopUp()
//     {
//         ResultPopUp.SetActive(false);
//     }
//     #endregion
//     #endregion
//     #region Photon Callbacks
//     public override void OnConnectedToMaster()
//     {
//         base.OnConnectedToMaster();
//         Debug.Log("Connected to Master Server. Ready for matchmaking.");
//         ConnectionPanel.SetActive(false);
//         if (isReadyToCreateRoom && !string.IsNullOrEmpty(pendingRoomCode))
//         {
//             PhotonNetwork.CreateRoom(pendingRoomCode, new RoomOptions { MaxPlayers = 2 });
//             isReadyToCreateRoom = false; // Reset flag
//             pendingRoomCode = null; // Clear pending room code
//         }
//         else
//         {
//             // When not creating a room, just ensure connected
//             PhotonNetwork.ConnectUsingSettings(); // Make sure we are connected to the Master Server if not yet connected
//         }
//     }


//     public override void OnJoinedRoom()
//     {
//         base.OnJoinedLobby();
//         Debug.Log("Successfully connected to Photon Master Server and entered the Lobby.");

//         // Update UI for the joined room
//         RoomListPanel.SetActive(true);
//         JoinRoomPanel.SetActive(false);

//         RoomCodeText.text = "Room Code: " + PhotonNetwork.CurrentRoom.Name;
//         StatusText.text = "You joined the room!";

//         if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
//         {
//             Player opponentPlayer = null;

//             foreach (Player player in PhotonNetwork.PlayerList)
//             {
//                 if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
//                 {
//                     // Set the opponent's name
//                     OpponentsName.text = $"{player.NickName}";
//                     Debug.Log("Opponent joined: " + player.NickName);
//                     opponentPlayer = player; // Store the opponent's player object
//                     break;
//                 }
//             }

//             if (opponentPlayer != null)
//             {
//                 StatusText.text = $"You Joined {opponentPlayer.NickName}'s room!";
//                 Invoke(nameof(OnClickDoneButton), 2f);
//             }
//         }


//     }

//     public override void OnPlayerEnteredRoom(Player newPlayer)
//     {
//         base.OnPlayerEnteredRoom(newPlayer);
//         foreach (Player player in PhotonNetwork.PlayerList)
//         {
//             if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
//             {
//                 // Set the opponent's name
//                 OpponentsName.text = $"{player.NickName}";
//                 break;
//             }
//         }
//         // Update status when another player joins
//         if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
//         {
//             StatusText.text = $"{newPlayer.NickName} joined your room!";
//             Invoke(nameof(OnClickDoneButton), 2f);
//         }

//         // Identify opponent's name
//         Debug.Log("Opponent joined: " + newPlayer.NickName);
//     }


//     public override void OnCreateRoomFailed(short returnCode, string message)
//     {
//         base.OnCreateRoomFailed(returnCode, message);
//         ErrorPrompt.SetActive(true);
//         ErrorPromptText.text = "Failed to create room: " + message;

//         // Automatically hide the error prompt after 3 seconds
//         Invoke(nameof(HideErrorPrompt), 3f);
//     }

//     public override void OnJoinRoomFailed(short returnCode, string message)
//     {
//         base.OnJoinRoomFailed(returnCode, message);
//         ErrorPrompt.SetActive(true);
//         ErrorPromptText.text = "Failed to join room: " + message;

//         // Wait before retrying or showing additional guidance to the user
//         Invoke(nameof(HideErrorPrompt), 3f);
//     }



//     // Method to hide the error prompt
//     private void HideErrorPrompt()
//     {
//         ErrorPrompt.SetActive(false);
//     }



//     public override void OnPlayerLeftRoom(Player otherPlayer)
//     {
//         base.OnPlayerLeftRoom(otherPlayer);

//         // Notify both players that the other player left
//         if (PhotonNetwork.IsMasterClient)
//         {
//             // If the player leaving is the opponent, the game will end, and the room will be closed
//             Debug.Log($"{otherPlayer.NickName} left the room.");
//             // Redirect to lobby if master client leaves
//             LobbyManager.Instance.BackToLobby();

//         }
//         else
//         {
//             // If the current player is not the master client, just leave the room after the opponent leaves
//             Debug.Log("Opponent left the room. You will be redirected to the lobby.");
//             PhotonNetwork.LeaveRoom();
//         }
//         // ConnectionPanel.SetActive(true);
//         // ConnectionText.text = "Connecting to Master Server...";
//         // PhotonNetwork.ConnectUsingSettings();
//         // Debug.Log("Connecting to Master Server..." + LobbyManager.Instance.PlayerName);
//     }


//     public override void OnLeftRoom()
//     {
//         base.OnLeftRoom();

//         Debug.Log("Player has left the room.");

//         // When the player leaves the room, we return to the lobby and ensure the room is deleted
//         LobbyManager.Instance.BackToLobby(); // Assuming this method handles UI redirection to the lobby

//         // Optionally, disable game UI and show a message
//         PlayWithOnlineFriendPanel.SetActive(false);
//         Debug.Log($"You have left the room.");
//     }


//     public override void OnDisconnected(DisconnectCause cause)
//     {
//         Debug.LogError("Disconnected from Photon: " + cause);

//         // Reconnect to Master Server
//         if (cause != DisconnectCause.DisconnectByClientLogic)
//         {
//             ConnectionPanel.SetActive(true);
//             ConnectionText.text = "Connecting to Master Server...";
//             PhotonNetwork.ConnectUsingSettings();
//             Debug.Log("Connecting to Master Server..." + LobbyManager.Instance.PlayerName); // Reconnect if disconnected for other reasons
//         }
//     }


//     #endregion
// }

// #endregion