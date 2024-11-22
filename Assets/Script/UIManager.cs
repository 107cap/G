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
        // ���۽� üũ/X ��ư �ʱ�ȭ, isReady = false
        isReady = false;
        ReadyBtn = GameObject.Find("ReadyBtn");
        ReadyBtn.GetComponent<Image>().sprite = sprites[1];
        ReadyBtn.GetComponent<Image>().color = Color.red;

        Ready = GameObject.Find("Ready");
        Ready.SetActive(true);
        ReadyText = Ready.GetComponent<TMP_Text>();
    }

    // ī��Ʈ ó�� �Լ�, �ش� �Լ� ȣ�� �ٶ�
    public void StartCount()
    {
        StartCoroutine(onReady());
        
    }

    // ī��Ʈ�ٿ� ó�� Coroutine
    private IEnumerator onReady()
    {
        int countdownValue = 3;

        while (countdownValue > 0)
        {
            ReadyText.text = countdownValue.ToString(); // �ؽ�Ʈ ������Ʈ
            yield return new WaitForSeconds(1f); // 1�� ���
            countdownValue--; // ī��Ʈ ����
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
