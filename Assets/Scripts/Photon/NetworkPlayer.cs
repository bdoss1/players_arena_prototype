using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    PhotonView PV;
    public GameObject ThePlayer;
    public static int CurrentPlayerCount = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        int spawn_jumper_rnd = Random.Range(0, MultiplayerGameSetup.GameSetup.SpawnJumpers.Length);

        GameObject spawn_jumper = MultiplayerGameSetup.GameSetup.SpawnJumpers[spawn_jumper_rnd];

        List<Transform> spawn_points = new List<Transform>();

        foreach(Transform child in spawn_jumper.gameObject.transform)
        {
            spawn_points.Add(child);
        }
        
        if (PV.IsMine)
        {
            ThePlayer = PhotonNetwork.Instantiate("Player", spawn_points[CurrentPlayerCount].position,
                spawn_points[CurrentPlayerCount].rotation, 0);

            CurrentPlayerCount++;

            // Only allow one camera for this player
            GameObject body = ThePlayer.transform.Find("PlayerBody").gameObject;
            GameObject eyes = body.transform.Find("PlayerEyes").gameObject;
            eyes.SetActive(true);
            
        }

    }
}
