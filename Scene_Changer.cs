using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Scene_Changer : MonoBehaviour
{

    public void SceneChanger()
    {
        string name = GameObject.Find("Main_Menu_Screen/Nickname").GetComponent<InputField>().text.ToString();

        if (name != "")
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().nickname = name;
            SceneManager.LoadScene("BLobby_Scene");
        }
    }
}
