using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayWithAIManager : MonoBehaviour
{
    public static PlayWithAIManager Instance { get; private set; }

    [SerializeField] private TMP_Text Score_Player, Score_Bot; // Score display for Player and Bot
    [SerializeField] private Button ResetButton, ExitButton;
    [SerializeField] private Button[] CellButtons;
    [SerializeField] private GameObject ResultPopUp;
    [SerializeField] private TMP_Text ResultText; // Text to show winner or draw in popup
    [SerializeField] private Button CancelButton, PlayAgainButton;

    private int[] boardState = new int[9]; // 0 for empty, 1 for Player, 2 for Bot
    private int turn = 0; // 0 for Player, 1 for Bot
    private int scorePlayer = 0, scoreBot = 0;

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

    private void onClickCellButton(Button clickedButton)
    {
        int index = System.Array.IndexOf(CellButtons, clickedButton);

        // Ignore click if it's not the player's turn or the cell is already filled
        if (turn != 0 || boardState[index] != 0)
            return;

        // Player's move
        MakeMove(index, 1);

        // Check for game state after player's move
        if (CheckWinner(1))
        {
            ShowResult("You Win!");
            scorePlayer++;
            UpdateScore();
            return;
        }
        else if (IsDraw())
        {
            ShowResult("It's a Draw!");
            return;
        }

        // Bot's turn
        turn = 1;
        StartCoroutine(BotMove());
    }

    private IEnumerator BotMove()
    {
        yield return new WaitForSeconds(0.5f); // Add delay for bot's move for a better experience

        int bestMove = GetBestMove();
        MakeMove(bestMove, 2);

        // Check for game state after bot's move
        if (CheckWinner(2))
        {
            ShowResult("Bot Wins!");
            scoreBot++;
            UpdateScore();
        }
        else if (IsDraw())
        {
            ShowResult("It's a Draw!");
        }

        // Switch turn back to player
        turn = 0;
    }

    private void MakeMove(int index, int player)
    {
        boardState[index] = player;

        Transform child = CellButtons[index].transform.GetChild(player - 1);
        if (child != null)
        {
            child.gameObject.SetActive(true);
        }

        CellButtons[index].interactable = false;
    }

    private int GetBestMove()
    {
        // Try to win
        for (int i = 0; i < boardState.Length; i++)
        {
            if (boardState[i] == 0)
            {
                boardState[i] = 2; // Simulate bot move
                if (CheckWinner(2))
                {
                    boardState[i] = 0;
                    return i; // Bot can win
                }
                boardState[i] = 0; // Undo simulation
            }
        }

        // Block player from winning
        for (int i = 0; i < boardState.Length; i++)
        {
            if (boardState[i] == 0)
            {
                boardState[i] = 1; // Simulate player move
                if (CheckWinner(1))
                {
                    boardState[i] = 0;
                    return i; // Block player
                }
                boardState[i] = 0; // Undo simulation
            }
        }

        // Choose a random move
        List<int> availableMoves = new List<int>();
        for (int i = 0; i < boardState.Length; i++)
        {
            if (boardState[i] == 0)
                availableMoves.Add(i);
        }

        return availableMoves[Random.Range(0, availableMoves.Count)];
    }

    private bool CheckWinner(int player)
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

            if (boardState[a] == player && boardState[a] == boardState[b] && boardState[b] == boardState[c])
            {
                return true;
            }
        }

        return false;
    }

    private bool IsDraw()
    {
        foreach (int state in boardState)
        {
            if (state == 0)
                return false;
        }
        return true;
    }

    private void ShowResult(string resultMessage)
    {
        ResultText.text = resultMessage;
        ResultPopUp.SetActive(true);

        foreach (Button cellButton in CellButtons)
        {
            cellButton.interactable = false;
        }
    }

    private void UpdateScore()
    {
        Score_Player.text = $"{scorePlayer}";
        Score_Bot.text = $"{scoreBot}";
    }

    public void ResetGame()
    {
        for (int i = 0; i < boardState.Length; i++)
        {
            boardState[i] = 0;
            for (int j = 0; j < CellButtons[i].transform.childCount; j++)
            {
                CellButtons[i].transform.GetChild(j).gameObject.SetActive(false);
            }
            CellButtons[i].interactable = true;
        }

        turn = 0; // Player's turn
        ResultPopUp.SetActive(false);
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
