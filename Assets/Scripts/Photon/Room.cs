using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static Room MultiplayerRoom;

    private PhotonView PV;
    public bool IsGameLoaded;
    public int CurrentScene;

    private Player[] AllPlayers;
    public int PlayersInRoom;
    public int NumberInRoom;
    public int PlayersInGame;

    private bool ReadyToCount;
    private bool ReadyToStart;
    public float StartingTime;
    private float LessThanMaxPlayers;
    public float MaxPlayerCountDownSeconds = 1;
    private float TimeToStart;

    // Start is called before the first frame update
    void Awake()
    {
        if(MultiplayerRoom == null)
        {
            MultiplayerRoom = this;
        }
        else
        {
            if (MultiplayerRoom != this)
            {
                Destroy(MultiplayerRoom.gameObject);
                MultiplayerRoom = this;
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoaded;
    }

    void Start()
    {
        PV = GetComponent<PhotonView>();
        ReadyToCount = false;
        ReadyToStart = false;
        LessThanMaxPlayers = StartingTime;
        TimeToStart = StartingTime;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Room Joined");
        AllPlayers = PhotonNetwork.PlayerList;
        PlayersInRoom = AllPlayers.Length;
        NumberInRoom = PlayersInRoom;
        PhotonNetwork.NickName = NumberInRoom.ToString();

        if (PlayersInRoom > 1)
        {
            ReadyToCount = true;
        }

        if(PlayersInRoom == MultiplayerSettings.Settings.MaxPlayers)
        {
            ReadyToStart = true;

            if (!PhotonNetwork.IsMasterClient) return;

            PhotonNetwork.CurrentRoom.IsOpen = false;

        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("New Player added.");
        AllPlayers = PhotonNetwork.PlayerList;
        PlayersInRoom++;

        if (PlayersInRoom > 1)
        {
            ReadyToCount = true;
        }

        if(PlayersInRoom == MultiplayerSettings.Settings.MaxPlayers)
        {
            ReadyToStart = true;

            if (!PhotonNetwork.IsMasterClient) return;

            PhotonNetwork.CurrentRoom.IsOpen = false;

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (PlayersInRoom < MultiplayerSettings.Settings.MaxPlayers)
        {
            RestartTimer();
        }

        if (!IsGameLoaded)
        {
            if (ReadyToStart)
            {
                MaxPlayerCountDownSeconds -= Time.deltaTime;
                LessThanMaxPlayers = MaxPlayerCountDownSeconds;
                TimeToStart = MaxPlayerCountDownSeconds;
            }
            else if (ReadyToCount)
            {
                LessThanMaxPlayers -= Time.deltaTime;
                TimeToStart = LessThanMaxPlayers;
            }

            if (TimeToStart <= 0)
            {
                StartGame();
            }
        }
    }

    void StartGame()
    {
        IsGameLoaded = true;

        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(MultiplayerSettings.Settings.MultiplayerScene);

    }

    void RestartTimer()
    {
        LessThanMaxPlayers = StartingTime;
        TimeToStart = StartingTime;
        MaxPlayerCountDownSeconds = 6;
        ReadyToCount = false;
        ReadyToStart = false;
    }

    void OnSceneFinishedLoaded(Scene scene, LoadSceneMode mode)
    {
        CurrentScene = scene.buildIndex;

        if (CurrentScene == MultiplayerSettings.Settings.MultiplayerScene)
        {
            IsGameLoaded = true;
            PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        PlayersInGame++;

        if (PlayersInGame == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("NetworkPrefabs", "NetworkPlayer"), transform.position, Quaternion.identity, 0);
    }
}
