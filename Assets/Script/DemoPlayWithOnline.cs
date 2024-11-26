// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using Photon.Pun;
// using Photon.Realtime;

// public class Demo : MonoBehaviourPunCallbacks
// {
//     public static Demo Instance { get; private set; }

//     [Header("Room Code Panel")]
//     [SerializeField] private GameObject EnterRoomCodePanel, RoomCodeTextPanel;
//     //jab user room create karega ya join krega to EnterRoomcodepanel inactive hoga or roomcodetextpanel active hoga or roomcodetext or statustext update hoga 
//     [SerializeField] private TMP_InputField RoomCodeInputField;
//     [SerializeField] private Button JoinButton, CreateButton;
//     [SerializeField] private TMP_Text RoomCodeText, StatusText;   // these texts are used when room created

//     [Header("Game UI")]
//     //Names should be shown for both the players
//     [SerializeField] private TMP_Text YourName, OpponentsName, Status;   //This Status is used when game start
//     [SerializeField] private Button ResetButton, ExitButton;
//     [SerializeField] private Button[] CellButtons;

//     [Header("Result UI")]
//     [SerializeField] private GameObject ResultPopUp;
//     [SerializeField] private TMP_Text ResultText;
//     [SerializeField] private Button CancelButton, PlayAgainButton;

//     private int turn = 0; // 0 for Player X, 1 for Player O     //Used for switching turns
//     private int[] boardState = new int[9];
//     private bool isPlayerTurn = false;
//     private bool isRoomCreator = false;


// }