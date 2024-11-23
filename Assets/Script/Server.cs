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
using UnityEditor.Sprites;
using UnityEditor.PackageManager;

public class Server : MonoBehaviour
{
    private static int serverPort = 8080;
    private static UdpClient udpServer;
    
    private static Dictionary<int, IPEndPoint> connectedClients = new Dictionary<int, IPEndPoint>();
    //ConcurrentQueue<byte[]> sendQue = new ConcurrentQueue<byte[]>();
    ConcurrentQueue<IPacket> receiveQue = new ConcurrentQueue<IPacket>();
    int ClientNum = 0;
    int receiveClientNum = 0;
    float raceTime;
    IPEndPoint clientEndPoint;
    // ���� ��ųʸ� (recrive que ����)

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            udpServer = new UdpClient(8080);
            udpServer.Client.Blocking = false; // ����� (���ŷ ���): ������ �����͸� ���� ������ ��ٸ���, �����Ͱ� ���� ������ ���α׷��� ����.
                                               //�񵿱��(����ŷ ���): ������ �����͸� ��ٸ��� �ʰ� �ٷ� ��ȯ�Ǹ�, �����Ͱ� ���� ������ �۾��� ���� ó��.
            udpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); //�߰�
            clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

            StartCoroutine(TempThread());
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
        raceTime += Time.deltaTime;
    }


    void setraceTime()
    {
        // ��Ŷ ���� ������
        
    }

    IEnumerator TempThread()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.02f);
            flush();
        }
    }

    //����->Ŭ��(broadcast, ���� �ִ� ���� ����)
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

    // ��¥ broadcast 
    void BroadCast(byte[] packet)
    {
        foreach (var client in connectedClients)
        {
           udpServer.Send(packet, packet.Length, client.Value);
        }
    }
    
    // �ش� Client Num���� send
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
            if (packet != null)
            {
                receiveQue.Enqueue(packet); // ��Ŷ que�� �ֱ�
            }

            //if (packet == null)
        }
    }

    void flush()
    {
        for (int i = receiveQue.Count; i > 0; i--) 
        {
            IPacket packet = null;
            receiveQue.TryDequeue(out packet);
            byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet));
            BroadCast(buff);
        }
        // setraceTime();
    }

    //void send()

    void Process(ref IPacket packet)
    {
        //Debug.Log("Receive Packet");
        if (!connectedClients.ContainsValue(clientEndPoint)) // ó�� ����
        {
            Debug.Log(connectedClients.ContainsValue(clientEndPoint));
            AddPlayerPacket pac = addPlayer();
            // ó�� ���� Ŭ�� ��ȣ �Ѱ��ֱ�
            EventPacket eventPacket = new EventPacket();
            eventPacket.clientNum = pac.ClientNum;
            eventPacket.eventType = EventType.JOIN_GAME;
            byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventPacket));
            UniCast(buff, eventPacket.clientNum);

            
            receiveQue.Enqueue(pac);
            packet = null;

            return;
        }

        else // ó�� ������ �ƴҶ�
        {
            if (packet.Type == PacketType.PLAYER)
            {
                PlayerPacket pac = packet as PlayerPacket;
                pac.SetPosition(pac.GetPosition2Vec3());
                packet = (IPacket)pac;
            }

            else if (packet.Type == PacketType.EVENT)
            {
                EventPacket pac = packet as EventPacket;
                switch(pac.eventType)
                {
                    
                }
            }
        }
    }
   
    AddPlayerPacket addPlayer()
    {
        AddPlayerPacket pac = new AddPlayerPacket();
        pac.eventType = EventType.ADD_PLAYER;
        pac.ClientNum = ClientNum; // 1
        // �迭 �� ������ if ��ųʸ��� �� 1�̶� ������ && ó�� �����̸�
        // ���� �Դٴ� ���� broadcast�Ұ� �ִٴ� ��

        
       pac.ClientNums = new int[connectedClients.Count + 1]; // ���� �������� Ŭ��� + 1, +1�� ���� ���������� �� �ڸ� 0 1 2
       //Debug.Log("connected count + 1 : " + (connectedClients.Count + 1));
       for (int i = 0; i <= ClientNum; i++) // 1
       {
            pac.ClientNums[i] = i;
       }



       // ��ġ�� Ŭ�󿡼� ���� ����
       connectedClients.Add(ClientNum, clientEndPoint);  // Ŭ�� ��ȣ ����
       ClientNum++;

       return pac;
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
    [ContextMenu("DeBug/BroadcastAddPlayer")]
    void DebugBroadCast()
    {
        AddPlayerPacket pac = addPlayer();
        // ó�� ���� Ŭ�� ��ȣ �Ѱ��ֱ�
        EventPacket eventPacket = new EventPacket();
        eventPacket.clientNum = pac.ClientNum;
        eventPacket.eventType = EventType.JOIN_GAME;
        byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventPacket));
        
        UniCast(buff, eventPacket.clientNum);

        receiveQue.Enqueue(pac);
        // broadcast �� ��Ŷ���� enque
        return;
    }
}
