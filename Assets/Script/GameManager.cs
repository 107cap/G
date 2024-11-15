using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool? isVictory = null;
    DateTime raceTime;

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
        if(player != null)
            Debug.Log(player.playerPacket.GetValue());
    }
}
