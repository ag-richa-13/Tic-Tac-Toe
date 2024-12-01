#region Play With Online Friends Using Photon PUN 2

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
// using ExitGames.Client.Photon;

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
    [SerializeField] private GameObject PlayWithOnlineFriendPanel;
    [SerializeField] private TMP_Text YourName, OpponentsName, Status;

    [Header("Assets for Game")]
    [SerializeField] private GameObject ResultPopUp;
    [SerializeField] private Button ResetButton, ExitButton;
    [SerializeField] private Button[] CellButtons;   // 9 Cell buttons for game 
    [SerializeField] private GameObject XPrefab, OPrefab;
    [SerializeField] private TMP_Text ResultText;
    [SerializeField] private Button CancelButton, PlayAgainButton;

    private string roomCode;
    private bool isMasterClient;
    private bool isPlayerTurn;
    private int[] gameBoard = new int[9]; // 0 = empty, 1 = "X", 2 = "O"
    private const int PLAYER_X = 1;
    private const int PLAYER_O = 2;
    private bool isGameActive = true;

    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (photonView == null)
        {
            Debug.LogError("PhotonView component is missing! Please attach it to the GameObject.");
        }
    }

    private void Start()
    {
        PlayWithOnlineFriendPanel.SetActive(true);
        RoomCreatePanel.SetActive(true);
        JoinRoomPanel.SetActive(false);
        RoomListPanel.SetActive(false);
        ErrorPrompt.SetActive(false);

        CreateButton.onClick.AddListener(CreateRoom);
        JoinButton.onClick.AddListener(OnClickJoin);
        JoinRoomButton.onClick.AddListener(JoinRoom);
        BackButton.onClick.AddListener(BackToCreateRoom);

        ExitButton.onClick.AddListener(() => StartCoroutine(LeaveRoomAndExit()));

        // Game Play
        for (int i = 0; i < CellButtons.Length; i++)
        {
            if (CellButtons[i] == null)
            {
                Debug.LogError($"CellButtons[{i}] is null! Assign all buttons in the Inspector.");
                return;
            }

            int index = i; // Fix lambda closure issue
            CellButtons[i].onClick.AddListener(() => OnCellClicked(index));
        }
        ResetGameBoard();
        ResetButton.onClick.AddListener(ResetGameBoard);
        PlayAgainButton.onClick.AddListener(PlayAgain);
        CancelButton.onClick.AddListener(CloseResultPopUp);
    }

    #region Room Create & Join
    public void CreateRoom()
    {
        roomCode = GenerateRoomCode();
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(roomCode, options, TypedLobby.Default);
        Debug.Log($"Room {roomCode} created");
        RoomListPanel.SetActive(true);
        RoomCreatePanel.SetActive(false);
        RoomCodeText.text = roomCode;
        StatusText.text = "Waiting for a friend to join...";
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
        Debug.Log($"Joining room: {roomName}");
        RoomListPanel.SetActive(true);
        JoinRoomPanel.SetActive(false);

    }
    #endregion

    #region Photon Callbacks
    public override void OnJoinedRoom()
    {
        isMasterClient = PhotonNetwork.IsMasterClient;
        YourName.text = PhotonNetwork.LocalPlayer.NickName;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            OpponentsName.text = PhotonNetwork.PlayerListOthers[0].NickName;
            Debug.Log($"Opponent joined: {PhotonNetwork.PlayerListOthers[0].NickName}");

            if (isMasterClient)
            {
                RoomCodeText.text = roomCode;
                StatusText.text = $"{PhotonNetwork.PlayerListOthers[0].NickName} joined your room.";
            }
            else
            {
                RoomCodeText.text = PhotonNetwork.CurrentRoom.Name;
                StatusText.text = $"You joined {PhotonNetwork.MasterClient.NickName}'s room.";
            }

            StartCoroutine(TransitionToGameplay());
        }
        else
        {
            RoomCodeText.text = roomCode;
            StatusText.text = "Waiting for opponent...";
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            OpponentsName.text = newPlayer.NickName;
            Debug.Log($"Opponent joined: {newPlayer.NickName}");

            if (isMasterClient)
            {
                RoomCodeText.text = roomCode;
                StatusText.text = $"{newPlayer.NickName} joined your room.";
            }

            StartCoroutine(TransitionToGameplay());
        }
    }


    private IEnumerator TransitionToGameplay()
    {

        yield return new WaitForSeconds(2f); // Wait for 5 seconds
        RoomListPanel.SetActive(false);
        SetupGame();
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {

            ShowError($"{otherPlayer.NickName} left the game.");

            StartCoroutine(LeaveRoomAndExit());
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
        isPlayerTurn = isMasterClient;  // Master client starts the game
        Status.text = isPlayerTurn ? "Your Turn" : "Opponent's Turn";
        OpponentsName.text = PhotonNetwork.PlayerListOthers[0].NickName;

        // // Reset the game board
        ResetGameBoard();
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

    public IEnumerator LeaveRoomAndExit()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Leaving room...");
            PhotonNetwork.LeaveRoom();
        }
        yield return new WaitForSeconds(2f);
        // Exit the application
        Debug.Log("Exiting application.");
        Application.Quit();
    }

    #region Game Logic
    private void OnCellClicked(int index)
    {
        if (!isPlayerTurn || gameBoard[index] != 0 || !isGameActive) return;

        if (photonView == null)
        {
            Debug.LogError("PhotonView is null! Ensure the GameObject has a PhotonView component.");
            return;
        }

        gameBoard[index] = isMasterClient ? PLAYER_X : PLAYER_O;

        GameObject marker = Instantiate(isMasterClient ? XPrefab : OPrefab, CellButtons[index].transform);
        marker.transform.localPosition = Vector3.zero;

        isPlayerTurn = false;
        Status.text = "Opponent's Turn";

        photonView.RPC("UpdateBoard", RpcTarget.Others, index, isMasterClient ? PLAYER_X : PLAYER_O);

        CheckGameState();
    }


    [PunRPC]
    private void UpdateBoard(int index, int player)
    {
        gameBoard[index] = player;

        GameObject marker = Instantiate(player == PLAYER_X ? XPrefab : OPrefab, CellButtons[index].transform);
        marker.transform.localPosition = Vector3.zero; // Center prefab in the button

        isPlayerTurn = true;
        Status.text = "Your Turn";

        CheckGameState();
    }


    private void CheckGameState()
    {
        if (CheckWinCondition())
        {
            string result = isPlayerTurn ? "You Lose!" : "You Win!";
            ResultText.text = result;

            // Track the winner
            if (isPlayerTurn)
            {
                // Player X wins
                isMasterClient = false;  // Assign to opponent as master client for the next round
            }
            else
            {
                // Player O wins
                isMasterClient = true;  // The player who won will get the first turn in the next game
            }

            isGameActive = false;
            ResultPopUp.SetActive(true);
        }
        else if (IsDraw())
        {
            ResultText.text = "It's a Draw!";
            isGameActive = false;
            ResultPopUp.SetActive(true);
        }
    }


    private bool CheckWinCondition()
    {
        int[,] winPatterns = {
        { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, // Rows
        { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, // Columns
        { 0, 4, 8 }, { 2, 4, 6 }              // Diagonals
    };

        for (int i = 0; i < winPatterns.GetLength(0); i++)
        {
            if (gameBoard[winPatterns[i, 0]] != 0 &&
                gameBoard[winPatterns[i, 0]] == gameBoard[winPatterns[i, 1]] &&
                gameBoard[winPatterns[i, 0]] == gameBoard[winPatterns[i, 2]])
            {
                return true;
            }
        }

        return false;
    }

    private bool IsDraw()
    {
        foreach (int cell in gameBoard)
        {
            if (cell == 0) return false;
        }
        return true;
    }

    // Reset the game board for both players
    // Reset the game board for both players
    public void ResetGameBoard()
    {
        // Check if we are in a room and if it's this player's turn to perform the action
        if (PhotonNetwork.InRoom)
        {
            // Perform the reset action for both players
            photonView.RPC("SyncResetGameBoard", RpcTarget.All);
        }
    }
    // Play again for both players
    public void PlayAgain()
    {
        if (PhotonNetwork.InRoom)
        {
            // Send play again request to both players
            photonView.RPC("SyncPlayAgain", RpcTarget.All);
        }
    }
    [PunRPC]
    private void SyncResetGameBoard()
    {
        // Reset game board for both players (this is called by both players)
        for (int i = 0; i < gameBoard.Length; i++)
        {
            gameBoard[i] = 0;
        }

        foreach (var cell in CellButtons)
        {
            foreach (Transform child in cell.transform)
            {
                Destroy(child.gameObject);
            }
        }

        isGameActive = true;
        isPlayerTurn = isMasterClient;
        Status.text = isPlayerTurn ? "Your Turn" : "Opponent's Turn";
    }

    [PunRPC]
    private void SyncPlayAgain()
    {
        // Reset game board for both players and show the play again state
        ResetGameBoard();
        ResultPopUp.SetActive(false);
    }


    private void CloseResultPopUp()
    {
        ResultPopUp.SetActive(false);
    }
    #endregion
    #endregion
}
#endregion