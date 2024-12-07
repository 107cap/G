using System.Collections;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    public bool isShake;

    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        if (!isShake)
            transform.position = player.transform.position + offset;
    }

    // 예시: 특정 이벤트 발생 시 흔들림 호출
    public void TriggerShake()
    {
        StartCoroutine(Shake(0.4f, 0.3f)); // 0.5초 동안 흔들림 강도 0.3
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        isShake = true;
        //Vector3 originalPosition = originalPos;

        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0.0f;
        float x, z;
        float tmpMagnitude = magnitude;
        while (elapsed < duration)
        {
            x = (Random.Range(-1f, 1f) * magnitude);
            z = (Random.Range(-1f, 1f) * magnitude);
            
            //Debug.Log("x : " + x + "  z : " + z);
            // 플레이어 좌표 기준으로 shake
            //Debug.Log(player.transform.position);
            transform.position = new Vector3(player.transform.position.x + x + offset.x, transform.position.y, player.transform.position.z + z + offset.z);
            //Debug.Log(transform.position);
            //Debug.Log("Local X : " + transform.localPosition.x + "  Local Y : " + transform.localPosition.z);
            //Debug.Log("exc");
            /*
            
            if (elapsed > duration)
                elapsed = duration;
            tmpMagnitude = Mathf.Lerp(magnitude, 0, (float)(duration / elapsed));
            */
            elapsed += Time.deltaTime;
            yield return null;
        }
        Debug.Log("shake");
        float currentTime = 0;
        float s_moveTime = 1.0f;
        
        while (currentTime < s_moveTime)
        {
            currentTime += Time.deltaTime;

            if (currentTime > s_moveTime)
                currentTime = s_moveTime;

            transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, (currentTime/s_moveTime));

            Debug.Log(transform.position);
        }
        isShake = false;
    }
}
