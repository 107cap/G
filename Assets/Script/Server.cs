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
    // 서버 딕셔너리 (recrive que 삭제)
    private void Awake()
    {
        

    }
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            udpServer = new UdpClient(serverPort);
            udpServer.Client.Blocking = false; // 동기식 (블로킹 모드): 소켓이 데이터를 받을 때까지 기다리고, 데이터가 오지 않으면 프로그램이 멈춤.
                                               //비동기식(비블로킹 모드): 소켓이 데이터를 기다리지 않고 바로 반환되며, 데이터가 오면 별도의 작업을 통해 처리.
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

    //서버->클라(broadcast)
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

    //클라->서버 받기
    void Receive()
    {
        if (udpServer.Available > 0)
        {
            byte[] receivedBuff = udpServer.Receive(ref clientEndPoint);
            if (!connectedClients.ContainsKey(clientEndPoint))
            {
                connectedClients.Add(clientEndPoint, ClientNum++);  // 클라 번호 저장
            }
            else
            {
                receiveQue.Enqueue(receivedBuff); // 이미 접속한 클라면 que에 넣기
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
        // 패킷 검증
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
