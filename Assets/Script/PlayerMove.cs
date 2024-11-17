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
    //TODO - ������ ���� �׽�Ʈ �� ĸ��ȭ�ϱ�
    public PlayerPacket playerPacket;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        //nowVelocity = GetComponent<Transform>().position;

        playerPacket = new PlayerPacket();
        packet = new PlayerPacket();
        //Debug.Log("Awake : " + packet);
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

        //TODO - ���� ���� �߰�
        Move();

        playerPacket.SetPosition(movement);

        return playerPacket;
    }


    public PlayerPacket OtherPlayerUpdate(PlayerPacket playerPacket)
    {
        if (packet != null)
        {
            //Debug.Log(playerPacket.GetPosition2Vec3() + " PlayerMove");
            transform.position += playerPacket.GetPosition2Vec3();
        }
        //Debug.Log(packet);
        //Debug.Log(nextPosition + "Ŭ�� nextPosition -> ���� ��ǥ");
        //playerPacket.SetPosition((nowVelocity.x, nowVelocity.y, nowVelocity.z));


        //�Է¿� ���� �߰� �̵�
        //TODO - ���� ���� �߰�
        //Move();

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
