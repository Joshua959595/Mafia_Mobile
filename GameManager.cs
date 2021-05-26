using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string nickname;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
