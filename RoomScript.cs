using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;

public class RoomScript : MonoBehaviourPunCallbacks
{

    public static Text outputText;
    public string nickname;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void GameStart()
    {
        outputText = GameObject.Find("outputTxt").GetComponent<Text>();
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.NickName = nickname;
        PhotonNetwork.ConnectUsingSettings();
    }

    public static void AddLine(string lineString)
    {
        if (outputText)
            outputText.text += lineString + "\r\n";
    }

    public override void OnConnectedToMaster()
    {
        AddLine("연결이 완료되었습니다!.");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        AddLine("방 연결에 실패하여 방을 생성합니다 !");
        this.CreateRoom();
    }
    public override void OnJoinedRoom()
    {
        AddLine("방에 입장했습니다");
        PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity);
    }

    void CreateRoom()
    {
        // CreateRoom(방이름, 방옵션)
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 8 });
    }

}



