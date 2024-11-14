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

    Vector3 _nowPos;                        // �� ��ġ

    #region KeyCode
    // KeyCode
    KeyCode _upKey =KeyCode.W;
    KeyCode _downKey=KeyCode.S;
    KeyCode _leftKey=KeyCode.A;
    KeyCode _rightKey=KeyCode.D;
    #endregion

    void Start()
    {
        // �� ��ġ �ʱ�ȭ
        _nowPos = transform.position;
    }

    void Update()
    {
        // GameManager���� �� ���� �б�
        UpdatePacketValue();

        // ��ġ �ʱ�ȭ
        Move();

        // �÷��̾� Ű �Է� �ޱ�
        AddInputVelocity();
    }

    void LateUpdate()
    {
        // ī�޶� ���� �ʱ�ȭ
        playerCamera.LookAt(transform);

        // GameManager�� Packet ����
        SendPacktValue();
    }

    // ��ġ �̵� �� ����
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

    // �÷��̾� Ű �Է�
    void AddInputVelocity()
    {
        // ������ ������ ��� Flags ����
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

    // �÷��̾� ������Ʈ �̵�
    void Move()
    {
        transform.Translate(SetVelocity(), Space.World);
        _nowPos=transform.position;
    }

    // GameManager�� Packet ����
    void SendPacktValue()
    {
        MoveDirFlags currentMoveFlag = flagManager.GetMoveFlags();

        playerPacket = new SendPacket(currentMoveFlag);

        GameManager.instance.SendPacket(playerPacket.Serialize());
    }

    // GameManager���� Packet ���� �ޱ�
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
