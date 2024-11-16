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

    //클라->서버 받기
    void Receive()
    {
        if (udpServer.Available > 0)
        {
            byte[] receivedBuff = udpServer.Receive(ref clientEndPoint);
            IPacket packet = JsonConvert.DeserializeObject<IPacket>(Encoding.UTF8.GetString(receivedBuff));
            if (!connectedClients.ContainsValue(clientEndPoint))
            {
                //처음 접속 처리
                connectedClients.Add(ClientNum, clientEndPoint);  // 클라 번호 저장
            }  
            // else로 처음 안왔을떄만 처리?
            else
            {
                receiveQue.Enqueue(packet); // 패킷 que에 넣기
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
            //Debug.Log(pac2.GetPosition() + "서버가 받은 패킷 좌표");
            byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet));
            BroadCast(buff, clientnum);
        }
    }

    int Process(ref IPacket packet)
    {
        // 패킷 처리
        if (packet.type == PacketType.PLAYER)
        {
            PlayerPacket pac = packet as PlayerPacket;
            //Debug.Log((pac.GetPosition2Vec3()));
            pac.SetPosition(pac.GetPosition2Vec3() + new Vector3(0.0001f,0.0f,0.0f));
            packet = (IPacket)pac;
        }

        // 누구한테 온건지 확인
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
