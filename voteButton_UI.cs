using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class voteButton_UI : MonoBehaviour
{
    public static voteButton_UI inst;
    public int[] playerV;
    public GameObject[] votebtns;
    public List<GameObject> UIlist = new List<GameObject>();
    public GameObject UIgroup;

    private void Awake()
    {
        if (inst == null)
            inst = this;
    }
    public void InitVote()
    {
        inst.UIgroup = inst.UIlist[GameRule.count - 4];
        inst.playerV = new int[ GameRule.count + 1];
        inst.votebtns = new GameObject[GameRule.count];
        for (int i = 0; i < inst.UIgroup.transform.childCount; i++)
        {
            votebtns[i] = inst.UIgroup.transform.GetChild(i).gameObject;
        }
        StartCoroutine(StartVote());
    }

    [PunRPC]
    public void selectVoter()
    {
         if (Player.inst.pv.IsMine && Player.inst.canvote)
            inst.UIgroup.SetActive(true);
    }

    [PunRPC]
    public void PBDown(int idx)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            playerV[idx]++;
            for (int i = 0; i < votebtns.Length; i++)
                votebtns[i].SetActive(false);
        }

    }

    IEnumerator StartVote()
    {

        RoomScript.AddLine("주의해서 10초간, 투표를 진행하십시오!");
        RoomScript.AddLine("투표 수정은 불가합니다.");
        yield return new WaitForSeconds(10f);
        int theMostVoted = CalcVote();
        RoomScript.AddLine($"투표 결과 최다득표자 P{theMostVoted}이 사망합니다.");
    }

    public int CalcVote()
    {
        int mostVotednum = playerV[0];
        int overlapped = 0;
        int mostVotedplayer = -1;

        for (int i=1; i < GameRule.count; i++)
        {
            if (mostVotednum <= playerV[i])
            {
                mostVotednum = playerV[i];
                mostVotedplayer = i;
            }
        }
        for (int i=0; i<GameRule.count; i++)
        {
            if (mostVotednum == playerV[i])
            {
                overlapped += 1;
            }
        }
        if (overlapped >= 2)
        {
            RoomScript.AddLine("최다 득표자가 두 명 이상입니다. 투표가 무효가 됩니다.");
            return -1;
        }

        for (int i = 0; i < GameRule.count; i++)
        {
            GameRule.players[i].canvote = false;
        }

        return mostVotedplayer;
        
    }
    
}

