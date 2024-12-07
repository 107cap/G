using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject ReadyBtn;
    [SerializeField] Sprite[] sprites;
    [SerializeField] bool isReady;

    [SerializeField] GameObject Ready;
    [SerializeField] TMP_Text ReadyText;
    [SerializeField] Text raceTime;
    private DateTime startTime;



    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.eventManager.Register(EventType.START_RACE, StartCount);

        // 시작시 체크/X 버튼 초기화, isReady = false
        isReady = false;
        ReadyBtn = GameObject.Find("ReadyBtn");
        ReadyBtn.GetComponent<Image>().sprite = sprites[1];
        ReadyBtn.GetComponent<Image>().color = Color.red;

        Ready = GameObject.Find("Ready");
        Ready.SetActive(true);
        ReadyText = Ready.GetComponent<TMP_Text>();
        raceTime = GameObject.Find("raceTime").GetComponent<Text>();
        
    }

    public bool getisReady()
    {
        return isReady;
    }

    // 카운트 처리 함수, 해당 함수 호출 바람
    public void StartCount()
    {
        StartCoroutine(onReady());
        
        
    }

    public void Update()
    {
        calculateRaceTime();
    }

    // 카운트다운 처리 Coroutine
    private IEnumerator onReady()
    {
        int countdownValue = 3;

        while (countdownValue > 0)
        {
            ReadyText.text = countdownValue.ToString(); // 텍스트 업데이트
            yield return new WaitForSeconds(1f); // 1초 대기
            countdownValue--; // 카운트 감소
        }
        
        ReadyText.text = "START!!!";
        
        yield return new WaitForSeconds(0.5f);
        Ready.SetActive(false);
        startTime = DateTime.Now;
        GameManager.Instance.isStarting = true;
    }

    public void ReadyBtnClick()
    {
        isReady = !isReady;
        if(isReady)
        {
            ReadyBtn.GetComponent<Image>().sprite = sprites[0];
            ReadyBtn.GetComponent<Image>().color = Color.green;
        }
        else
        {
            ReadyBtn.GetComponent<Image>().sprite = sprites[1];
            ReadyBtn.GetComponent<Image>().color = Color.red;
        }
    }

    private void calculateRaceTime()
    {
        TimeSpan timer = DateTime.Now - startTime;
        int minutes = timer.Minutes; // 분 계산
        int seconds = timer.Seconds; // 초 계산
        int milliseconds = timer.Milliseconds; // 밀리초 계산

        // 타이머 텍스트 업데이트
        raceTime.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
