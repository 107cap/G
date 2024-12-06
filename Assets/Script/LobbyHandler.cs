using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviour
{
    [SerializeField] InputField nickName;
    [SerializeField] InputField ip_Address;
    [SerializeField] InputField portNum;
   
    [SerializeField] Button joinGame;
    [SerializeField] GameObject warningPopup;
    public void JoinGame()
    {
        try
        {
            GameManager.Instance.networkManager.SetConnectionInfo(ip_Address.text, portNum.text);

        }
        catch (System.Exception)
        {
            warningPopup.SetActive(true);
        }
    }
}


