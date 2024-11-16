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

    public NetworkManager networkManager = new NetworkManager();

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

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,  // 타입 정보 포함
        };
    }
    #endregion

    [SerializeField] PlayerMove player;

    void Update()
    {
        if (player != null)
        {
            Debug.Log(player.playerPacket.GetPosition2Vec3());
            networkManager.Receive();

            //받은 값 -> 플레이어 값 적용
            player.PlayerUpdate();

            //이동된 플레이어값 받기

            //서버로 해당 값 전달


            networkManager.sendQue.Enqueue(player.playerPacket);
            networkManager.Send();
        }
    }
}
