using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerFunctions : NetworkBehaviour {

    [SyncVar]
    public string PlayerName;
    [SyncVar]
    public int team = 0;
    int maxHealth = 100;
    [SyncVar]
    public int currethealth = 100;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    public PlayerControl pc;
    public GunFunction gf;
    GameObject Cam;
    public GameObject handPos;
    public GameObject currentWep;
    public GameObject[] Weapons = new GameObject[3];
    public GameObject ph;
    public GameObject pb;
    LayerMask lm;
    public GameObject droppos;
    public Collider headCollider;
    public Collider bodyCollider;
    public GUIFunction guif;
    public Camera mainCamera;
    public AudioListener audiolistener;
    public bool isDead = true;
    Camera sceneCamera;
    public Canvas teambuttons;
    GameManager gm;
    int money = 0;
    // Use this for initialization
    void Start() {
        sceneCamera = Camera.main;
        guif = GameObject.FindGameObjectWithTag("GUIFunction").GetComponent<GUIFunction>();

        //gets the game manager
        gm = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManager>();

        lm = ~(1 << 8);
        pb.layer = 8;
        Cam = gameObject.GetComponentInChildren<Camera>().gameObject;
        //lol = Instantiate(meleeWep, handPos.transform.position, handPos.transform.localRotation);
        //adds wep to player
        //RpcchangeWep(lol);
        //sets the current wep
        //currentWep = lol;
        //RpcpickWep(lol.GetComponent<GunFunction>().position);
    }
    public void Setup() {

        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        setDefaults();
    }
    public void setDefaults() {
        isDead = false; 
        currethealth = maxHealth;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null) {
            _col.enabled = true;   
        }
    }
    // Update is called once per frame
    void Update () {
        
        //gets the gunfunction of the currently held weapon
        if (currentWep != null)
        {
            gf = currentWep.GetComponent<GunFunction>();
        }
        else {
            autoSelectWep();
        }
        //updates GUI
        GUIFunction.setHud(currethealth, gf.currentClipSize, gf.maxClipSize, money, gf._minSpreadFactor);
        //Raycastssssss
        RaycastHit hit;
        Ray ray = new Ray(Cam.transform.position, Cam.transform.forward);
        //presses ACTION
        if (Input.GetKeyDown("e"))
        {
            if (Physics.Raycast(ray, out hit, 3.0f, lm))
            {
                if (hit.collider.tag == "Weapon")
                {
                    CmdchangeWep(hit.collider.gameObject);
                }
            }
        }   

        //presses ESCAPE
        if (Input.GetKeyDown("escape")) {
            GUIFunction.pause(pc);
        }
        //DEBUG RESPAWN
        if (Input.GetKeyDown("q"))
        {
            CmdDoDamage(transform.name, 0, 100);
        }
        //Left CLICK
        if (Input.GetMouseButton(0)) {
            Debug.Log("Pressed left click.");
            //sets the mouse to lock and no-show
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //calls the fire function from the current weapon
            currentWep.GetComponent<GunFunction>().fire(Cam.GetComponent<Camera>());
        }
        //right click
        if (Input.GetMouseButton(1))
        {
            Debug.Log("Pressed right click.");
            //sets the mouse to lock and no-show
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        //drops weapon on button press
        if (Input.GetKeyDown("n")) {
            RpcdropWep(currentWep, currentWep.GetComponent<GunFunction>().position, false);
        }
        //switches weapons when key is pressed
        if (Input.GetKeyDown("1"))
        {
            if (Weapons[0] != null)
            {
                CmdPickWep(0);
            }
        }
        if (Input.GetKeyDown("2"))
        {
            if (Weapons[1] != null)
            {
                CmdPickWep(1);
            }
        }
        if (Input.GetKeyDown("3"))
        {
            if (Weapons[2] != null)
            {
                CmdPickWep(2);
            }
        }

    }//end of update

    void autoSelectWep() {
        //primary
        if (Weapons[0] == null)
        {
            //secondary
            if (Weapons[1] == null)
            {
                //picks knife
                CmdPickWep(2);
            }
            else {
                CmdPickWep(1);
            }
            
        }
        else {
            Debug.Log("PICKED PRIMARY " + Weapons[0].name);
            CmdPickWep(0);
        }
            
        
    }
    //switches between weapons in ur inventory P1
    [Command]
    void CmdPickWep(int x) {
        //before switch
        if (currentWep != null)
        {
            currentWep.GetComponent<GunFunction>().shouldShow = false;
            currentWep.GetComponent<GunFunction>().active = false;
            if (currentWep.GetComponent<GunFunction>().reload)
            {
                Debug.Log("Reload Canceled");
                currentWep.GetComponent<GunFunction>().cancelReload();
            }
        }
        currentWep = Weapons[x];
        //after switch
        currentWep.GetComponent<GunFunction>().shouldShow = true;
        currentWep.GetComponent<GunFunction>().active = true;

        RpcPickWep(x);
    }
    [ClientRpc]
    void RpcPickWep(int x)
    {
        //before switch
        if (currentWep != null)
        {
            currentWep.GetComponent<GunFunction>().shouldShow = false;
            currentWep.GetComponent<GunFunction>().active = false;
            if (currentWep.GetComponent<GunFunction>().reload)
            {
                currentWep.GetComponent<GunFunction>().cancelReload();
            }
        }
        currentWep = Weapons[x];
        //after switch
        currentWep.GetComponent<GunFunction>().shouldShow = true;
        currentWep.GetComponent<GunFunction>().active = true;
    }
    //picks up wep off ground p1
    [Command]
    void CmdchangeWep(GameObject gunobject) {
        int x;
        //GunFunction gof = gunobject.GetComponent<GunFunction>();
        //if u are replacing a wep
        if (Weapons[gunobject.GetComponent<GunFunction>().position] != null)
        {
            
            Debug.Log(gunobject.name + " Replacing " + Weapons[gunobject.GetComponent<GunFunction>().position].name);
            if (currentWep.GetComponent<GunFunction>().reload)
            {
                Debug.Log("Reload Canceled");
                currentWep.GetComponent<GunFunction>().cancelReload();
            }
            RpcdropWep(Weapons[gunobject.GetComponent<GunFunction>().position], gunobject.GetComponent<GunFunction>().position, true);
            Weapons[gunobject.GetComponent<GunFunction>().position] = gunobject;
            if (gunobject.GetComponent<GunFunction>().position == currentWep.GetComponent<GunFunction>().position)
            {
                gunobject.GetComponent<GunFunction>().active = true;
                gunobject.GetComponent<GunFunction>().shouldShow = true;
                currentWep = gunobject;
                x = 1;
            }
            else {
                gunobject.GetComponent<GunFunction>().active = false;
                gunobject.GetComponent<GunFunction>().shouldShow = false;
                x = 0;
            }
        }
        else {
            //just picking a wep up
            Weapons[gunobject.GetComponent<GunFunction>().position] = gunobject;
            gunobject.GetComponent<GunFunction>().active = false;
            gunobject.GetComponent<GunFunction>().shouldShow = false;
            x = 0;
        }
        
        gunobject.GetComponent<Rigidbody>().useGravity = false;
        gunobject.GetComponent<Rigidbody>().isKinematic = true;
        gunobject.GetComponent<BoxCollider>().enabled = false;

        gunobject.transform.parent = handPos.transform;
        gunobject.transform.position = handPos.transform.position;
        gunobject.transform.rotation = handPos.transform.rotation;

        RpcchangeWep(gunobject, x);

    }
    //picks up wep off ground p2
    [ClientRpc]
    void RpcchangeWep(GameObject gunobject, int x)
    {
        if (x == 0) {
            //just picking a wep up
            gunobject.GetComponent<GunFunction>().active = false;
            gunobject.GetComponent<GunFunction>().shouldShow = false;
        }
        if (x == 1) {
            //replacing a wep
            gunobject.GetComponent<GunFunction>().active = true;
            gunobject.GetComponent<GunFunction>().shouldShow = true;
        }

        Weapons[gunobject.GetComponent<GunFunction>().position] = gunobject;

        gunobject.GetComponent<Rigidbody>().useGravity = false;
        gunobject.GetComponent<Rigidbody>().isKinematic = true;
        gunobject.GetComponent<BoxCollider>().enabled = false;
        gunobject.transform.parent = handPos.transform;
        gunobject.transform.position = handPos.transform.position;
        gunobject.transform.rotation = handPos.transform.rotation;
    }
    [ClientRpc]
    void RpcdropWep(GameObject wep, int position, bool replacing) {
        if (wep != Weapons[2])
        {
            wep.GetComponent<GunFunction>().active = false;
            wep.transform.parent = null;
            wep.GetComponent<Rigidbody>().useGravity = true;
            wep.GetComponent<BoxCollider>().enabled = true;
            wep.GetComponent<Rigidbody>().isKinematic = false;

            wep.GetComponent<Rigidbody>().AddForce(transform.forward * 10.0f);
            wep.transform.position = droppos.transform.position;
            wep.transform.rotation = Quaternion.Euler(new Vector3(-90, -90, transform.rotation.z));
            wep.GetComponent<GunFunction>().shouldShow = true;
            if (!replacing) {
                currentWep = null;
                Weapons[position] = null;
            }    
        }
        else {
            Debug.Log("CANT DROP THE KNIFEEEEE");
        }
    }
    [Command]
    public void CmdDoDamage(string _ID, int part, int dmg) {

        PlayerFunctions _PF = GameManager.GetPlayerFunctions(_ID);
        _PF.RpcTakeDamage(part, dmg, _ID);
        
    }
    [ClientRpc]
    public void RpcTakeDamage(int bodypart, int damageTaken, string _ID) {
        if (isDead) {
            return;
        }
        if (bodypart == 1)
        {
            damageTaken = damageTaken * 3;
        }

            //Debug.Log(gameObject.name + " has taken " + damageTaken);
            currethealth -= damageTaken;
        
            //if health = 0 then u die
        if (currethealth <= 0)
        {
            die();
        }
    }
    void die() {
        isDead = true;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        Collider _col = GetComponent<Collider>();
        if (_col != null) {
            _col.enabled = false;   
        }
        StartCoroutine(Respawn());
        //ph.SetActive(false);
        //pb.SetActive(false);
        //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        //mainCamera.enabled = false;
        //audiolistener.enabled = false;
        //sceneCamera.gameObject.SetActive(true);
        //guif.hud.enabled = true;
    }
    private IEnumerator Respawn() {
        yield return new WaitForSeconds(3f);
        setDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;
    }
    [ClientRpc]
    public void RpcRespawn() {
        ph.SetActive(true);
        pb.SetActive(true);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        mainCamera.enabled = true;
        audiolistener.enabled = true;
        sceneCamera.gameObject.SetActive(false);
        guif.hud.enabled = true;
        transform.SetParent(null);
    }
    public void chooseteam() {
        teambuttons = GameObject.FindGameObjectWithTag("GUIFunction").GetComponent<GUIFunction>().teambuttons;
        teambuttons.enabled = true;
    }
    [Command]//p2 x//
    public void CmdjoinTeam(int x, string _ID)
    {
        if (x == 1)
        {
            gm.TeamRed.Add(_ID);
        }
        if (x == 0)
        {
            gm.TeamBlue.Add(_ID);
        }
        teambuttons.enabled = false;    
    }
}
