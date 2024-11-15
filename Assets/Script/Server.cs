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
    
    private static Dictionary<IPEndPoint, int> connectedClients = new Dictionary<IPEndPoint, int>();
    ConcurrentQueue<byte[]> sendQue = new ConcurrentQueue<byte[]>();
    ConcurrentQueue<byte[]> receiveQue = new ConcurrentQueue<byte[]>();
    ConcurrentQueue<IPEndPoint> receiveQue_Sender = new ConcurrentQueue<IPEndPoint>();
    int ClientNum = 0;
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
        if (udpServer.Available > 0)
        {
            byte[] receivedBuff = udpServer.Receive(ref clientEndPoint);
            if (!connectedClients.ContainsKey(clientEndPoint))
            {
                connectedClients.Add(clientEndPoint, ClientNum++);  // Ŭ�� ��ȣ ����
            }
            else
            {
                receiveQue.Enqueue(receivedBuff); // �̹� ������ Ŭ��� que�� �ֱ�
                receiveQue_Sender.Enqueue(clientEndPoint); //need fix
            }
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

            Debug.Log(Encoding.UTF8.GetString(buff));
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
