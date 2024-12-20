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
    bool isReady = false;

    [SerializeField] GameObject Ready;
    [SerializeField] TMP_Text ReadyText;
    [SerializeField] Text raceTime;
    private DateTime startTime;
    bool start = false;

    [Header("KMJ")]
    public Text[] rankUIText;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.eventManager.Register(EventType.START_RACE, StartCount);
        GameManager.Instance.eventManager.Register(EventType.UPDATE_RANK, RankUIUpdate);

        // 시작시 체크/X 버튼 초기화, isReady = false
        //isReady = false;
        //ReadyBtn = GameObject.Find("ReadyBtn");
        ReadyBtn.GetComponent<Image>().sprite = sprites[1];
        ReadyBtn.GetComponent<Image>().color = Color.red;

        //Ready = GameObject.Find("Ready");
        Ready.SetActive(true);
        //ReadyText = Ready.GetComponent<TMP_Text>();
        //raceTime = GameObject.Find("raceTime").GetComponent<Text>();
        
        for(int i=0;i<4;i++)
        {
            rankUIText[i].text = "";
        }
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
        start = true;
        startTime = DateTime.Now;
        GameManager.Instance.isStarting = true;
    }

    public void ReadyBtnClick()
    {
        isReady = !isReady;
        if(isReady)
        {
            ReadyBtn.GetComponent<Button>().interactable = false;
            ReadyBtn.GetComponent<Image>().sprite = sprites[0];
            ReadyBtn.GetComponent<Image>().color = Color.green;

            ReadyPacket readyPacket = new ReadyPacket();
            readyPacket.SetIsReady(getisReady());
            readyPacket.clientNum = GameManager.Instance.GetSelfClientNum();
            GameManager.Instance.networkManager.sendQue.Enqueue(readyPacket);
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
        if (start)
        {
            raceTime.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
        
    }

    public void RankUIUpdate()
    {
        for (int i = 0; i < 4; i++)
        {
            if (rankUIText[i].text == "")
            {
                string playerName;
                GameManager.Instance.playerNames.TryGetValue(GameManager.Instance.eventPacket.clientNum, out playerName);
                rankUIText[i].text = playerName;
                return;
            }
        }
    }
}
