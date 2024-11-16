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
    ConcurrentQueue<byte[]> receiveQue = new ConcurrentQueue<byte[]>();
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
            if (!connectedClients.ContainsValue(clientEndPoint))
            {
                //ó�� ���� ó��
                connectedClients.Add(ClientNum, clientEndPoint);  // Ŭ�� ��ȣ ����
            }  
            // else�� ó�� �ȿ������� ó��?
            else
            {
                receiveQue.Enqueue(receivedBuff); // ��Ŷ que�� �ֱ�
            }

            

        }
        
    }

    void flush()
    {
        while(receiveQue.Count > 0 ) 
        {
            byte[] buff = null;
            receiveQue.TryDequeue(out buff);
            Debug.Log(Encoding.UTF8.GetString(buff));
            int clientnum = Process(buff);
            BroadCast(buff, clientnum);
        }
    }

    int Process(byte[] buff)
    {
        // ��Ŷ ó��
        // �������� �°��� Ȯ��
        return 0; // return clientnum
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
