using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool? isVictory = null;
    DateTime raceTime;

    NetworkManager networkManager = new NetworkManager();

    #region Singleton
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField] PlayerMove player;

    void Update()
    {
        if (player != null)
        {
            networkManager.sendQue.Enqueue(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(player.playerPacket.GetValue())));
            networkManager.Send();
        }
    }
}
