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
    


    // Start is called before the first frame update
    void Start()
    {
        // 시작시 체크/X 버튼 초기화, isReady = false
        isReady = false;
        ReadyBtn = GameObject.Find("ReadyBtn");
        ReadyBtn.GetComponent<Image>().sprite = sprites[1];
        ReadyBtn.GetComponent<Image>().color = Color.red;

        Ready = GameObject.Find("Ready");
        Ready.SetActive(true);
        ReadyText = Ready.GetComponent<TMP_Text>();
    }

    // 카운트 처리 함수, 해당 함수 호출 바람
    public void StartCount()
    {
        StartCoroutine(onReady());
        
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
}
