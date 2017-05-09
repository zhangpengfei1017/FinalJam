using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Photon.PunBehaviour
{
    public static GameController instance;

    HeroController[] allPlayersArray;

    List<GameCharacter> teammateList = new List<GameCharacter>();

    private GameCharacter localPlayer;

    public PlayerFrameUI localFrame;

    public ActionBarUI CDBar;

    public CastBarUI castBar;

    public EnemyFrameUI target;

    public UIGrid teamList;

    public GameObject teammatePrefab;

    public Transform startPosition;

    private bool ready = false;
    // Use this for initialization
    void Start()
    {
        if (PhotonNetwork.connectionState != ConnectionState.Connected)
        {
            return;
        }
        GameObject myPlayer;
        switch (PlayerInfo.instance.playerClass) {
            case HeroController.Class.Knight:
                myPlayer = PhotonNetwork.Instantiate("New_Player_Knight", startPosition.position, Quaternion.identity, 0);
                break;
            case HeroController.Class.Priest:
                myPlayer = PhotonNetwork.Instantiate("New_Player_Priest", startPosition.position, Quaternion.identity, 0);
                break;
            case HeroController.Class.Sorceress:
                myPlayer = PhotonNetwork.Instantiate("New_Player_Sorceress", startPosition.position, Quaternion.identity, 0);
                break;
            default:
                myPlayer = PhotonNetwork.Instantiate("New_Player_Knight", startPosition.position, Quaternion.identity, 0);
                break;
        }
        localFrame.SetClass((int)PlayerInfo.instance.playerClass);
        localPlayer = myPlayer.GetComponent<GameCharacter>();
        localPlayer.characterName = PlayerInfo.instance.playerName;
        CDBar.SetClass((int)PlayerInfo.instance.playerClass);
        photonView.RPC("OnPlayerJoinedGameplay", PhotonTargets.AllViaServer, null);
        Camera.main.GetComponent<AdvancedUtilities.Cameras.BasicCameraController>().Target.Target = myPlayer.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (ready)
        {
            UpdateUI();
        }
    }

    [PunRPC]
    void OnPlayerJoinedGameplay()
    {
        if (ready)
        {
            return;
        }
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        allPlayersArray = FindObjectsOfType<HeroController>();
        if (allPlayersArray.Length == PhotonNetwork.room.PlayerCount)
        {
            foreach (HeroController h in allPlayersArray)
            {
                if (!h.GetComponent<PhotonView>().isMine) {
                    teammateList.Add(h.GetComponent<GameCharacter>());
                }                
            }
            //sort
            for (int i = 0; i < teammateList.Count - 1; i++)
            {
                for (int j = 0; j < teammateList.Count - 1 - i; j++)
                {
                    if (teammateList[j].GetComponent<PhotonView>().ownerId > teammateList[j + 1].GetComponent<PhotonView>().ownerId)
                    {
                        GameCharacter temp = teammateList[j];
                        teammateList[j] = teammateList[j + 1];
                        teammateList[j + 1] = temp;
                    }
                }
            }
            CreateTeammateFrame();
        }
    }

    void CreateTeammateFrame()
    {
        foreach (Transform t in teamList.GetChildList())
        {
            Destroy(t.gameObject);
        }

        foreach (GameCharacter g in teammateList)
        {
            if (g.GetComponent<PhotonView>().isMine) {
                continue;
            }
            GameObject go = Instantiate(teammatePrefab, teamList.transform);
            teamList.AddChild(go.transform);
        }
        teamList.Reposition();
        localFrame.SetPlayerName(localPlayer.characterName);
        ready = true;
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected(otherPlayer);
        foreach (GameCharacter g in teammateList)
        {
            if (g.GetComponent<PhotonView>().ownerId == otherPlayer.ID)
            {
                teammateList.Remove(g);
            }
        }
        CreateTeammateFrame();
    }


    void UpdateUI()
    {
        //frame
        localFrame.SetHpValue((float)localPlayer.CurHP / (float)localPlayer.MaxHP);
        localFrame.SetMpValue((float)localPlayer.CurMP / (float)localPlayer.MaxMP);
        //CD time
        for (int i = 0; i < 5; i++)
        {
            CDBar.SetCD(i, localPlayer.GetCDProgress(i));
        }
        //Cast Bar
        if (localPlayer.IsCasting)
        {
            castBar.gameObject.SetActive(true);
            castBar.SetValue(localPlayer.GetCastingProgress(), localPlayer.GetCurSkillName());
        }
        else
        {
            castBar.gameObject.SetActive(false);
        }
        //target
        if (localPlayer.GetTarget() != null)
        {
            GameCharacter gc = localPlayer.GetTarget();
            target.gameObject.SetActive(true);
            target.SetPlayerName(gc.characterName);
            target.SetHpValue((float)gc.CurHP / (float)gc.MaxHP);
            if (gc.IsCasting)
            {
                target.SetCast(gc.GetCurSkillName(), gc.GetCastingProgress());
            }
            else
            {
                target.SetCast(false);
            }
        }
        else
        {
            target.gameObject.SetActive(false);
        }
        //teamma                       
        for (int i = 0; i < teammateList.Count; i++)
        {
            teamList.GetComponent<TeamListUI>().SetTeammateInfo(i, teammateList[i].characterName, (int)teammateList[i].myClass,(float)teammateList[i].CurHP / (float)teammateList[i].MaxHP, (float)teammateList[i].CurMP / (float)teammateList[i].MaxMP);
        }

    }
}
