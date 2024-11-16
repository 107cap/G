using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerMove : MonoBehaviour
{ 

    [SerializeField] int id;
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;

    Rigidbody m_Rigidbody;
    Vector3 nextPosition;

    IPacket packet;
    //TODO - 서버와 연결 테스트 후 캡슐화하기
    public PlayerPacket playerPacket;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        //nowVelocity = GetComponent<Transform>().position;

        playerPacket = new PlayerPacket();
        packet = new PlayerPacket();
        //Debug.Log("Awake : " + packet);
    }

    // Update is called once per frame
    void Update()
    {
        
        GameManager.Instance.networkManager.receiveQue.TryDequeue(out packet);
        if (packet != null)
        {
            PlayerPacket playerPacket = packet as PlayerPacket;
            //Debug.Log(playerPacket);
            nextPosition = playerPacket.GetPosition2Vec3();
        }
        //Debug.Log(packet);
        Debug.Log(nextPosition + "클라 nextPosition -> 다음 좌표");
        //playerPacket.SetPosition((nowVelocity.x, nowVelocity.y, nowVelocity.z));
    }

    private void FixedUpdate()
    {
        transform.position = nextPosition;
        Move();

        playerPacket.SetPosition(transform.position);
    }

    private void Move()
    {
        float moveHorizontal = 0.0f;
        float moveVertical = 0.0f;

        if (Input.GetKey(KeyCode.W))
        {
            moveVertical += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveVertical -= 1.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal += 1.0f;
        }

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized * minSpeed * Time.deltaTime;

        transform.Translate(movement, Space.World);
    }

    //public string IPacketHandler.Serialize(Packet _packet)
    //{
    //    throw new System.NotImplementedException();
    //}

    //Packet IPacketHandler.Deserialize(string data)
    //{
    //    throw new System.NotImplementedException();
    //}

    //void IPacketHandler.Process()
    //{
    //    throw new System.NotImplementedException();
    //}
}
