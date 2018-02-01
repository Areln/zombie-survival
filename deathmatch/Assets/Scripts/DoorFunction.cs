using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorFunction : MonoBehaviour {

    public int Cost = 10;
    public bool Activated = false;

    public GameObject objectsToDisable;
    public GameObject objectsToEnable;
    public Collider purchasecollider;
    public int i = 0;
    ZombieGM zgm;

    public void activate() {
        //enable and disable the objects need to open the path
        objectsToDisable.SetActive(false);
        objectsToEnable.SetActive(true);
        purchasecollider.enabled = false;
        //find the zgm, activates a zone if not already activated then updates the spawns
        zgm = GameObject.FindGameObjectWithTag("ZombieGM").GetComponent<ZombieGM>();
        zgm.spawnPointZonesBool[i] = true;
        zgm.updateSpawns();
        GetComponent<AudioSource>().Play();
    }
}
