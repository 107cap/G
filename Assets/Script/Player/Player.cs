using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour
{
    [Header ("Player")]
    public int id;
    public float maxSpeed;
    public float minSpeed;

    [Header("Flag")]
    public FlagManager flagManager;

    [Header("PlayerCamera")]
    public PlayerCamera playerCamera;

    public IPacket playerPacket;
    public ReceivePacket gameManagerPacket;

    Vector3 _nowPos;                        // 현 위치

    #region KeyCode
    // KeyCode
    KeyCode _upKey =KeyCode.W;
    KeyCode _downKey=KeyCode.S;
    KeyCode _leftKey=KeyCode.A;
    KeyCode _rightKey=KeyCode.D;
    #endregion

    void Start()
    {
        // 현 위치 초기화
        _nowPos = transform.position;
    }

    void Update()
    {
        // GameManager에서 온 정보 읽기
        UpdatePacketValue();

        // 위치 초기화
        Move();

        // 플레이어 키 입력 받기
        AddInputVelocity();
    }

    void LateUpdate()
    {
        // 카메라 시점 초기화
        playerCamera.LookAt(transform);

        // GameManager로 Packet 전달
        SendPacktValue();
    }

    // 위치 이동 값 설정
    Vector3 SetVelocity()
    {
        Vector3 movement = Vector3.zero;

        if(flagManager.HasFlag(MoveDirFlags.Up))
        {
            movement += Vector3.up;
        }
        if(flagManager.HasFlag (MoveDirFlags.Down))
        {
            movement += Vector3.down;
        }
        if (flagManager.HasFlag(MoveDirFlags.Left))
        {
            movement += Vector3.left;
        }
        if (flagManager.HasFlag(MoveDirFlags.Right))
        {
            movement += Vector3.right;
        }

        movement = movement.normalized * minSpeed * Time.deltaTime;

        return movement;
    }

    // 플레이어 키 입력
    void AddInputVelocity()
    {
        // 이전에 설정된 모든 Flags 제거
        flagManager.InitFlags(MoveDirFlags.None);

        if (Input.GetKey(_upKey))
        {
            flagManager.AddFlags(MoveDirFlags.Up);
        }
        if (Input.GetKey(_downKey))
        {
            flagManager.AddFlags(MoveDirFlags.Down);
        }
        if (Input.GetKey(_leftKey))
        {
            flagManager.AddFlags(MoveDirFlags.Left);
        }
        if (Input.GetKey(_rightKey))
        {
            flagManager.AddFlags(MoveDirFlags.Right);
        }
    }

    // 플레이어 오브젝트 이동
    void Move()
    {
        transform.Translate(SetVelocity(), Space.World);
        _nowPos=transform.position;
    }

    // GameManager로 Packet 전달
    void SendPacktValue()
    {
        MoveDirFlags currentMoveFlag = flagManager.GetMoveFlags();

        playerPacket = new SendPacket(currentMoveFlag);

        GameManager.instance.SendPacket(playerPacket.Serialize());
    }

    // GameManager에서 Packet 전달 받기
    public void ReceivePacketValue(string data)
    {
        MoveDirFlags currentMoveFlag =flagManager.GetMoveFlags();

        gameManagerPacket=new ReceivePacket(currentMoveFlag);

        gameManagerPacket.DeSerialize(data);
    }

    public void UpdatePacketValue()
    {
        
    }
}
