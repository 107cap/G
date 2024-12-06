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
    ConcurrentQueue<IPacket> sendQue = new ConcurrentQueue<IPacket>();
    ConcurrentQueue<IPacket> receiveQue = new ConcurrentQueue<IPacket>();
    private static Dictionary<int, string> clientsName = new Dictionary<int, string>();
    int ClientNum = 0;
    int receiveClientNum = 0;
    float raceTime;
    IPEndPoint clientEndPoint;

    [SerializeField]
    int maxClientNum = 0;

    bool[] isreadyPlayers;
    // 서버 딕셔너리 (recrive que 삭제)

    #region Singleton
    public static Server Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,  //직렬화 설정
        };
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            udpServer = new UdpClient(8080);
            udpServer.Client.Blocking = false; // 동기식 (블로킹 모드): 소켓이 데이터를 받을 때까지 기다리고, 데이터가 오지 않으면 프로그램이 멈춤.
                                               //비동기식(비블로킹 모드): 소켓이 데이터를 기다리지 않고 바로 반환되며, 데이터가 오면 별도의 작업을 통해 처리.
            udpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); //추가
            clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            isreadyPlayers = new bool[maxClientNum];
            initReadyArray();
            StartCoroutine(TempThread());
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
       
    }


    // Update is called once per frame
   void initReadyArray()
    {
        for (int i = 0; i < maxClientNum; i++)
            isreadyPlayers[i] = false;
    }


    void setraceTime()
    {
        // 패킷 만들어서 보내기
    }

    IEnumerator TempThread()
    {
        while (true)
        {
            Receive();
            flush();
            yield return new WaitForSecondsRealtime(0.02f);
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
            try
            {
                udpServer.Send(packet, packet.Length, client.Value);
            }
            catch (Exception e)
            {

                Debug.Log(e);
            }
        }
    }
    
    // 해당 Client Num에만 send
    void UniCast(byte[] packet, int ClientNum)
    {
        //Debug.Log("Unicast 시작");
        foreach(var client in connectedClients)
        {
            try
            {
                if (client.Key.Equals(ClientNum))
                {
                    //Debug.Log("Unicast 찾음");
                    udpServer.Send(packet, packet.Length, client.Value);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            

            
        }
    }
    

    //클라->서버 받기
    void Receive()
    {
        //Debug.Log("Receive 시작");
        if (udpServer.Available > 0)
        {
            byte[] receivedBuff = udpServer.Receive(ref clientEndPoint);
            //ebug.Log("Receive 처리");
            IPacket packet = JsonConvert.DeserializeObject<IPacket>(Encoding.UTF8.GetString(receivedBuff));
            // else로 처음 안왔을떄만 처리?
            receiveQue.Enqueue(packet);

            Process();


        }

    }

    void Process()
    {
        IPacket packet = null;
        receiveQue.TryDequeue(out packet);
        //Debug.Log("Receive Packet");
        if (!connectedClients.ContainsValue(clientEndPoint)) // 처음 접속
        {
            //AddPlayerPacket playerpac = packet as AddPlayerPacket;
            AddPlayerPacket pac = addPlayer(packet);
            //Debug.Log("@@@");
            // 처음 접속 클라 번호 넘겨주기
            EventPacket eventPacket = new EventPacket();
            eventPacket.clientNum = pac.ClientNum;
            eventPacket.eventType = EventType.JOIN_GAME;
            byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventPacket));
            UniCast(buff, eventPacket.clientNum);


            sendQue.Enqueue(pac);
            packet = null;

            return;
        }

        else // 처음 접속이 아닐때
        {
            if (packet.Type == PacketType.PLAYER)
            {
                PlayerPacket pac = packet as PlayerPacket;
                pac.SetPosition(pac.GetPosition2Vec3());
                if (pac.clientNum == 1 || pac.clientNum == 0)
                {
                    //Debug.Log("Client Num = " + pac.clientNum);
                    pac.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }
                packet = (IPacket)pac;
            }

            else if (packet.Type == PacketType.EVENT)
            {
                EventPacket pac = packet as EventPacket;
                switch (pac.eventType)
                {
                    case EventType.NONE:
                        //Debug.Log("!!!");
                        break;
                }
            }

            else if (packet.Type == PacketType.READY)
            {
                ReadyPacket pac = packet as ReadyPacket;
                Debug.Log("server cLIENT nUM" + pac.ClientNum);
                isreadyPlayers[pac.clientNum] = pac.isReady;

                if (checkAllReady())
                {
                    EventPacket startracePacket = new EventPacket();
                    startracePacket.eventType = EventType.START_RACE;
                    packet = (IPacket)startracePacket;
                }

                else
                    return;
            }
        }
        if (packet != null)
        {
            sendQue.Enqueue(packet); // 패킷 que에 넣기
        }
    }

    bool checkAllReady()
    {
        for (int i = 0; i<maxClientNum; i++)
        {
            if (isreadyPlayers[i] == false)
                return false;
        }
        return true;
    }

    void flush()
    {
        //Debug.Log("Server Packet : " + receiveQue.Count);
        for (int i = sendQue.Count; i > 0; i--) 
        {
            IPacket packet = null;
            sendQue.TryDequeue(out packet);
            byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet));
            BroadCast(buff);
        }
        // setraceTime();
    }

    AddPlayerPacket addPlayer(IPacket packet)
    {
        AddPlayerPacket addplayerPac = packet as AddPlayerPacket;
        // addplayerPac.eventType = EventType.ADD_PLAYER;
        addplayerPac.ClientNum = ClientNum; // 1
                                            // 배열 값 설정은 if 딕셔너리에 값 1이라도 있으면 && 처음 접속이면
                                            // 여기 왔다는 것은 broadcast할게 있다는 뜻
        clientsName.Add(ClientNum, addplayerPac.nickName);
        addplayerPac.ClientNums = new int[connectedClients.Count + 1]; // 현재 접속중인 클라수 + 1, +1은 내가 마지막으로 들어갈 자리 0 1 2
        addplayerPac.ClientNames = new string[connectedClients.Count + 1];
       //Debug.Log("connected count + 1 : " + (connectedClients.Count + 1));
       for (int i = 0; i <= ClientNum; i++) // 1
       {
            addplayerPac.ClientNums[i] = i;
            if (!clientsName.TryGetValue(i, out addplayerPac.ClientNames[i]))
            {
                Debug.Log($"{i} Fail tryget clientnames");
            }

            else
            {
                Debug.Log($"server client num :[{i}], {addplayerPac.ClientNames[i]}");
            }
       }

       // 위치는 클라에서 직접 지정
       connectedClients.Add(ClientNum, clientEndPoint);  // 클라 번호 저장
       ClientNum++;

       return addplayerPac;
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
