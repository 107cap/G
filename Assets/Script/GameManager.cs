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

    NetworkManager networkManager;

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

    // Update is called once per frame
    void Update()
    {
        if (player != null)
            networkManager.sendQue.Enqueue(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(player.playerPacket.GetValue())));
    }
}
