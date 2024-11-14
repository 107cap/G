using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;             // GameManager 싱글톤 선언

    public IPacket sendPacket;
    public IPacket receivePacket;

    [Header("Players")]
    public Player myPlayer;
    public List<Player> otherPlayers;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // otherPlayers 초기화
        otherPlayers = new List<Player>();
    }

    void Start()
    {
        // 다른 Player 탐색
        SearchOtherPlayer();
    }

    // 다른 Player 탐색
    void SearchOtherPlayer()
    {
        Player[] allPlayers = FindObjectsOfType<Player>();

        foreach (Player player in allPlayers)
        {
            if (player != myPlayer)
            {
                otherPlayers.Add(player);
            }
        }
    }

    // NetworkManager로 패킷 보내기
    public void SendPacket(string packetData)
    {

    }

    // NetworkManager한테서 패킷 받기
    public void ReceivePacket(string packetData)
    {
        // 예시) Packet을 전달해야하는 Player에게 Data 전달
        myPlayer.ReceivePacketValue(packetData);
    }
}
