using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;

public class LobbyUser : PunBehaviour {

    public static LobbyUser _instance;

    private void Awake()
    {
        if (null == _instance)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.transform);
        }
    }

    public string name;
    private List<GameObject> Rooms = new List<GameObject>();
    public GameObject RoomPrefab;

    public GameObject RoomListObject;
    public GameObject LoginGameObject;
    public GameObject CharacterSelect;

    // Use this for initialization
    void Start () {
        name = "noddy";
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void SetName(Text inputName )
    {
        name = inputName.text;
        // Connect to the Lobby here and check all the rooms.
        if (!PhotonNetwork.ConnectUsingSettings("1.0f"))
        {
            // TODO: Do something here.
            return;
        }
        LoginGameObject.SetActive(false);
        RoomListObject.SetActive(true);
    }

    public void RefreshRoomList()
    {
        if (Rooms.Count > 0)
        {
            for (int i = 0; i < Rooms.Count; i++)
            {
                Destroy(Rooms[i]);
            }
            Rooms.Clear();
        }
        Debug.Log("");
        Debug.Log(PhotonNetwork.GetRoomList().Length);
        for (int i = 0; i < PhotonNetwork.GetRoomList().Length;)
        {
            Debug.Log("In the Loop");
            GameObject temp_room = Instantiate(RoomPrefab);
            Debug.Log(RoomPrefab.transform.parent);
            temp_room.transform.SetParent(RoomPrefab.transform.parent);
            temp_room.GetComponent<RectTransform>().localScale = RoomPrefab.GetComponent<RectTransform>().localScale;
            Vector3 tempos = RoomPrefab.GetComponent<RectTransform>().position;
            temp_room.GetComponent<RectTransform>().position = new Vector3(tempos.x, tempos.y - 20 * i, tempos.z);
            temp_room.transform.FindChild("GameName").GetComponent<Text>().text = PhotonNetwork.GetRoomList()[i].Name;
            temp_room.transform.FindChild("Players").GetComponent<Text>().text = PhotonNetwork.GetRoomList()[i].PlayerCount.ToString() + "/5";
            temp_room.transform.FindChild("SelectButton").GetComponent<Button>().onClick.AddListener( () => {
                SelectRoom(temp_room.transform.FindChild("SelectButton").GetComponent<Button>());
                });
            temp_room.SetActive(true);
            
            Debug.Log(PhotonNetwork.GetRoomList()[i].Name.ToString());
            Rooms.Add(temp_room);

            i++;
        }

    }

    public void CreateARoom(Text _RoomName)
    {
        RoomOptions RO = new RoomOptions();
        RO.MaxPlayers = byte.Parse("2");
        bool result = PhotonNetwork.CreateRoom(_RoomName.text, RO, TypedLobby.Default);

        RoomListObject.SetActive(false);
        CharacterSelect.SetActive(true);

    }

    public void SelectRoom(Button button)
    {
        GameObject room = button.transform.parent.FindChild("GameName").gameObject;
        if ( PhotonNetwork.JoinRoom(room.GetComponent<Text>().text))
        {
            RoomListObject.SetActive(false);
            CharacterSelect.SetActive(true);
        }
    }
}
