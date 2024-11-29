using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using VFolders.Libs;

public class PlayerMove : MonoBehaviour
{ 

    [SerializeField] int id;
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;

    Rigidbody m_Rigidbody;
    Vector3 movement;
    //Vector3 nextPosition;

    Coroutine selfUpdate = null;
    Coroutine otherUpdate = null;

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

    public void MoveSelf()
    {
        Debug.Log("MoveSelf");
        if (selfUpdate != null)
        {
            StopCoroutine(selfUpdate);

            //중도 정지 시, 현재까지의 위치 서버에 전송
            playerPacket.SetPosition(movement);
            playerPacket.clientNum = GameManager.Instance.GetSelfClientNum();
            GameManager.Instance.networkManager.sendQue.Enqueue(playerPacket);
        }
        selfUpdate = StartCoroutine(UpdateSelf());
    }

    public void MoveOther(PlayerPacket _pp)
    {
        Debug.Log("MoveOther");
        if (otherUpdate != null)
            StopCoroutine(otherUpdate);

        otherUpdate = StartCoroutine(UpdateOther(_pp));
    }
   
    private IEnumerator UpdateOther(PlayerPacket _pp)
    {
        if (_pp != null)
        {
            Debug.Log("UpdateOther");
            float o_currentTime = 0;    //other
            float o_moveTime = 0.08f;
            Vector3 o_tmpPos;

            if (o_currentTime != 0)
                o_currentTime = 0;

            while (o_currentTime < o_moveTime)  //부드러운 이동 가능
            {
                if (o_currentTime >= o_moveTime)
                {
                    o_currentTime = o_moveTime;
                }

                yield return null;

                o_currentTime += Time.deltaTime;

                o_tmpPos = transform.position + _pp.GetPosition2Vec3();
                o_tmpPos.y = transform.position.y;
                //Debug.LogWarning((transform.position - Vector3.Lerp(transform.position, o_tmpPos, o_currentTime / o_moveTime)).magnitude);
                //transform.position = Vector3.MoveTowards(transform.position, Vector3.Lerp(transform.position, o_tmpPos, o_currentTime / o_moveTime), Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, o_tmpPos, Time.deltaTime * 15);
            }
        }
    }


    private IEnumerator UpdateSelf()
    {
        Debug.Log("UpdateSelf");
        if (playerPacket == null)
            yield break;

        float s_currentTime = 0;    //self
        float s_moveTime = 0.08f;
        Vector3 s_tmpPos;

        //TODO - 로컬 변수 추가
        if (GameManager.Instance.isStarting)
        {
            if (Move())
            {
                if (s_currentTime != 0)
                    s_currentTime = 0;

                while (s_currentTime < s_moveTime)  //부드러운 이동 가능
                {
                    if (s_currentTime >= s_moveTime)
                    {
                        s_currentTime = s_moveTime;
                    }

                    yield return null;

                    s_currentTime += Time.deltaTime;

                    s_tmpPos = transform.position + movement;
                    s_tmpPos.y = transform.position.y;
                    transform.position = Vector3.MoveTowards(transform.position, s_tmpPos, Time.deltaTime * 15);
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

        if (movement.Equals(Vector3.zero))
            return false;
        else
            return true;

        //transform.Translate(movement, Space.World);
    }
}
