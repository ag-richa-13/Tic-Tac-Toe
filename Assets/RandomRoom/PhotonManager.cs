using UnityEngine;
using UnityEngine.UI; // For UI
using Photon.Pun; // Photon namespace
using Photon.Realtime; // For Room and Player events
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public TMP_Text statusText; // Reference to the status Text
    public Button joinRoomButton; // Reference to the Join Room button

    void Start()
    {
        // Disable the button initially and connect to Photon
        joinRoomButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
        UpdateStatus("Connecting to Photon...");
        joinRoomButton.onClick.AddListener(JoinRandomRoom);
    }

    public void JoinRandomRoom()
    {
        // Check connection and attempt to join a room
        if (PhotonNetwork.IsConnected)
        {
            UpdateStatus("Joining a random room...");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            UpdateStatus("Not connected to Photon!");
        }
    }

    public override void OnConnectedToMaster()
    {
        // Enable the button when connected
        joinRoomButton.interactable = true;
        UpdateStatus("Connected to Photon. Ready to join a room.");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // Handle the case when no rooms are available
        UpdateStatus("No rooms found, creating a new room...");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {
        // Confirm successful room join
        UpdateStatus($"Joined a room! Players: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Update when a new player joins the room
        UpdateStatus($"A new player joined! Total players: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }

    private void UpdateStatus(string message)
    {
        // Update the status Text UI
        if (statusText != null)
        {
            statusText.text = $"Status: {message}";
        }
    }
}
