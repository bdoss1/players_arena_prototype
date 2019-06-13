using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGameSetup : MonoBehaviour
{
    public static MultiplayerGameSetup GameSetup;

    public GameObject[] SpawnJumpers;

    private void Awake()
    {
        if (GameSetup == null)
        {
            GameSetup = this;
        }
    }
}
