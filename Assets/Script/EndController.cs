using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndController : MonoBehaviour
{
    public Button ourGod;
    void Update()
    {
        ourGod.transform.Rotate(new Vector3(10f, 20f, 30f) * Time.deltaTime);
    }

    public void OnclickedOurGod()
    {
        Application.Quit();
    }
}
