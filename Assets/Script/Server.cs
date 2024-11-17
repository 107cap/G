using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json;

public class Server : MonoBehaviour
{
    private static int serverPort = 8080;
    private static UdpClient udpServer;
    
    private static Dictionary<int, IPEndPoint> connectedClients = new Dictionary<int, IPEndPoint>();
    //ConcurrentQueue<byte[]> sendQue = new ConcurrentQueue<byte[]>();
    ConcurrentQueue<IPacket> receiveQue = new ConcurrentQueue<IPacket>();
    int ClientNum = 0;
    int receiveClientNum = 0;
    DateTime raceTime;
    IPEndPoint clientEndPoint;
    // ���� ��ųʸ� (recrive que ����)
    private void Awake()
    {
        

    }
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            udpServer = new UdpClient(serverPort);
            udpServer.Client.Blocking = false; // ����� (���ŷ ���): ������ �����͸� ���� ������ ��ٸ���, �����Ͱ� ���� ������ ���α׷��� ����.
                                               //�񵿱��(����ŷ ���): ������ �����͸� ��ٸ��� �ʰ� �ٷ� ��ȯ�Ǹ�, �����Ͱ� ���� ������ �۾��� ���� ó��.
            StartCoroutine(TempThread());
            clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        Receive();
        
    }

    IEnumerator TempThread()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            flush();
        }
    }

    //����->Ŭ��(broadcast)
    void BroadCast(byte[] packet, int senderClientNum)
    {
        foreach (var client in connectedClients)
        {
            if (!client.Key.Equals(senderClientNum))
            {
                udpServer.Send(packet, packet.Length, client.Value);
            }
        }
    }

    void UniCast(byte[] packet, int ClientNum)
    {
        foreach(var client in connectedClients)
        {
            if (client.Key.Equals(ClientNum))
            {
                udpServer.Send(packet, packet.Length, client.Value);
            }
        }
    }

    //Ŭ��->���� �ޱ�
    void Receive()
    {
        if (udpServer.Available > 0)
        {
            byte[] receivedBuff = udpServer.Receive(ref clientEndPoint);
            IPacket packet = JsonConvert.DeserializeObject<IPacket>(Encoding.UTF8.GetString(receivedBuff));
            // else�� ó�� �ȿ������� ó��?
            Process(ref packet);
            receiveQue.Enqueue(packet); // ��Ŷ que�� �ֱ�

        }
        
    }

    void flush()
    {
        for (int i = receiveQue.Count; i > 0; i--) 
        {
            IPacket packet = null;
            receiveQue.TryDequeue(out packet);
            byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet));
            BroadCast(buff, packet.ClientNum);
        }
    }

    //void send()

    void Process(ref IPacket packet)
    {
        if (!connectedClients.ContainsValue(clientEndPoint)) // ó�� ����
        {
            EventPacket evePacket = new EventPacket();
            EventPacket pac = packet as EventPacket;

            addPlayer(ref packet);
        }

        else // ó�� ������ �ƴҶ�
        {
            if (packet.Type == PacketType.PLAYER)
            {
                PlayerPacket pac = packet as PlayerPacket;
                pac.SetPosition(pac.GetPosition2Vec3());
                packet = (IPacket)pac;
            }
        }
    }

    // ó�� ���� ó��
    void addPlayer(ref IPacket packet)
    {
        AddPlayerPacket pac = packet as AddPlayerPacket;
        pac.eventType = EventType.ADD_PLAYER;
        pac.ClientNum = ClientNum;
        // �迭 �� ������ if ��ųʸ��� �� 1�̶� ������ && ó�� �����̸�
        if (connectedClients.Count > 0)
        {
            for (int i = 0; i < ClientNum; i++)
            {
                pac.ClientNums[i] = i;
            }
        }

        // ��ġ�� Ŭ�󿡼� ���� ����
        ClientNum++;
        packet = (IPacket)pac;
    }

    void EndServer()
    {
        udpServer.Close();
    }

    bool CheckConnect()
    {
        return true;
    }

    void Connect()
    {

    }

    void UpdateRaceTime()
    {

    }
}
