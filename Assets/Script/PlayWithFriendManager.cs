using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayWithFriendManager : MonoBehaviour
{
    public static PlayWithFriendManager Instance { get; private set; }

    [SerializeField] private TMP_Text Score_X, Score_O; // Score display for X and O
    [SerializeField] private Button ResetButton, ExitButton;
    [SerializeField] private Button[] CellButtons;
    [SerializeField] private GameObject ResultPopUp;
    [SerializeField] private TMP_Text ResultText; // Text to show winner or draw in popup
    [SerializeField] private Button CancelButton, PlayAgainButton;

    private int turn = 0; // 0 for Player X, 1 for Player O
    private int[] boardState = new int[9]; // 0 for empty, 1 for X, 2 for O
    private int scoreX = 0, scoreO = 0; // Track scores

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
        ResetButton.onClick.AddListener(ResetGame);
        ExitButton.onClick.AddListener(() => LobbyManager.Instance.BackToLobby());
        PlayAgainButton.onClick.AddListener(PlayAgain);
        CancelButton.onClick.AddListener(ClosePopup);

        foreach (Button cellButton in CellButtons)
        {
            cellButton.onClick.AddListener(() => onClickCellButton(cellButton));
        }

        ResetGame();
    }


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
            if (turn == 0)
                scoreX++;
            else
                scoreO++;

            UpdateScore();
            return;
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

    private void UpdateScore()
    {
        Score_X.text = $"{scoreX}";
        Score_O.text = $"{scoreO}";
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

    public void ClosePopup()
    {
        ResultPopUp.SetActive(false);
    }
}
