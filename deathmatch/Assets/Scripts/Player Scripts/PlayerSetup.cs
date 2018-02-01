using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(PlayerFunctions))]
public class PlayerSetup : NetworkBehaviour {

    OptionManager om;

    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    Camera sceneCamera;
    // Use this for initialization
    void Start()
    {
        om = GameObject.FindGameObjectWithTag("OptionManager").GetComponent<OptionManager>();
        sceneCamera = Camera.main;
        if (!isLocalPlayer)
        {
            disableComponents();
            AssignRemoteLayer();
        }
        else
        {
            gameObject.GetComponent<PlayerControl>().lookSensitivity = om.sensitivity;
            gameObject.GetComponent<PlayerFunctions>().PlayerName = om.playername;
            //choose team or stay as spectator
            
            //sceneCamera.gameObject.SetActive(false);
        }
        GetComponent<PlayerFunctions>().Setup();
    }
    
    public override void OnStartClient() {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerFunctions _PF = GetComponent<PlayerFunctions>(); 
        //registers player as spectator
        GameManager.RegisterPlayer(_netID, _PF);
        
    }

    void AssignRemoteLayer() {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
        gameObject.GetComponent<PlayerFunctions>().headCollider.gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
        gameObject.GetComponent<PlayerFunctions>().bodyCollider.gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void disableComponents() {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }
    //when we are destroyed
    void OnDisable() {
        if (sceneCamera != null) { 
            sceneCamera.gameObject.SetActive(true);
        }
        GameManager.UnregisterPlayer(gameObject.name);
    }
}
