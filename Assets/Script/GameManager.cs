using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool? isVictory = null;
    DateTime raceTime;

    public NetworkManager networkManager = new NetworkManager();
    public EventManager eventManager = new EventManager();
    
    //패킷 캐싱
    IPacket tmpPacket;
    PlayerPacket playerPacket;
    AddPlayerPacket addPlayerPacket;

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
            TypeNameHandling = TypeNameHandling.All,  // 타입 정보 포함
        };
    }
    #endregion

    private void Start()
    {
        eventManager.Register(EventType.ADD_PLAYER, AddPlayers(addPlayerPacket));   
    }

    public GameObject playerPrefab;
    Dictionary<int, PlayerMove> playerDict = new Dictionary<int, PlayerMove>();
    Vector3[] sponPositions = new Vector3[] {
        new Vector3(0f, 0f, 0f),
        new Vector3(10f, 0f, 0f),
        new Vector3(20f, 0f, 0f),
        new Vector3(30f, 0f, 0f)
    };

    void Update()
    {
        //Receive
        networkManager.Receive();

        #region Process

        networkManager.receiveQue.TryDequeue(out tmpPacket);

        switch (tmpPacket.Type)
        {
            case PacketType.NONE:
                break;
            case PacketType.PLAYER:
                if (!playerDict.Count.Equals(0))
                {
                    playerPacket = (PlayerPacket)tmpPacket;

                    //플레이어 Update 및 패킷 래핑
                    networkManager.sendQue.Enqueue(
                        playerDict[playerPacket.ClientNum].PlayerUpdate(playerPacket));

                    //Debug.Log(player.playerPacket.GetPosition() + "enque 뒤");
                }
                break;
            case PacketType.EVENT:
                eventManager.Invoke((tmpPacket as EventPacket).eventType);
                break;
            case PacketType.ERROR:
                break;
            default:
                break;
        }

        #endregion

        //Send
        networkManager.Send();
    }

    /// <summary>
    /// Add Players In PlayScene
    /// </summary>
    /// <param name="clientNums"></param>
    void AddPlayers(int[] clientNums)
    {
        for (int i = 0; i < clientNums.Length; i++)
        {
            CreatePlayer(clientNums[i], sponPositions[i]);
        }
    }
    
    void CreatePlayer(int clientNum, Vector3 position)
    {
        //Early return;
        if (playerDict.ContainsKey(clientNum))
            return;

        //Create Player And Add Dict
        GameObject player = Instantiate(playerPrefab);
        player.transform.position = position;
        playerDict.Add(clientNum, player.GetComponent<PlayerMove>());
    }
}
