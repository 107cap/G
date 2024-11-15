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
    Vector3 nowVelocity;

    //TODO - ������ ���� �׽�Ʈ �� ĸ��ȭ�ϱ�
    public PlayerPacket playerPacket;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        nowVelocity = GetComponent<Transform>().position;

        playerPacket = new PlayerPacket();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        nowVelocity = GetComponent<Transform>().position;

        playerPacket.SetValue(nowVelocity);
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