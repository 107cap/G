using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerCamera playerCamera;

    int selfClientNum = -1;
    bool? isVictory = null;
    DateTime raceTime;

    public NetworkManager networkManager = new NetworkManager();
    public EventManager eventManager;
    
    //패킷 캐싱
    IPacket tmpPacket;
    EventPacket eventPacket;
    PlayerPacket playerPacket;
    AddPlayerPacket testAddPacket;
    //AddPlayerPacket addPlayerPacket;

    #region Singleton
    public static GameManager Instance;
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
            TypeNameHandling = TypeNameHandling.All,  // Ÿ�� ���� ����
        };
    }
    #endregion

    private void Start()
    {
        eventManager.Register(EventType.ADD_PLAYER, () => { AddPlayers(); Debug.Log("AddPlayer 받았다 데스"); });
        eventManager.Register(EventType.JOIN_GAME, () => { SetSelfClientNum(); Debug.Log("Join Game받았다 데스네"); }); 

        RequestJoin();
    }

    public GameObject playerPrefab;
    Dictionary<int, PlayerMove> playerDict = new Dictionary<int, PlayerMove>();
    Vector3[] sponPositions = new Vector3[] {
        new Vector3(0f, 2f, 0f),
        new Vector3(10f, 2f, 0f),
        new Vector3(20f, 2f, 0f),
        new Vector3(30f, 2f, 0f)
    };

    void Update()
    {
        //Receive
        networkManager.Receive();

        #region Process

        networkManager.receiveQue.TryDequeue(out tmpPacket);

        //AddPlayerPacket pac = tmpPacket as AddPlayerPacket;

        if (testAddPacket != null)
        {
            testAddPacket = (AddPlayerPacket)tmpPacket;
            foreach (var item in testAddPacket.ClientNums)
            {
                Debug.Log("오아시스 : " + item);
            }
        }
        if (tmpPacket != null)
        {
            //Debug.Log(tmpPacket.Type);
            switch (tmpPacket.Type)
            {
                case PacketType.NONE:
                    break;
                case PacketType.PLAYER:
                    if (!playerDict.Count.Equals(0))
                    {
                        playerPacket = (PlayerPacket)tmpPacket;

                        //Update other Player Position, Not send Updateed other Position
                        if(!playerPacket.clientNum.Equals(selfClientNum))
                            playerDict[playerPacket.ClientNum].OtherPlayerUpdate(playerPacket);
                    }
                    break;
                case PacketType.EVENT:
                    //Debug.Log("스위치 작동");
                    eventPacket = (EventPacket)tmpPacket;
                    eventManager.Invoke(eventPacket.eventType);
                    break;
                case PacketType.ERROR:
                    break;
                default:
                    break;
            }
        }
        #endregion

        //Send
        //본인 클라이언트의 좌표값은 항상 전송
        if(playerDict.ContainsKey(selfClientNum))    //본인 클라이언트가 존재 시
            networkManager.sendQue.Enqueue( 
                playerDict[selfClientNum].SelfPlayerUpdate(playerPacket));

        networkManager.Send();

        tmpPacket = null;
    }

    void SetSelfClientNum() => selfClientNum = eventPacket.clientNum;
    public int GetSelfClientNum() => selfClientNum;

    /// <summary>
    /// Add Players In PlayScene
    /// </summary>
    /// <param name="clientNums"></param>
    void AddPlayers()
    {
        AddPlayerPacket addPlayerPacket = (AddPlayerPacket)eventPacket;

        foreach (var item in addPlayerPacket.ClientNums)
        {
            Debug.Log("ADDPlayer.ClientNums : " + item);
        }

        for (int i = 0; i < addPlayerPacket.ClientNums.Length; i++)
        {
            Debug.Log("create 전 num값 : " + addPlayerPacket.ClientNums[i]);
            CreatePlayer(addPlayerPacket.ClientNums[i], sponPositions[i]);
        }
    }
    
    void CreatePlayer(int clientNum, Vector3 position)
    {
        Debug.Log("createPlayer.num : " + clientNum);

        //Early return;
        if (playerDict.ContainsKey(clientNum))
            return;

        //Create Player And Add Dict
        Debug.Log("작동 createPlayer");
        GameObject player = Instantiate(playerPrefab);
        player.transform.position = position;
        playerDict.Add(clientNum, player.GetComponent<PlayerMove>());

        if (clientNum == selfClientNum)
        {
            if (playerCamera != null)
            {
                playerCamera.SetPlayer(player);
            }
        }
    }

    [ContextMenu("DeBug/RequestJoin")]
    void RequestJoin()
    {
        //아무 패킷이나 보내서 UDP 연결
        EventPacket pac = new EventPacket();
        networkManager.sendQue.Enqueue(pac);
        // broadcast 용 패킷만들어서 enque
        return;
    }
}
