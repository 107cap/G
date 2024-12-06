using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] int id;
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;

    Rigidbody m_Rigidbody;
    Vector3 movement;
    Vector3 backupPos;
    Vector3 backupColVec;
    //Vector3 nextPosition;

    //IPacket packet;
    //TODO - 서버와 연결 테스트 후 캡슐화하기
    public PlayerPacket playerPacket;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        playerPacket = new PlayerPacket();
        //packet = new PlayerPacket();
    }
    //public PlayerPacket SelfPlayerUpdate(PlayerPacket playerPacket)
    //{
    //    if (playerPacket == null)
    //        playerPacket = new PlayerPacket();

    //    if (packet != null)
    //    {
    //        if(playerPacket.ClientNum == GameManager.Instance.GetSelfClientNum())
    //            transform.position += playerPacket.GetPosition2Vec3();
    //    }

    //    //TODO - 로컬 변수 추가
    //    if(GameManager.Instance.isStarting)
    //    {
    //        Move();
    //        playerPacket.SetPosition(movement);
    //        playerPacket.clientNum = GameManager.Instance.GetSelfClientNum();
    //        Debug.Log(DateTime.Now.ToString("HH:mm:ss.ffff"));
    //    }

    //    return playerPacket;
    //}
    Coroutine selfUpdate = null;
    Coroutine otherUpdate = null;

    public void MoveSelf()
    {
        if (selfUpdate != null)
        {
            StopCoroutine(selfUpdate);

            //playerPacket.SetPosition(movement);
            //playerPacket.clientNum = GameManager.Instance.GetSelfClientNum();
            //GameManager.Instance.networkManager.sendQue.Enqueue(playerPacket);
        }

        selfUpdate = StartCoroutine(UpdateSelf());
    }

    public void MoveOther(PlayerPacket _pp)
    {
        if (otherUpdate != null)
            StopCoroutine(otherUpdate);

        otherUpdate = StartCoroutine(UpdateOther(_pp));
    }

    private IEnumerator UpdateOther(PlayerPacket _pp)
    {
        double o_currentTime = 0;    //other
        double o_moveTime = 0.08f;
        Vector3 o_tmpPos;
        if (_pp != null)
        {
            if (o_currentTime != 0)
                o_currentTime = 0;

            o_tmpPos = (otherUpdate != null ? backupPos : transform.position) // 코루틴 중첩 실행 시 ?백업 좌표 :현재 좌표 o_tmpPos = 
                + _pp.GetPosition2Vec3();
            Debug.Log("패킷 안에 pos : " + _pp.GetPosition2Vec3());
            o_tmpPos.y = 2;

            backupPos = o_tmpPos;
            Debug.Log("BackUp pos" + backupPos);

            while (o_currentTime < o_moveTime)  //부드러운 이동 가능
            {
                if (o_currentTime >= o_moveTime)
                {
                    o_currentTime = o_moveTime;
                }

                yield return null;

                o_currentTime += Time.deltaTime;


                //Debug.Log("other Pos : " + o_tmpPos);

                transform.position = Vector3.Lerp(transform.position, o_tmpPos, (float)(o_currentTime / o_moveTime));
            }
        }
    }


    private IEnumerator UpdateSelf()
    {
        double s_currentTime = 0;    //self
        double s_moveTime = 0.08f;
        Vector3 s_tmpPos;

        if (playerPacket == null)
            yield break;

        //TODO - 로컬 변수 추가
        if (GameManager.Instance.isStarting)
        {
            if (Move())
            {
                if (s_currentTime != 0)
                    s_currentTime = 0;

                s_tmpPos = transform.position + movement;
                s_tmpPos.y = transform.position.y;

                while (s_currentTime < s_moveTime)  //부드러운 이동 가능
                {
                    if (s_currentTime >= s_moveTime)
                    {
                        s_currentTime = s_moveTime;
                    }

                    yield return null;

                    s_currentTime += Time.deltaTime;


                    Debug.Log("My Pos : " + s_tmpPos);
                    transform.position = Vector3.Lerp(transform.position, s_tmpPos, (float)(s_currentTime / s_moveTime));
                }
                playerPacket.SetPosition(movement);
                playerPacket.clientNum = GameManager.Instance.GetSelfClientNum();
                GameManager.Instance.networkManager.sendQue.Enqueue(playerPacket);
            }
        }
    }

    private bool Move()
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
        movement += backupColVec;
        backupColVec = Vector3.zero;

        if (movement.Equals(Vector3.zero))
            return false;
        else
            return true;

        //transform.Translate(movement, Space.World);
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("나님 작동");
            backupColVec = Vector3.forward * 50;
            //MoveSelf();
        }
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("나님 작동");
            backupColVec = Vector3.forward * 50;
            //MoveSelf();
        }
    }
}