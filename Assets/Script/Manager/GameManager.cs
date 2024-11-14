using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;             // GameManager �̱��� ����

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

        // otherPlayers �ʱ�ȭ
        otherPlayers = new List<Player>();
    }

    void Start()
    {
        // �ٸ� Player Ž��
        SearchOtherPlayer();
    }

    // �ٸ� Player Ž��
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

    // NetworkManager�� ��Ŷ ������
    public void SendPacket(string packetData)
    {

    }

    // NetworkManager���׼� ��Ŷ �ޱ�
    public void ReceivePacket(string packetData)
    {
        // ����) Packet�� �����ؾ��ϴ� Player���� Data ����
        myPlayer.ReceivePacketValue(packetData);
    }
}
