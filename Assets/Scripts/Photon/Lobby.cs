using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Lobby : MonoBehaviourPunCallbacks
{
    public static Lobby GameLobby;
    public GameObject BeginButton;
    public GameObject CancelButton;

    // Start is called before the first frame update
    private void Awake()
    {
        GameLobby = this;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected.");
        PhotonNetwork.AutomaticallySyncScene = true;
        BeginButton.SetActive(true);

    }

    public void OnBeginButtonClick()
    {
        PhotonNetwork.JoinRandomRoom();
        BeginButton.SetActive(false);
        CancelButton.SetActive(true);
    }

    public void OnCancelButtonClick()
    {
        CancelButton.SetActive(false);
        BeginButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room.");
        CreateRoom();
    }

    private void CreateRoom()
    {
        int random_room_name = Random.Range(0, 10000);
        RoomOptions room_ops = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers =  (byte)MultiplayerSettings.Settings.MaxPlayers};
        PhotonNetwork.CreateRoom("Room_" + random_room_name, room_ops);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room.");
        CreateRoom();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
