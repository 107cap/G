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
    Vector3 movement;
    //Vector3 nextPosition;

    IPacket packet;
    //TODO - 서버와 연결 테스트 후 캡슐화하기
    public PlayerPacket playerPacket;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        playerPacket = new PlayerPacket();
        packet = new PlayerPacket();
    }
    public PlayerPacket SelfPlayerUpdate(PlayerPacket playerPacket)
    {
        if (playerPacket == null)
            playerPacket = new PlayerPacket();

        if (packet != null)
        {
            if(playerPacket.ClientNum == GameManager.Instance.GetSelfClientNum())
                transform.position += playerPacket.GetPosition2Vec3();
        }

        //TODO - 로컬 변수 추가
        if(GameManager.Instance.isStarting)
        {
            Move();
            playerPacket.SetPosition(movement);
            playerPacket.clientNum = GameManager.Instance.GetSelfClientNum();
        }

        return playerPacket;
    }


    public PlayerPacket OtherPlayerUpdate(PlayerPacket playerPacket)
    {
        if (packet != null)
        {
            transform.position += playerPacket.GetPosition2Vec3();
        }

        playerPacket.SetPosition(movement);
        
        return playerPacket; 
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

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);//.normalized * minSpeed * Time.deltaTime;

        //transform.Translate(movement, Space.World);
    }
}
