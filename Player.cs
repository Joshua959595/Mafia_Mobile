using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player : MonoBehaviour
{
    public static Player inst;

    public string type;
    public bool ismafia = false;
    public bool ispolice = false;
    public bool isdoctor = false;
    public bool isdead = false;
    public bool canvote = false;
    public int pidx = 0;

    public static int playerCounter;


    public PhotonView pv;

    private ChatTest pchat;


    private void Start()
    {
        pchat = GameObject.Find("PhotonCore").GetComponent<ChatTest>();
        pv = PhotonView.Get(this);
        if (pv.IsMine && !inst)
        {
            inst = this;
            // PhotonNetwork.CurrentRoom.Players.Keys;
        }
    }


    [PunRPC]
    public void mChatStart()
    {
        if (pv.IsMine && ismafia)
        {
            pchat.chatClient.Subscribe(new string[] { pchat.currentChannelName + "-mafia" }, 10);
        }
    }

    [PunRPC]
    public void mChatOver()
    {
        if (pv.IsMine && ismafia)
        {
            pchat.chatClient.Subscribe(new string[] { pchat.gameChannelName }, 10);
        }
    }

    [PunRPC]
    public void BroadcastChat(string msg)
    {
        RoomScript.AddLine(msg);
    }

    [PunRPC]
    public void RoleInfo()
    {
        /*
        foreach (Player p in GameRule.players)
        {
            RoomScript.AddLine($"test {p.pidx}");
            RoomScript.AddLine($"test {pidx}");
        }*/
        if (PhotonNetwork.IsMasterClient)
        {
            RoomScript.AddLine($"당신의 직업은 {inst.type}입니다.");
        }
        else if (!PhotonNetwork.IsMasterClient)
        {

        }

    }

    //RoomScript.AddLine($"당신의 직업은 {GameRule.players[this.pidx].type}입니다..");
    //inst.pidx   



    [PunRPC]
    public void RefreshPlayers(string[] occu)
    {
        int idx = 0;
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            RoomScript.AddLine(idx.ToString());
            GameRule.players.Add(p.GetComponent<Player>());
            p.GetComponent<Player>().pidx = idx++;
            if (inst.pv.ViewID == p.GetComponent<Player>().pv.ViewID)
            {
                Debug.LogError(idx);
            }
        }

        // 1. 직업 판별
        for (int f = 0; f < GameRule.count; f++)
        {
            GameRule.players[f].type = occu[f];
            // 2. 진영 판별
            if (occu[f] == "마피아")
            {
                GameRule.players[f].ismafia = true;
                GameRule.mafiaSide++;
                GameRule.mafiaPlayerNumbers.Add(f);
            }

            if (occu[f] == "경찰")
            {
                GameRule.players[f].ispolice = true;
                GameRule.policePlayerNumber = f;
            }

            if (occu[f] == "의사")
            {
                GameRule.players[f].isdoctor = true;
                GameRule.doctorPlayerNumber = f;
            }
        }
    }
    
}
