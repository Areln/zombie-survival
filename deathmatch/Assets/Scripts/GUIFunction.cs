using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIFunction : MonoBehaviour {
    public static string t1;
    public static string t2;
    public static string t3;
    public static string t4;
    public Text healthAmount;
    public Text currentAmmoAmount;
    public Text maxAmmoAmount;
    public Text moneyAmount;
    public static GameObject pauseObject;
    static bool pauseMenuActive = false;
    public Canvas teambuttons;
    public Canvas hud;
    //cost sign
    public GameObject purchaseBox;
    float maxPurchaseboxtime = 3;
    float currentPurchasebotime;
    //crosshair
    public static float accuracyShift;
    public int accuracyscale = 200;
    public RectTransform xhair1;
    public RectTransform xhair2;
    public RectTransform xhair3;
    public RectTransform xhair4;
    //reload bar
    public GameObject reloadbar;
    public bool showReloadBar;
    GunFunction weapon;
    //intermission screen
    public GameObject intermission;
    public bool showintermission;
    //debug purposes
    ZombieGM zgm;
    public Text zombiestospawn;
    public Text zombiesactive;
    public Text roundnumber;
    public Text roundtimer;
    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(transform.gameObject);
        purchaseBox.SetActive(false);
        reloadbar.SetActive(false);
        //debug
        zgm = GameObject.FindGameObjectWithTag("ZombieGM").GetComponent<ZombieGM>();
    }
    public static void setHud(int health,int currentAmmo,int maxAmmo, int money, float accuracy) {
        t1 = health.ToString();
        t2 = currentAmmo.ToString();
        t3 = maxAmmo.ToString();
        t4 = money.ToString();
        accuracyShift = accuracy;
    }
    public static void pause(PlayerControl pc) {
        //pauseMenuActive = !pauseMenuActive;
        //Debug.Log(pauseMenuActive);
        //pauseObject.SetActive(pauseMenuActive);
        //pc.pauseToggle();
    }
    public void displayPrice(int price, bool activate) {
        purchaseBox.SetActive(activate);
        purchaseBox.GetComponentInChildren<Text>().text = price.ToString();
    }
    public void reload(GunFunction _weapon)
    {
        weapon = _weapon;
        reloadbar.GetComponentInChildren<Slider>().maxValue = (float)weapon.reloadSpeed;
        showReloadBar = true;
    }
	// Update is called once per frame
	void Update () {
        //updates the numbers
        healthAmount.text = t1;
        currentAmmoAmount.text = t2;
        maxAmmoAmount.text = t3;
        moneyAmount.text = t4;
        //debug texts
        zombiesactive.text = zgm.zombiesActive.ToString();
        zombiestospawn.text = zgm.ZombiesLeftToSpawn.ToString();
        roundnumber.text = zgm.roundnumber.ToString();
        roundtimer.text = zgm.currentWaitTime.ToString();
        //hands the PAUSE sign

        //crosshair
        //Debug.Log(accuracyShift);
        xhair1.localPosition = new Vector3(-10-(accuracyShift*accuracyscale), 0, 0);
        xhair2.localPosition = new Vector3(10 + (accuracyShift * accuracyscale), 0, 0);
        xhair3.localPosition = new Vector3(0, 10 + (accuracyShift * accuracyscale), 0);
        xhair4.localPosition = new Vector3(0, -10 - (accuracyShift * accuracyscale), 0);
        
        //reload bar
        if (showReloadBar)
        {
            reloadbar.SetActive(true);
            reloadbar.GetComponentInChildren<Slider>().value = (float)weapon.currentReloadSpeed;
            if (reloadbar.GetComponentInChildren<Slider>().value == 0)
            {
                showReloadBar = false;
                reloadbar.SetActive(false);
            }
        }
	}
}
