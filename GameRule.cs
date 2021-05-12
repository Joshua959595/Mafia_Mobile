using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;

public class GameRule : MonoBehaviourPun
{
    // 변수 설정

    public static List<Player> players = new List<Player>();
    public static List<int> mafiaPlayerNumbers = new List<int>();
    public static int count = 1;
    public static int MN;
    public static int mafiaSide;
    public static int citizenSide;
    public static int policePlayerNumber;
    public static int doctorPlayerNumber;

    public int DayCount = 1;
    public int theMostVoted;
    public int beingInvestigated;
    public int beingProtected;
    public int beingmafiaKilled;

    public bool GameOngoing = false;

    public GameObject Canvas;
    public GameObject GameStartBtn;
    public GameObject Police_UI;
    public PhotonView pv;

    // 셔플 함수
    public void Shuffle(ref List<string> ol)
    {
        for (int i = 0; i < ol.Count; i++)
        {
            int rnd = Random.Range(0, ol.Count);
            string tmp = ol[rnd];
            ol[rnd] = ol[i];
            ol[i] = tmp;
        }
    }

    // 챗 종료 메소드
    public void chatOver()
    {
        ChatTest.isallmuted = true;
        Vote();
    }

    // 챗 시작 메소드
    public void chatStart()
    {
        ChatTest.isallmuted = false;

    }

    // 경찰 UI
    public void yesmafiaUI_Inactivate()
    {
        Police_UI.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void nomafiaUI_Inactivate()
    {
        Police_UI.transform.GetChild(1).gameObject.SetActive(false);
    }

    // 멍때리기 함수
    public void HitMung()
    {
        return;
    }

    public void WinOrLoseOne()
    {
        // 마피아가 죽을 경우 2인이면 1인체제, 1인이면 게임 종료
        if (MN == 1)
        {
            if (players[mafiaPlayerNumbers[0]].isdead)
            {
                RoomScript.AddLine("마피아가 모두 죽고 시민이 승리하였습니다!");
                GameOngoing = false;
                players.Clear();
                return;
            }
        }
        else if (MN == 2)
        {
            if (players[mafiaPlayerNumbers[0]].isdead || players[mafiaPlayerNumbers[1]].isdead)
            {
                MN = 1;

            }
        }
        if (mafiaSide == citizenSide)
        {
            RoomScript.AddLine("마피아의 수가 시민 진영의 수와 같아져, 마피아가 승리합니다.");
            GameOngoing = false;
            players.Clear();
            return;
        }
        Night();
    }

    public void WinOrLoseTwo()
    {
        // 마피아가 죽을 경우 2인이면 1인체제, 1인이면 게임 종료
        if (MN == 1)
        {
            if (players[mafiaPlayerNumbers[0]].isdead)
            {
                RoomScript.AddLine("마피아가 모두 죽고 시민이 승리하였습니다!");
                GameOngoing = false;
                players.Clear();
                return;
            }
        }
        else if (MN == 2)
        {
            if (players[mafiaPlayerNumbers[0]].isdead || players[mafiaPlayerNumbers[1]].isdead)
            {
                MN = 1;

            }
        }
        if (mafiaSide == citizenSide)
        {
            RoomScript.AddLine("마피아의 수가 시민 진영의 수와 같아져, 마피아가 승리합니다.");
            GameOngoing = false; 
            players.Clear();
            return;
        }
        Day();
    }

    private void Start()
    {
        StartCoroutine("GetCount");
        Canvas = GameObject.Find("Canvas");
        GameStartBtn = GameObject.Find("GameStart");
        GameStartBtn.SetActive(false);

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
            Debug.developerConsoleVisible = true;

        if (!PhotonNetwork.IsMasterClient) return;

        if (GameOngoing || count < 4)
        {
            GameStartBtn.SetActive(false);
        }

        else if (!GameOngoing && 4 <= count && count <= 8)
        {
            GameStartBtn.SetActive(true);
        }

    }

    IEnumerator GetCount()
    {
        while (true)
        {
            if (PhotonNetwork.InRoom)
            {
                int tmp = 0;
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    tmp++;

                count = tmp;
            }
            yield return new WaitForSeconds(2f);
        }
    }

    public void GameStart()
    {
        // 게임 시작 bool
        GameOngoing = true;
        // 방 입장 인원수에 맞는 직업 리스트 생성
        List<string> OccupationList = new List<string>();
        // 4-5인일 때, 마피아1&경찰1&의사1&시민1-2
        if (count <= 5)
        {
            OccupationList.Add("마피아");
            OccupationList.Add("경찰");
            OccupationList.Add("의사");
            OccupationList.Add("시민");
            if (count == 5)
                OccupationList.Add("시민");
        }
        // 6-8인일 때, 마피아1-2&경찰1&의사1&시민2-5
        MN = Random.Range(1, 3);
        if (count >= 6 && count <= 8)
        {
            OccupationList.Add("마피아");
            OccupationList.Add("경찰");
            OccupationList.Add("의사");
            if (MN == 1)
                OccupationList.Add("시민");
            else
                OccupationList.Add("마피아");
            OccupationList.Add("시민");
            OccupationList.Add("시민");
            if (count == 7)
                OccupationList.Add("시민");
            if (count == 8)
                OccupationList.Add("시민");
        }

        // OccupationList 리스트 문자열 셔플
        Shuffle(ref OccupationList);

        int idx = -1;
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            Debug.Log("idx: " + idx);
            players.Add(p.GetComponent<Player>());
            p.GetComponent<Player>().pidx = ++idx;
            p.gameObject.name = "Player" + idx.ToString();
        }

        // 1. 직업 판별
        for (int f = 0; f < count; f++)
        {
            players[f].type = OccupationList[f];
            // 2. 진영 판별
            if (OccupationList[f] == "마피아")
            {
                players[f].ismafia = true;
                mafiaSide++;
                mafiaPlayerNumbers.Add(f);
            }

            if (OccupationList[f] == "경찰")
            {
                players[f].ispolice = true;
                policePlayerNumber = f;
            }

            if (OccupationList[f] == "의사")
            {
                players[f].isdoctor = true;
                doctorPlayerNumber = f;
            }
            citizenSide = count - mafiaSide;
        }

        Debug.Log(OccupationList);

        Player.inst.pv.RPC("RefreshPlayers", RpcTarget.Others, OccupationList.ToArray());

        Player.inst.pv.RPC("BroadcastChat", RpcTarget.All, "마피아 게임을 시작합니다..");

        Player.inst.pv.RPC("RoleInfo", RpcTarget.All);
       

        Day();
    }


    void Day()
    {
        // beingKilled 있으면 알려줌 없으면 스킵 (docProtect == mafiaKill)
        if (DayCount != 1)
        {
            if (players[beingmafiaKilled] == players[beingProtected])
                RoomScript.AddLine("의사가 마피아로부터 공격당한 사람을 살렸습니다!");
            if (players[beingmafiaKilled] != players[beingProtected])
            {
                RoomScript.AddLine("의사가 마피아로부터 공격당한 사람을 살리지 못했습니다!");
                players[beingmafiaKilled].isdead = true;
            }
        }
        // 1분간 채팅 & 투표
        chatStart();
        Player.inst.pv.RPC("BroadcastChat", RpcTarget.All, $"{DayCount}번 째 날, 아침이 되었습니다.");
        Player.inst.pv.RPC("BroadcastChat", RpcTarget.All, "1분간 자유롭게 대화하여 마피아를 색출하십시오.");
        Invoke("chatOver", 60f);

    }

    void Vote()
    {
        Player.inst.pv.RPC("BroadcastChat", RpcTarget.All, "1분이 지났습니다. 투표를 시작합니다.");
        Player.inst.pv.RPC("BroadcastChat", RpcTarget.All, "처형할 사람을 골라주세요.");

        for (int i = 0; i < count; i++)
        {
            if (!players[i].isdead)
            {
                players[i].canvote = true;
            }
        }

        GameObject.Find("Canvas").GetComponent<voteButton_UI>().InitVote();
        GameObject.Find("Canvas").GetComponent<voteButton_UI>().selectVoter();

        if (!players[theMostVoted].ismafia)
            citizenSide--;
        WinOrLoseOne();
    }


    void Night()
    {
        // 밤
        RoomScript.AddLine("밤이 되었습니다. 마피아, 경찰, 의사가 일을 시작합니다.");
        // 마피아 간 대화 10초 & 인터페이스 클릭으로 결정
        if (MN == 1)
        {
            players[mafiaPlayerNumbers[0]].mChatStart();
        }
        if (MN == 2)
        {
            players[mafiaPlayerNumbers[0]].mChatStart();
            players[mafiaPlayerNumbers[1]].mChatStart();
        }
        Invoke("mChatOver", 10f);

        // 마피아 능력 발동
        if (MN == 1)
            players[mafiaPlayerNumbers[0]].canvote = true;
        RoomScript.AddLine("마피아는 밤에 죽일 사람을 고르십시오.");
        RoomScript.AddLine("주의하십시오! 클릭 수정은 불가하며,");
        RoomScript.AddLine("의견이 갈릴 시, 오늘 밤은 아무도 죽지 않습니다.");

        if (MN == 2)
        {
            players[mafiaPlayerNumbers[0]].canvote = true;
            players[mafiaPlayerNumbers[1]].canvote = true;
            RoomScript.AddLine("마피아는 상의를 통해 죽일 사람을 고르십시오.");
            RoomScript.AddLine("주의하십시오! 클릭 수정은 불가하며,");
            RoomScript.AddLine("의견이 갈릴 시, 오늘 밤은 아무도 죽지 않습니다.");
        }

        GameObject.Find("Canvas").GetComponent<voteButton_UI>().InitVote();
        GameObject.Find("Canvas").GetComponent<voteButton_UI>().selectVoter();
        GameObject.Find("Canvas").GetComponent<voteButton_UI>().PBDown(count);
        beingmafiaKilled = GameObject.Find("Canvas").GetComponent<voteButton_UI>().CalcVote();

        if (beingmafiaKilled != -1)
        {
            players[beingmafiaKilled].isdead = true;
        }

        if (!players[beingmafiaKilled].ismafia)
            citizenSide--;


        // 경찰이 UI로 투표
        RoomScript.AddLine("경찰은 5초 이내로 조사를 진행하십시오.");
        if (!players[policePlayerNumber].isdead)
        {
            players[policePlayerNumber].canvote = true;
            GameObject.Find("Canvas").GetComponent<voteButton_UI>().InitVote();
            GameObject.Find("Canvas").GetComponent<voteButton_UI>().selectVoter();
            GameObject.Find("Canvas").GetComponent<voteButton_UI>().PBDown(count);
            beingInvestigated = GameObject.Find("Canvas").GetComponent<voteButton_UI>().CalcVote();

            // 경찰 관련 UI 불러오기
            Police_UI = GameObject.Find("Police_UI");

            // 맞는지아닌지 확인
            // 맞을 때
            if (GameRule.players[beingInvestigated].ismafia)
            {
                Police_UI.transform.GetChild(0).gameObject.SetActive(true);
                Invoke("yesmafiaUI_Inactivate", 3f);
            }
            // 틀릴 때
            else
            {
                Police_UI.transform.GetChild(1).gameObject.SetActive(true);
                Invoke("nomafiaUI_Inactivate", 3f);
            }
        }
        else
            Invoke("HitMung", 5f);

        // 의사가 UI로 투표
        if (!players[doctorPlayerNumber].isdead)
        {
            players[doctorPlayerNumber].canvote = true;

            RoomScript.AddLine("의사는 4초 이내로 밤 새 치료할 사람을 정하십시오.");

            GameObject.Find("Canvas").GetComponent<voteButton_UI>().InitVote();
            GameObject.Find("Canvas").GetComponent<voteButton_UI>().selectVoter();
            GameObject.Find("Canvas").GetComponent<voteButton_UI>().PBDown(count);
            beingProtected = GameObject.Find("Canvas").GetComponent<voteButton_UI>().CalcVote();
        }
        else
            Invoke("hit_the_Mung", 4f);

        // DayCount ++
        DayCount++;

        WinOrLoseTwo();
    }
}

