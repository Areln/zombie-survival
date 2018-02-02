using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerFunctionZombies : MonoBehaviour
{

    public string PlayerName;
    public int team = 0;
    int maxHealth = 100;

    public int currethealth = 100;
    public int money = 0;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    public PlayerControl pc;
    public PlayerMotor pm;
    public GunFunction gf;
    GameObject Cam;
    public GameObject handPos;
    public GameObject currentWep;
    public GameObject[] Weapons = new GameObject[3];
    public GameObject ph;
    public GameObject pb;
    public LayerMask lm;
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

    public CursorLockMode wantedMode;

    // Use this for initialization
    void Start()
    {

        sceneCamera = Camera.main;
        guif = GameObject.FindGameObjectWithTag("GUIFunction").GetComponent<GUIFunction>();

        //gets the game manager

        //lm = ~(1 << 8);
        lm = ~lm;

        pb.layer = 8;
        Cam = gameObject.GetComponentInChildren<Camera>().gameObject;

        //locks mouse
        Cursor.lockState = wantedMode = CursorLockMode.Locked;
    }
    public void Setup()
    {

        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
        setDefaults();
    }
    public void setDefaults()
    {
        isDead = false;
        currethealth = maxHealth;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = true;
        }
    }
    // Update is called once per frame
    void Update()
    {

        //gets the gunfunction of the currently held weapon
        if (currentWep != null)
        {
            gf = currentWep.GetComponent<GunFunction>();
        }
        else
        {
            autoSelectWep();
        }
        //updates GUI
        GUIFunction.setHud(currethealth, gf.currentClipSize, gf.ammoReserve, money, gf._realSpreadFactor);
        //Raycastssssss
        RaycastHit hit;
        Ray ray = new Ray(Cam.transform.position, Cam.transform.forward);
        //scans to display door cost
        if (Physics.Raycast(ray, out hit, 3.0f, lm))
        {
            if (hit.collider.tag == "BuyCollider")
            {
                guif.displayPrice(hit.collider.GetComponentInParent<DoorFunction>().Cost, true);
            }
            else {
                //disable price display
                guif.displayPrice(0, false);
            }
        }
        //presses ACTION
        if (Input.GetKeyDown("e"))
        {
            if (Physics.Raycast(ray, out hit, 3.0f, lm))
            {
                if (hit.collider.tag == "Weapon")
                {
                    CmdchangeWep(hit.collider.gameObject);
                }
                if (hit.collider.tag == "BuyCollider")
                {
                    if (money >= hit.collider.GetComponentInParent<DoorFunction>().Cost)
                    {
                        hit.collider.GetComponentInParent<DoorFunction>().activate();
                        money -= hit.collider.GetComponentInParent<DoorFunction>().Cost;
                    }
                    else {
                        //Debug.Log("Cant Afford");
                    }
                }
                if (hit.collider.tag == "BuySign")
                {
                    if (money >= hit.collider.GetComponent<BuySignFunction>().cost)
                    {
                        money -= hit.collider.GetComponent<BuySignFunction>().cost;
                        //must instantiate weapon before call changewep
                        GameObject buysignwep = Instantiate(hit.collider.GetComponent<BuySignFunction>().weapon);
                        CmdchangeWep(buysignwep);
                    }
                    else
                    {
                        Debug.Log("Not enough keesh");
                    }
                }
                if (hit.collider.tag == "Randwepbox")
                {
                    if (money >= hit.collider.GetComponent<RandBoxFunction>().Cost && hit.collider.GetComponent<RandBoxFunction>().activated)
                    {
                        money -= hit.collider.GetComponent<RandBoxFunction>().Cost;
                        hit.collider.GetComponent<RandBoxFunction>().activate(this);
                    }
                    else
                    {
                        Debug.Log("Not enough keesh");
                    }
                }
            }
        }

        //presses ESCAPE
        if (Input.GetKeyDown("escape"))
        {
            GUIFunction.pause(pc);
        }
        //DEBUG RESPAWN
        if (Input.GetKeyDown("q"))
        {
            CmdDoDamage(transform.name, 0, 100);
        }
        if (Input.GetKeyDown("r"))
        {
            currentWep.GetComponent<GunFunction>().reloadGun();
        }
        //Left CLICK
        if (Input.GetMouseButton(0))
        {
            //sets the mouse to lock and no-show
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //calls the fire function from the current weapon
            currentWep.GetComponent<GunFunction>().fire(Cam.GetComponent<Camera>());
        }
        //right click
        if (Input.GetMouseButtonDown(1))
        {
            //sets the mouse to lock and no-show
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //aims down sights or scopes
            currentWep.GetComponent<GunFunction>().aimDownSights(Cam.GetComponent<Camera>());
        }
        //drops weapon on button press
        if (Input.GetKeyDown("n"))
        {
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

    void autoSelectWep()
    {
        //primary
        if (Weapons[0] == null)
        {
            //secondary
            if (Weapons[1] == null)
            {
                //picks knife
                CmdPickWep(2);
            }
            else
            {
                CmdPickWep(1);
            }

        }
        else
        {
            //Debug.Log("PICKED PRIMARY " + Weapons[0].name);
            CmdPickWep(0);
        }


    }
    //switches between weapons in ur inventory P1
    void CmdPickWep(int x)
    {
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
    public void CmdchangeWep(GameObject gunobject)
    {
        int x;
        //GunFunction gof = gunobject.GetComponent<GunFunction>();
        //if u are replacing a wep
        if (Weapons[gunobject.GetComponent<GunFunction>().position] != null)
        {
            // if the player is trying to pick up the same weapon, its ammo is added instead
            if (Weapons[gunobject.GetComponent<GunFunction>().position].GetComponent<GunFunction>().GunName == gunobject.GetComponent<GunFunction>().GunName)
            {
                Debug.Log("same name: " + gunobject.GetComponent<GunFunction>().ammoReserve);
                gunobject.GetComponent<GunFunction>().setUp();
                Weapons[gunobject.GetComponent<GunFunction>().position].GetComponent<GunFunction>().ammoReserve += gunobject.GetComponent<GunFunction>().ammoReserve + gunobject.GetComponent<GunFunction>().currentClipSize;
                Destroy(gunobject);
                return;
            }
            if (currentWep.GetComponent<GunFunction>().reload)
            {
                //Debug.Log("Reload Canceled");
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
            else
            {
                gunobject.GetComponent<GunFunction>().active = false;
                gunobject.GetComponent<GunFunction>().shouldShow = false;
                x = 0;
            }
        }
        else
        {
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

        autoSelectWep();
        //RpcchangeWep(gunobject, x);

    }
    void RpcdropWep(GameObject wep, int position, bool replacing)
    {
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
            if (!replacing)
            {
                currentWep = null;
                Weapons[position] = null;
            }
        }
        else
        {
            Debug.Log("CANT DROP THE KNIFEEEEE");
        }
    }
    public void CmdDoDamage(string _ID, int part, int dmg)
    {

        PlayerFunctions _PF = GameManager.GetPlayerFunctions(_ID);
        _PF.RpcTakeDamage(part, dmg, _ID);

    }
    public void RpcTakeDamage(int bodypart, int damageTaken, string _ID)
    {
        if (isDead)
        {
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
            //die();
        }
    }
}
