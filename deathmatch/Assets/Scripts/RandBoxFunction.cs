using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandBoxFunction : MonoBehaviour {

    int x;
    PlayerFunctionZombies pfz;
    bool inUse = false;
    public bool activated = true;
    public List<GameObject> Weapons = new List<GameObject>();
    public GameObject displayPos;
    public GameObject displayWep;

	// Use this for initialization
	void Start () {
		
	}

    public int Cost = 1000;

    public void activate(PlayerFunctionZombies _pfz)
    {
        if (inUse == false && activated)
        {
            if (displayWep != null && displayWep.GetComponentInParent<PlayerFunctionZombies>() != null)
            {
                displayWep = null;
            }
            else
            {
                Destroy(displayWep);
            }
            inUse = true;
            pfz = _pfz;
            //cycle weps first
            CycleWeapons();
            Invoke("giveWep", 4f);
        }
    }
    void CycleWeapons()
    {
        for (int i = 0; i < 4; i++)
        {
            Invoke("wepTimer", i);
        }
        
    }
    void wepTimer()
    {
        if (displayWep != null)
        {
            Destroy(displayWep);
        }
        x = Random.Range(0, Weapons.Count);
        displayWep = (GameObject)Instantiate(Weapons[x], displayPos.transform);
        displayWep.GetComponent<BoxCollider>().enabled = false;
        displayWep.GetComponent<Rigidbody>().useGravity = false;
        displayWep.transform.localPosition = Vector3.zero;
        displayWep.transform.localRotation = Quaternion.Euler(0, 0, 0);
        displayWep.transform.localScale = new Vector3(10, 10, 10);
    }
    void giveWep()
    {
        //Debug.Log("give wep " + x);
        displayWep.GetComponent<BoxCollider>().enabled = true;
        GetComponent<AudioSource>().Play();
        inUse = false;
    }
}
