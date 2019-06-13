using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSettings : MonoBehaviour
{

    public static MultiplayerSettings Settings;

    public int MaxPlayers;
    public int MenuScene;
    public int MultiplayerScene;

    private void Awake()
    {
        if (Settings == null)
        {
            Settings = this;
        }
        else
        {
            if (Settings != this)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
    }
}
