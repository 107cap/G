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
    float raceTime;
    IPEndPoint clientEndPoint;
    // 서버 딕셔너리 (recrive que 삭제)

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
        raceTime += Time.deltaTime;
    }


    void setraceTime()
    {
        // 패킷 만들어서 보내기
        
    }

    IEnumerator TempThread()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            flush();
        }
    }

    //서버->클라(broadcast, 보낸 애는 받지 않음)
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

    // 진짜 broadcast 
    void BroadCast(byte[] packet)
    {
        foreach (var client in connectedClients)
        {
           udpServer.Send(packet, packet.Length, client.Value);
        }
    }
    
    // 해당 Client Num에만 send
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
    

    //클라->서버 받기
    void Receive()
    {
        if (udpServer.Available > 0)
        {
            byte[] receivedBuff = udpServer.Receive(ref clientEndPoint);
            IPacket packet = JsonConvert.DeserializeObject<IPacket>(Encoding.UTF8.GetString(receivedBuff));
            // else로 처음 안왔을떄만 처리?
            Process(ref packet);
            if (packet != null)
            {
                receiveQue.Enqueue(packet); // 패킷 que에 넣기
            }

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

        setraceTime();


    }

    //void send()

    void Process(ref IPacket packet)
    {
        if (!connectedClients.ContainsValue(clientEndPoint)) // 처음 접속
        {
            AddPlayerPacket pac = addPlayer();
            // 처음 접속 클라 번호 넘겨주기
            EventPacket eventPacket = new EventPacket();
            eventPacket.clientNum = pac.ClientNum;
            eventPacket.eventType = EventType.JOIN_GAME;
            byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventPacket));
            UniCast(buff, eventPacket.clientNum);

            
            receiveQue.Enqueue(pac);
            packet = null;
            return;
        }

        else // 처음 접속이 아닐때
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
    // 클라에선 배열 있으면 만들어주고 없으면 안만듬. 이미 있어도 안만듬
   
    AddPlayerPacket addPlayer()
    {
        AddPlayerPacket pac = new AddPlayerPacket();
        pac.eventType = EventType.ADD_PLAYER;
        pac.ClientNum = ClientNum;
        // 배열 값 설정은 if 딕셔너리에 값 1이라도 있으면 && 처음 접속이면
        // 여기 왔다는 것은 broadcast할게 있다는 뜻

        
        pac.ClientNums = new int[connectedClients.Count + 1]; // 현재 접속중인 클라수 + 1, +1은 내가 마지막으로 들어갈 자리
        if (connectedClients.Count > 0)
        {
            for (int i = 0; i < ClientNum; i++)
            {
                pac.ClientNums[i] = i;
            }
        }

        // 위치는 클라에서 직접 지정
        connectedClients.Add(ClientNum, clientEndPoint);  // 클라 번호 저장
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
        // 처음 접속 클라 번호 넘겨주기
        EventPacket eventPacket = new EventPacket();
        eventPacket.clientNum = pac.ClientNum;
        eventPacket.eventType = EventType.JOIN_GAME;
        byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventPacket));
        
        UniCast(buff, eventPacket.clientNum);

        receiveQue.Enqueue(pac);
        // broadcast 용 패킷만들어서 enque
        return;
    }
}
