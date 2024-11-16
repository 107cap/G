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
            yield return new WaitForSecondsRealtime(0.2f);
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

    //Ŭ��->���� �ޱ�
    void Receive()
    {
        if (udpServer.Available > 0)
        {
            byte[] receivedBuff = udpServer.Receive(ref clientEndPoint);
            IPacket packet = JsonConvert.DeserializeObject<IPacket>(Encoding.UTF8.GetString(receivedBuff));
            if (!connectedClients.ContainsValue(clientEndPoint))
            {
                //ó�� ���� ó��
                connectedClients.Add(ClientNum, clientEndPoint);  // Ŭ�� ��ȣ ����
            }  
            // else�� ó�� �ȿ������� ó��?
            else
            {
                receiveQue.Enqueue(packet); // ��Ŷ que�� �ֱ�
                //PlayerPacket pac = packet as PlayerPacket;
                //Debug.Log(pac.GetPosition2Vec3());

            }

            

        }
        
    }

    void flush()
    {
        while(receiveQue.Count > 0 ) 
        {
            IPacket packet = null;
            receiveQue.TryDequeue(out packet);
            //PlayerPacket pac = packet as PlayerPacket;
            //Debug.Log(pac.GetPosition2Vec3());
            //Debug.Log(Encoding.UTF8.GetString(buff));
            int clientnum = Process(ref packet);
            PlayerPacket pac2 = packet as PlayerPacket;
            //Debug.Log(pac2.GetPosition() + "������ ���� ��Ŷ ��ǥ");
            byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet));
            BroadCast(buff, clientnum);
        }
    }

    int Process(ref IPacket packet)
    {
        // ��Ŷ ó��
        if (packet.type == PacketType.PLAYER)
        {
            PlayerPacket pac = packet as PlayerPacket;
            //Debug.Log((pac.GetPosition2Vec3()));
            pac.SetPosition(pac.GetPosition2Vec3() + new Vector3(0.0001f,0.0f,0.0f));
            packet = (IPacket)pac;
        }

        // �������� �°��� Ȯ��
        return 655555; // return clientnum
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
