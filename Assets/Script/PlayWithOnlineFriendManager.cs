#region PlayWithOnlineFriend Method

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayWithOnlineFriendManager : MonoBehaviourPunCallbacks
{
    #region Game UI

    public static PlayWithOnlineFriendManager Instance { get; private set; }
    [Header("RoomManager")]
    [SerializeField] private TMP_InputField RoomNameInputField;
    [SerializeField] private GameObject RoomCreatePanel, JoinRoomPanel, RoomListPanel;
    [SerializeField] private Button CreateButton, JoinButton, BackButton, JoinRoomButton, DoneButton;
    [SerializeField] private TMP_Text RoomCodeText, StatusText, ErrorText;

    [Header("GamePanel")]
    [SerializeField] private GameObject PlayWithOnlineFriendPanel;
    [SerializeField] private TMP_Text YourName, OpponentsName, Status;

    [Header("Assets for Game")]
    [SerializeField] private GameObject ResultPopUp;
    [SerializeField] private Button ResetButton, ExitButton;
    [SerializeField] private Button[] CellButtons;

    [SerializeField] private TMP_Text ResultText; // Text to show winner or draw in popup
    [SerializeField] private Button CancelButton, PlayAgainButton;   //Buttons in Result Popup
    private int[] boardState = new int[9]; // 0 for empty, 1 for Player, 2 for 2nd Player
    private int turn = 0; // 0 for Player, 1 for  2nd Player
    private string roomCode;
    private bool isPlayerTurn;

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
        RoomCreatePanel.SetActive(true);
        JoinRoomPanel.SetActive(false);
        RoomListPanel.SetActive(false);

        JoinButton.onClick.AddListener(OnClickJoin);
        CreateButton.onClick.AddListener(CreateRoom);
        JoinRoomButton.onClick.AddListener(JoinRoom);
        DoneButton.onClick.AddListener(OnClickDoneButton);
        BackButton.onClick.AddListener(BackToCreateRoom);

        // Initialize board state
        foreach (Button cellButton in CellButtons)
        {
            cellButton.onClick.AddListener(() => onClickCellButton(cellButton));
        }
        ResultPopUp.SetActive(false);
        ExitButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Disconnect();
            }
            Debug.Log("Exit button clicked. Leaving the room...");
            // PhotonNetwork.LeaveRoom();
            LobbyManager.Instance.BackToLobby();
        });
        ResetButton.onClick.AddListener(ResetGame);
        PlayAgainButton.onClick.AddListener(PlayAgain);
        CancelButton.onClick.AddListener(ClosePopUp);
        ResetGame();
    }

    #region Room Create

    public void CreateRoom()
    {
        // Generate random room code
        roomCode = Random.Range(1000, 9999).ToString();
        PhotonNetwork.ConnectUsingSettings();
        // Create Photon Room
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(roomCode, options);

        // Update UI
        RoomCodeText.text = "Room Code: " + roomCode;
        StatusText.text = "Waiting for a friend to join...";
        RoomCreatePanel.SetActive(false);
        RoomListPanel.SetActive(true);
    }

    public void OnClickDoneButton()
    {
        // Transition to Game Panel when both players are ready
        RoomListPanel.SetActive(false);
        PlayWithOnlineFriendPanel.SetActive(true);
        YourName.text = LobbyManager.Instance.PlayerName;
        Status.text = isPlayerTurn ? "Your Turn" : "Opponent's Turn";
    }

    public void OnClickJoin()
    {
        JoinRoomPanel.SetActive(true);
        RoomCreatePanel.SetActive(false);
    }

    public void BackToCreateRoom()
    {
        RoomCreatePanel.SetActive(true);
        JoinRoomPanel.SetActive(false);
    }
    public void JoinRoom()
    {
        // Join room with the entered code
        string enteredRoomCode = RoomNameInputField.text;
        if (!string.IsNullOrEmpty(enteredRoomCode))
        {
            PhotonNetwork.JoinRoom(enteredRoomCode);
        }
        else
        {
            ErrorText.text = "Please enter a valid room code!";
        }
    }
    public void ResetRoomState()
    {
        // Reset all room-related states
        roomCode = string.Empty;
        isPlayerTurn = false;

        // Reset UI elements related to room state
        RoomCreatePanel.SetActive(true); // Show room creation options
        JoinRoomPanel.SetActive(false);  // Hide join room panel
        RoomListPanel.SetActive(false);  // Hide room list panel

        // StatusText.text = "Create or Join a Room"; // Reset status text
        RoomCodeText.text = "";  // Clear room code
        ErrorText.text = "";  // Clear any errors
        YourName.text = "";
        OpponentsName.text = "";
        Status.text = "";

        // Reset board state, game UI, and any previous player data
        ResetGame();  // This method is used to reset the gameplay state (if applicable)
    }

    #endregion
    #region Gameplay Methods
    public void onClickCellButton(Button clickedButton)
    {
        int index = System.Array.IndexOf(CellButtons, clickedButton);

        // Ignore click if cell is already filled
        if (boardState[index] != 0)
            return;

        // Update board state
        boardState[index] = turn == 0 ? 1 : 2;

        // Show X or O based on turn
        Transform child = clickedButton.transform.GetChild(turn);
        if (child != null)
        {
            child.gameObject.SetActive(true);
        }

        // Check for winner or draw
        if (CheckWinner())
        {
            ShowResult(turn == 0 ? "Player X Wins!" : "Player O Wins!");
            // if (turn == 0)
            //     scoreX++;
            // else
            //     scoreO++;

            // UpdateScore();
            // return;
        }
        else if (IsDraw())
        {
            ShowResult("It's a Draw!");
            return;
        }

        // Switch turn
        turn = (turn + 1) % 2;
    }

    private bool CheckWinner()
    {
        int[,] winConditions = new int[,]
        {
            { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, // Rows
            { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, // Columns
            { 0, 4, 8 }, { 2, 4, 6 }              // Diagonals
        };

        for (int i = 0; i < winConditions.GetLength(0); i++)
        {
            int a = winConditions[i, 0];
            int b = winConditions[i, 1];
            int c = winConditions[i, 2];

            if (boardState[a] != 0 && boardState[a] == boardState[b] && boardState[b] == boardState[c])
            {
                return true; // Winner found
            }
        }

        return false; // No winner
    }

    private bool IsDraw()
    {
        foreach (int state in boardState)
        {
            if (state == 0)
                return false; // Empty cell found, not a draw
        }

        return true; // No empty cells, it's a draw
    }

    private void ShowResult(string resultMessage)
    {
        ResultText.text = resultMessage;
        ResultPopUp.SetActive(true);

        // Disable all cell buttons to prevent further moves
        foreach (Button cellButton in CellButtons)
        {
            cellButton.interactable = false;
        }
    }



    public void ResetGame()
    {
        // Reset board state
        for (int i = 0; i < boardState.Length; i++)
        {
            boardState[i] = 0;
            for (int j = 0; j < CellButtons[i].transform.childCount; j++)
            {
                CellButtons[i].transform.GetChild(j).gameObject.SetActive(false); // Deactivate children
            }
            CellButtons[i].interactable = true; // Make buttons clickable again
        }

        turn = 0; // Reset turn to Player X
        ResultPopUp.SetActive(false); // Hide result popup
    }

    public void PlayAgain()
    {
        ResetGame();
    }

    public void ClosePopUp()
    {
        ResultPopUp.SetActive(false);
    }
    #endregion
    #endregion
    #region Photon Callbacks

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // Update UI for the joined room
        RoomListPanel.SetActive(true);
        JoinRoomPanel.SetActive(false);

        RoomCodeText.text = "Room Code: " + PhotonNetwork.CurrentRoom.Name;
        StatusText.text = "You joined the room!";

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StatusText.text = "Both players joined! Starting the game...";
            Invoke(nameof(OnClickDoneButton), 2f);
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                // Set the opponent's name
                OpponentsName.text = $"{player.NickName}";
                Debug.Log("Opponent joined: " + player.NickName);
                break;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        // Update status when another player joins
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StatusText.text = "Both players joined! Starting the game...";
            Invoke(nameof(OnClickDoneButton), 2f);
        }

        // Identify opponent's name
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                // Set the opponent's name
                OpponentsName.text = $"{player.NickName}";
                break;
            }
        }

        Debug.Log("Opponent joined: " + newPlayer.NickName);
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        StatusText.text = "Failed to create room: " + message;
        RoomCreatePanel.SetActive(true);
        RoomListPanel.SetActive(false);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        StatusText.text = "Failed to join room: " + message;
        JoinRoomPanel.SetActive(true);
        RoomListPanel.SetActive(false);
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        // Notify both players that the other player left
        if (PhotonNetwork.IsMasterClient)
        {
            // If the player leaving is the opponent, the game will end, and the room will be closed
            Debug.Log($"{otherPlayer.NickName} left the room.");
            // Redirect to lobby if master client leaves
            LobbyManager.Instance.BackToLobby();
        }
        else
        {
            // If the current player is not the master client, just leave the room after the opponent leaves
            Debug.Log("Opponent left the room. You will be redirected to the lobby.");
            PhotonNetwork.LeaveRoom();
        }
    }


    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        Debug.Log("Player has left the room.");

        // When the player leaves the room, we return to the lobby and ensure the room is deleted
        LobbyManager.Instance.BackToLobby(); // Assuming this method handles UI redirection to the lobby

        // Optionally, disable game UI and show a message
        PlayWithOnlineFriendPanel.SetActive(false);
        Debug.Log($"You have left the room.");
    }





    #endregion
}

#endregion