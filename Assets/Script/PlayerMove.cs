using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public int id;
    public float maxSpeed;
    public float minSpeed;

    Rigidbody m_Rigidbody;

    public Vector3 nowVelocity;

    // Start is called before the first frame update
    void Start()
    {
        maxSpeed = 10f;
        minSpeed = 5f;
        m_Rigidbody = GetComponent<Rigidbody>();
        nowVelocity = GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        nowVelocity = GetComponent<Transform>().position;
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
}
