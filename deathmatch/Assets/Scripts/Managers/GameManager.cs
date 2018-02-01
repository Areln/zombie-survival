using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

    private const string PLAYER_ID_PREFIX = "Player ";

    public static Dictionary<string, PlayerFunctions> PlayerFunctions = new Dictionary<string, PlayerFunctions>();

    public List<string> TeamRed = new List<string>();
    public List<string> TeamBlue = new List<string>();



    public static string playerid;

    public void Start() {
        
    }

    public static void RegisterPlayer(string _netID, PlayerFunctions _PF) {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        PlayerFunctions.Add(_playerID, _PF);
        _PF.transform.name = _playerID;
        GameObject spectatorStorage = GameObject.FindGameObjectWithTag("SpecatorStorage");
        _PF.gameObject.transform.SetParent(spectatorStorage.transform);
        //disables player body
        _PF.ph.SetActive(false);
        _PF.pb.SetActive(false);
        _PF.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        _PF.mainCamera.enabled = false;
        _PF.audiolistener.enabled = false;

        //pf.chooseteam
        _PF.chooseteam();

        playerid = _playerID;
        Debug.Log(playerid);
    }
    [Client]//p1 x
    public void CmdjoinRed()
    {
        Debug.Log("join red start");
        if (TeamRed.Count <= 4)
        {
            Debug.Log("joined red");
            GetPlayerFunctions(playerid).CmdjoinTeam(1, playerid);
            
            //Spawns Character
            GetPlayerFunctions(playerid).team = 1;
            GetPlayerFunctions(playerid).RpcRespawn();
        }
    }
    [Client] //p1 x
    public void CmdjoinBlue()
    {
        Debug.Log("join blue start");
        if (TeamBlue.Count <= 4)
        {
            Debug.Log("joined blue; " + playerid);
            GetPlayerFunctions(playerid).CmdjoinTeam(0, playerid);
            //Spawns Character
            GetPlayerFunctions(playerid).team = 0;
            GetPlayerFunctions(playerid).RpcRespawn();
        }
    }


    public static PlayerFunctions GetPlayerFunctions(string _playerID) {
        return PlayerFunctions[_playerID];
    }
    public static void UnregisterPlayer(string _playerID) {
        PlayerFunctions.Remove(_playerID);
    }

}
