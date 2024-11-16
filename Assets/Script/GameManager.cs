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
            TypeNameHandling = TypeNameHandling.All,  // Ÿ�� ���� ����
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

            //���� �� -> �÷��̾� �� ����
            player.PlayerUpdate();

            //�̵��� �÷��̾ �ޱ�

            //������ �ش� �� ����


            networkManager.sendQue.Enqueue(player.playerPacket);
            networkManager.Send();
        }
    }
}
