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

public class Server : MonoBehaviour
{
    private static int serverPort = 8080;
    private static UdpClient udpServer;
    
    private static Dictionary<IPEndPoint, int> connectedClients = new Dictionary<IPEndPoint, int>();
    ConcurrentQueue<byte[]> sendQue = new ConcurrentQueue<byte[]>();
    ConcurrentQueue<byte[]> receiveQue = new ConcurrentQueue<byte[]>();
    ConcurrentQueue<IPEndPoint> receiveQue_Sender = new ConcurrentQueue<IPEndPoint>();
    int ClientNum = 0;
    DateTime raceTime;

    private void Awake()
    {
        udpServer = new UdpClient(serverPort);
        StartCoroutine(TempThread());
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
    void BroadCast(byte[] packet, IPEndPoint senderEndPoint)
    {
        foreach (var clinet in connectedClients)
        {
            if (!clinet.Key.Equals(senderEndPoint))
            {
                udpServer.Send(packet, packet.Length, clinet.Key);
            }
        }
    }

    //Ŭ��->���� �ޱ�
    void Receive()
    {
        IPEndPoint clientEndPoint = null;
        byte[] receivedBuff = udpServer.Receive(ref clientEndPoint);
        if (!connectedClients.ContainsKey(clientEndPoint))
        {
            connectedClients.Add(clientEndPoint, ClientNum++);  // Ŭ�� ��ȣ ����
        }
        else
        {
            receiveQue.Enqueue(receivedBuff); // �̹� ������ Ŭ��� que�� �ֱ�
            receiveQue_Sender.Enqueue(clientEndPoint);
        }
    }

    void flush()
    {
        for (int i = receiveQue.Count; i>0; i--)
        {
            byte[] buff = null;
            IPEndPoint senderEndPoint = null;
            receiveQue.TryDequeue(out buff);
            receiveQue_Sender.TryDequeue(out senderEndPoint);
            BroadCast(buff, senderEndPoint);
        }
    }

    void Process()
    {
        // ��Ŷ ����
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
