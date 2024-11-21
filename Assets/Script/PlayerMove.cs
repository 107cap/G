using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerMove : MonoBehaviour
{ 
    [SerializeField] int id;
    [SerializeField] float curSpeed;
    [SerializeField] float basicSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float modifySpeed;

    Rigidbody m_Rigidbody;
    Vector3 movement;
    //Vector3 nextPosition;

    [Header("Impact")]
    [SerializeField] bool isImpact;
    [SerializeField] Vector3 impactForce;
    [SerializeField] float curImpactTime;
    [SerializeField] float maxImpactTime;

    bool m_topMove=true;
    bool m_bottomMove=true;
    bool m_leftMove = true;
    bool m_rightMove = true;

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

    void Start()
    {
        // 기본 스피드 초기화
        curSpeed = basicSpeed;
    }

    void Update()
    {
        SpeedControl();     // speed 일정 값 조정
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
        Move();
        ImpactControl();

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
        //Debug.Log(nextPosition + "클라 nextPosition -> 다음 좌표");
        //playerPacket.SetPosition((nowVelocity.x, nowVelocity.y, nowVelocity.z));


        //입력에 의한 추가 이동
        //TODO - 로컬 변수 추가
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
            if (m_topMove)
            {
                moveVertical = 1.0f;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (m_bottomMove)
            {
                moveVertical = -1.0f;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (m_leftMove)
            {
                moveHorizontal = -1.0f;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (m_rightMove)
            {
                moveHorizontal = 1.0f;
            }
        }

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * curSpeed * Time.deltaTime;//.normalized * minSpeed * Time.deltaTime;

        //transform.Translate(movement, Space.World);
    }

    void SpeedControl()
    {
        if (curSpeed > basicSpeed)
        {
            curSpeed = Mathf.Clamp(curSpeed - modifySpeed * Time.deltaTime, basicSpeed, maxSpeed);
        }
        else if (curSpeed <= basicSpeed)
        {
            curSpeed = Mathf.Clamp(curSpeed + modifySpeed * Time.deltaTime, minSpeed, basicSpeed);
        }
    }

    void ImpactControl()
    {
        if (curImpactTime > 0)
        {
            movement += impactForce * (curImpactTime / maxImpactTime) * Time.deltaTime;
            curImpactTime -= Time.deltaTime;

            if (curImpactTime <= 0)
            {
                impactForce = Vector3.zero;
                isImpact = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject=collision.gameObject;

        switch (collisionObject.tag)
        {
            case "Wall":
                Wall basicWall = collisionObject.GetComponent<Wall>();

                switch (basicWall.posType)
                {
                    case Pos.Top:
                        m_topMove = false;
                        break;
                    case Pos.Bottom:
                        m_bottomMove = false;
                        break;
                    case Pos.Left:
                        m_leftMove = false;
                        break;
                    case Pos.Right:
                        m_rightMove = false;
                        break;
                }

                if (basicWall.wallType == WallType.Reflection)
                {
                    impactForce = basicWall.ReflectionPos();
                    curImpactTime = maxImpactTime;
                    isImpact = true;
                }

                break;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;

        switch (collisionObject.tag)
        {
            case "Wall":
                Wall basicWall = collisionObject.GetComponent<Wall>();

                switch (basicWall.posType)
                {
                    case Pos.Top:
                        m_topMove = true;
                        break;
                    case Pos.Bottom:
                        m_bottomMove = true;
                        break;
                    case Pos.Left:
                        m_leftMove = true;
                        break;
                    case Pos.Right:
                        m_rightMove = true;
                        break;
                }
                break;
        }
    }

    void OnTriggerStay(Collider other)
    {
        GameObject colliderObject=other.gameObject;

        switch (colliderObject.tag)
        {
            case "SpeedObject":
                SpeedObject speedObject= colliderObject.GetComponent<SpeedObject>();
                curSpeed = Mathf.Clamp(speedObject.SpeedControl(curSpeed),minSpeed,maxSpeed);
                break;
        }
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
