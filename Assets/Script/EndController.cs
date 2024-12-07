using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndController : MonoBehaviour
{
    public Button ourGod;
    public Image blackImg;
    public TMP_Text thxTMP;

    void Update()
    {
        blackImg.transform.Rotate(new Vector3(0f, 100f, 0f) * Time.deltaTime);
        thxTMP.transform.Rotate(new Vector3(0f, -100f, 0f) * Time.deltaTime);
        ourGod.transform.Rotate(new Vector3(10f, 20f, 30f) * Time.deltaTime);
    }

    public void OnclickedOurGod()
    {
        Application.Quit();
    }
}
