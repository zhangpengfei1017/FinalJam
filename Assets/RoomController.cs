using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : Photon.PunBehaviour
{

    public static RoomController instance;

    public GameObject playerPrefab;

    private GameObject myPlayer;

    private RoomPlayer[] allRoomPlayers;

    public UIGrid playerList;

    private int myClass = 0;

    public GameObject[] classIntro;

    public UILabel SelectClass;

    public GameObject nClassBtn;

    public GameObject pClassBtn;

    public GameObject readyBtn;

    public GameObject unReadyBtn;

    public UILabel roomTitle;

    public UILabel playerNum;

    private float startTimer=10;

    private bool isStarting = false;

    private int startingCount=10;

    public GameObject chatSystem;

    // Use this for initialization
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        myPlayer = PhotonNetwork.Instantiate("RoomPlayerPrefab", Vector3.zero, Quaternion.identity, 0);
        myPlayer.GetComponent<RoomPlayer>().SetInfo(PlayerInfo.instance.playerName, PlayerInfo.instance.playerClass, false);
        photonView.RPC("OnPlayerListUpdate", PhotonTargets.AllViaServer, null);
        roomTitle.text = PhotonNetwork.room.Name;
    }

    // Update is called once per frame
    void Update()
    {
        playerList.Reposition();
        if (PhotonNetwork.isMasterClient)
        {
            CheckStartGame();
        }
    }
    void CheckStartGame()
    {
        if (allRoomPlayers == null) {
            return;
        }
        isStarting = true;
        foreach (RoomPlayer r in allRoomPlayers)
        {
            if (!r.isReady)
            {
                if (isStarting)
                {
                    startTimer = 1;
                    startingCount = 1;
                }
                isStarting = false;
                break;
            }
        }
        if (isStarting)
        {
            startTimer -= Time.deltaTime;
            if (startTimer <= startingCount-1) {
                startingCount = Mathf.FloorToInt(startTimer+1);
                string msg = "Game will start in "+startingCount.ToString()+" seconds.";
                chatSystem.GetComponent<PhotonView>().RPC("ChatToAll", PhotonTargets.AllViaServer, msg);
            }
            if (startTimer <= 0) {
                MasterStartGame();
            }
        }
    }

    [PunRPC]
    void OnPlayerListUpdate()
    {
        allRoomPlayers = FindObjectsOfType<RoomPlayer>();
        for (int i = 0; i < allRoomPlayers.Length - 1; i++)
        {
            for (int j = 0; j < allRoomPlayers.Length - 1 - i; j++)
            {
                if (allRoomPlayers[j].GetComponent<PhotonView>().ownerId > allRoomPlayers[j + 1].GetComponent<PhotonView>().ownerId)
                {
                    RoomPlayer temp = allRoomPlayers[j];
                    allRoomPlayers[j] = allRoomPlayers[j + 1];
                    allRoomPlayers[j + 1] = temp;
                }
            }
        }
        foreach (Transform t in playerList.GetChildList())
        {
            playerList.RemoveChild(t);
            Destroy(t.gameObject);
        }
        foreach (RoomPlayer rp in allRoomPlayers)
        {
            GameObject go = Instantiate(playerPrefab, GameObject.Find("UI Root").transform);
            playerList.AddChild(go.transform);
            rp.playerUI = go.GetComponent<PlayerUI>();
        }
        playerNum.text = allRoomPlayers.Length.ToString() + "/5";
    }

    public virtual void OnPhotonPlayerDisconnected()
    {
        photonView.RPC("OnPlayerListUpdate", PhotonTargets.AllViaServer, null);
    }

    public void NextClass()
    {
        myClass = (myClass + 1) % 4;
        PlayerInfo.instance.playerClass = (HeroController.Class)myClass;
        SelectClass.text = PlayerInfo.instance.playerClass.ToString();
        myPlayer.GetComponent<RoomPlayer>().playerClass = PlayerInfo.instance.playerClass;
        ChangeIntro(myClass);
    }
    public void PrevousClass()
    {
        myClass = (myClass - 1) % 4;
        if (myClass < 0)
        {
            myClass = 3;
        }
        PlayerInfo.instance.playerClass = (HeroController.Class)myClass;
        SelectClass.text = PlayerInfo.instance.playerClass.ToString();
        myPlayer.GetComponent<RoomPlayer>().playerClass = PlayerInfo.instance.playerClass;
        ChangeIntro(myClass);
    }

    void ChangeIntro(int i)
    {
        foreach (GameObject g in classIntro)
        {
            g.SetActive(false);
        }
        classIntro[i].SetActive(true);
    }

    public void Ready()
    {
        nClassBtn.SetActive(false);
        pClassBtn.SetActive(false);
        readyBtn.SetActive(false);
        unReadyBtn.SetActive(true);
        myPlayer.GetComponent<RoomPlayer>().isReady = true;

    }
    public void Unready()
    {
        nClassBtn.SetActive(true);
        pClassBtn.SetActive(true);
        readyBtn.SetActive(true);
        unReadyBtn.SetActive(false);
        myPlayer.GetComponent<RoomPlayer>().isReady = false;
    }
    public void MasterStartGame()
    {
        PhotonNetwork.room.IsOpen = false;
        PhotonNetwork.room.IsVisible = false;
        photonView.RPC("StartGame", PhotonTargets.AllViaServer, null);
    }

    [PunRPC]
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(2);
    }





}
