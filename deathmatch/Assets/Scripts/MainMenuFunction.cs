using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuFunction : MonoBehaviour
{
    public Text sensText;
    public string leveltoload;
    public ScreenFader sf;
    //the different Cameras cam1 is starting cam, cam2 is room select, cam3 is createroom
    public Camera Cam1, Cam2, Cam3;

    public NetworkManager nm;
    public OptionManager om;
    public List<GameObject> roomList = new List<GameObject>();

    private string roomName;
    private uint roomSize = 3;
    [SerializeField]
    private GameObject RoomListItemPrefab;
    [SerializeField]
    private Transform roomListParent;

    // Use this for initialization
    void Start()
    {
        nm = NetworkManager.singleton;
        if (nm.matchMaker == null)
        {
            Debug.Log("mm start");
            nm.StartMatchMaker();
        }
    }
    void Update() {
        sensText.text = om.sensitivity.ToString();
    }

    //starts here
    public void startZombies()
    {
        //single player zombies
        sf.fadeTo("Zombies");
    }
    public void RefreshList()
    {
        //nm.matchMaker.ListMatches(0, 20, "", OnMatchList);
        nm.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);

    }
    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (matches == null)
        {
            Debug.Log("no matches");
            return;
        }
        ClearRoomList();
        foreach (MatchInfoSnapshot match in matches)
        {
            GameObject _roomListItemGO = Instantiate(RoomListItemPrefab, roomListParent, false);
            
            
            RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();

            if (_roomListItem != null)
            {
                //AWDAWDAWD
                _roomListItem.Setup(match, JoinRoom);
            }
            //have component on object set name/ amount of users
            roomList.Add(_roomListItemGO);
        }
    }
    void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }
        roomList.Clear();
    }
    //DAWDWADAD
    public void JoinRoom(MatchInfoSnapshot _match)
    {
        //makes sure the name isnt blank
        //if (gf.charName != null && gf.charName != "")
        //{
        //nm.matchMaker.JoinMatch(_match.networkId, "", nm.OnMatchJoined);
        nm.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, nm.OnMatchJoined);
        sf.fadeToNothing();
        ClearRoomList();
        //}
    }
    public void createRoom()
    {
        roomName = "LOL";
        //&& gf.charName != null && gf.charName != "" ADD BACK IN WHEN NAMES ARE READY
        if (roomName != null)
        {
            //creates room
            //nm.matchMaker.CreateMatch(roomName, roomSize, true, "", nm.OnMatchCreate);
            //Debug.Log(nm.isActiveAndEnabled + " : " + roomName);
            nm.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, nm.OnMatchCreate);
            //  spits out room details Debug.Log(roomName + " / " + roomSize);
        }
        else
        {

        }
    }

    //buttons
    public void JoinRoomBtn() {
        //Debug.Log("Play");
        RefreshList();
        sf.fadeToCam(Cam2);

    }
    public void BackToCam1()
    {
        //Debug.Log("Back");
        sf.fadeToCam(Cam1);
    }
    public void ToOptions() {
        sf.fadeToCam(Cam3);
    }

    public void SetRoomName(string _name)
    {
        roomName = _name;
    }
    public void quit() {
        //Debug.Log("Quit");
        Application.Quit();
    }

}
