using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField nickName;
    [SerializeField] TMP_InputField ip_Address;
    [SerializeField] TMP_InputField portNum;
   
    [SerializeField] Button joinGame;
    [SerializeField] GameObject warningPopup;
    public void JoinGame()
    {
        try
        {
            GameManager.Instance.networkManager.SetConnectionInfo(ip_Address.text, portNum.text);
            GameManager.Instance.RequestJoin(nickName.text);

            SceneManager.LoadScene("Map_1");
        }
        catch (System.Exception)
        {
            warningPopup.SetActive(true);
        }
    }

    public void ExitWaringPopup()
    {
        warningPopup.SetActive(false);
    }
}


