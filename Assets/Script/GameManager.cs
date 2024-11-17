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
    public EventManager eventManager;
    
    //��Ŷ ĳ��
    IPacket tmpPacket;
    EventPacket eventPacket;
    PlayerPacket playerPacket;
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
        eventManager.Register(EventType.ADD_PLAYER, () => AddPlayers());   
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

        if (tmpPacket != null)
        {
            switch (tmpPacket.Type)
            {
                case PacketType.NONE:
                    break;
                case PacketType.PLAYER:
                    if (!playerDict.Count.Equals(0))
                    {
                        playerPacket = (PlayerPacket)tmpPacket;

                        //�÷��̾� Update �� ��Ŷ ����
                        networkManager.sendQue.Enqueue(
                            playerDict[playerPacket.ClientNum].PlayerUpdate(playerPacket));

                        //Debug.Log(player.playerPacket.GetPosition() + "enque ��");
                    }
                    break;
                case PacketType.EVENT:
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
        networkManager.Send();
    }

    /// <summary>
    /// Add Players In PlayScene
    /// </summary>
    /// <param name="clientNums"></param>
    void AddPlayers()
    {
        AddPlayerPacket addPlayerPacket = (AddPlayerPacket)eventPacket;

        for (int i = 0; i < addPlayerPacket.ClientNums.Length; i++)
        {
            CreatePlayer(addPlayerPacket.ClientNums[i], sponPositions[i]);
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

    [ContextMenu("DeBug/CreatePacket")]
    void CreatePacket()
    {
        EventPacket pac = new EventPacket();
        networkManager.sendQue.Enqueue(pac);
        // broadcast 용 패킷만들어서 enque
        return;
    }
}
