using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public bool isStarting = true;

    int selfClientNum = -1;
    bool? isVictory = null;
    DateTime raceTime;
    [SerializeField] float updateTime = 0.02f;

    public NetworkManager networkManager;
    public EventManager eventManager;

    //패킷 캐싱
    IPacket tmpPacket;
    EventPacket eventPacket;
    PlayerPacket playerPacket;
    //AddPlayerPacket addPlayerPacket;

    #region Singleton
    public static GameManager Instance;
    private void Awake()
    {
        Application.targetFrameRate = 60;
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

    private void Start()
    {
        eventManager.Register(EventType.ADD_PLAYER, () => { AddPlayers(); });
        eventManager.Register(EventType.JOIN_GAME, () => { SetSelfClientNum(); });

        RequestJoin();

        StartCoroutine(Process());
    }

    public GameObject playerPrefab;
    Dictionary<int, PlayerMove> playerDict = new Dictionary<int, PlayerMove>();
    Vector3[] sponPositions = new Vector3[] {
        new Vector3(0f, 2f, 0f),
        new Vector3(10f, 2f, 0f),
        new Vector3(20f, 2f, 0f),
        new Vector3(30f, 2f, 0f)
    };

    private void Update()
    {
        //Receive
        networkManager.Receive();

        #region Process

        //Debug.LogWarning("시작 패킷 수 : " + networkManager.receiveQue.Count);

        networkManager.receiveQue.TryDequeue(out tmpPacket);

        //Debug.LogWarning("남은 처리 패킷 수 : " + networkManager.receiveQue.Count);

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

                        //Debug.Log("프로세스 과정 : " + (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - playerPacket.timestamp));

                        //Update other Player Position, Not send Updateed other Position
                        //Debug.Log(selfClientNum);
                        if (!playerPacket.clientNum.Equals(selfClientNum))
                            playerDict[playerPacket.ClientNum].MoveOther(playerPacket);
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
    }

    private IEnumerator Process()
    {
        while (true)
        {
            //Send
            //본인 클라이언트의 좌표값은 항상 전송
            //if (playerDict.ContainsKey(selfClientNum))    //본인 클라이언트가 존재 시
            //    networkManager.sendQue.Enqueue(
            //        playerDict[selfClientNum].SelfPlayerUpdate(playerPacket));

            if (playerDict.ContainsKey(selfClientNum))
                playerDict[selfClientNum].MoveSelf();

            networkManager.flush();

            tmpPacket = null;

            yield return new WaitForSeconds(updateTime);
        }
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
            //Debug.Log("ADDPlayer.ClientNums : " + item);
        }

        for (int i = 0; i < addPlayerPacket.ClientNums.Length; i++)
        {
            //Debug.Log("create 전 num값 : " + addPlayerPacket.ClientNums[i]);
            CreatePlayer(addPlayerPacket.ClientNums[i], sponPositions[i]);
        }
    }

    void CreatePlayer(int clientNum, Vector3 position)
    {
        // Debug.Log("createPlayer.num : " + clientNum);

        //Early return;
        if (playerDict.ContainsKey(clientNum))
            return;

        //Create Player And Add Dict
        //Debug.Log("작동 createPlayer");
        GameObject player = Instantiate(playerPrefab);
        player.transform.position = position;
        playerDict.Add(clientNum, player.GetComponent<PlayerMove>());
    }

    [ContextMenu("DeBug/RequestJoin")]
    void RequestJoin()
    {
        //아무 패킷이나 보내서 UDP 연결
        EventPacket pac = new EventPacket();
        networkManager.sendQue.Enqueue(pac);
        networkManager.flush();
        // broadcast 용 패킷만들어서 enque
        return;
    }
}