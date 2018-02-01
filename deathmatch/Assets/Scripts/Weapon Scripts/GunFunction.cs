using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class GunFunction : MonoBehaviour
{
    public bool active;
    public bool automatic;
    public int maxClipSize;
    public int currentClipSize;
    public double fireRate;
    double currentFireRate;
    public double reloadSpeed;
    public double currentReloadSpeed;
    public int damage;
    public int shootAmount = 1;
    //max range the raycast will shoot
    public float range = 3.0f;
    //distance before the damage falloff is calculated
    public float falloffRange;
    public bool reload = false;
    //inventory position
    public int position;
    //ads positions
    public bool canADS = true;
    //public Vector3 position1;
    //public Vector3 position2;
    public float adsSpeed = 0;
    public bool isADS = false;
    public bool shouldShow;
    public bool hasParticles = false;
    public float _minSpreadFactor = 0.02f;
    public float _maxSpreadFactor = 0.5f;
    public float _realSpreadFactor;
    private float calculatedAccuracy;
    Vector3 direction;
    public bool multiMesh = false;
    public List<GameObject> meshes = new List<GameObject>();

    //debug
    float[] samples;
    //guifunction for reload bar and scopes
    GUIFunction guif;

    LayerMask lm;

    Rigidbody rb;
	// Use this for initialization
	void Start () {
        guif = GameObject.FindGameObjectWithTag("GUIFunction").GetComponent<GUIFunction>();
        lm = ~(1 << 8);
        currentClipSize = maxClipSize;
        rb = GetComponent<Rigidbody>();
        //sets the actual max accuracy
        _maxSpreadFactor = _maxSpreadFactor - _minSpreadFactor;
        //debug
        if (multiMesh) {
            foreach (Transform child in gameObject.transform)
            {
                meshes.Add(child.gameObject);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        //calculates current accuracy percentage based off how fast we are moving
        
        if (active)
        {
            //accuracy
            if (active && isADS == false)
            {
                calculatedAccuracy = GetComponentInParent<PlayerMotor>().speed / 0.16f;
                _realSpreadFactor = (_maxSpreadFactor * calculatedAccuracy) + _minSpreadFactor;
            }
            else
            {
                _realSpreadFactor = 0;
            }
            //timers
            //firerate timer
            if (currentFireRate >= 0)
            {
                currentFireRate -= Time.deltaTime;
            }
            //reload timer
            if (currentReloadSpeed >= 0 && reload == true)
            {
                currentReloadSpeed -= Time.deltaTime;
                if (currentReloadSpeed <= 0)
                {
                    reload = false;
                    currentClipSize = maxClipSize;
                }
            }
        }
        //shows weapon or not
        if (shouldShow == false)
        {
            if (multiMesh == false)
            {
                GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                for (int o = 0; o < meshes.Count-1; o++)
                {
                    meshes[o].SetActive(false);
                }
            }
        }
        else
        {
            if (multiMesh == false)
            {
                GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                for (int o = 0; o < meshes.Count - 1; o++)
                {
                    meshes[o].SetActive(true);
                }
            }
        }
    }
    //aim down sights with right click
    public void aimDownSights(Camera Cam)
    {
        if (canADS)
        {
            if (isADS == false)
            {
                Cam.fieldOfView = 25;
                GetComponentInParent<PlayerControl>().speed = adsSpeed;
                GetComponentInParent<PlayerControl>().lookSensitivity = GetComponentInParent<PlayerControl>().lookSensitivity / 2;
                //Debug.Log("set position 1");
                //position1 = transform.localPosition;
                //transform.localPosition = position2;
            }
            else
            {
                Cam.fieldOfView = 60;
                GetComponentInParent<PlayerControl>().speed = 8f;
                GetComponentInParent<PlayerControl>().lookSensitivity = GetComponentInParent<PlayerControl>().lookSensitivity * 2;
                //Debug.Log("set position 2");
                //position2 = transform.localPosition;
                //transform.localPosition = position1;
                
            }
            isADS = !isADS;
        }
    }
    //[Client]
    public void fire(Camera Cam)
    {
        //Debug.Log(currentFireRate + " | " + currentClipSize + " | " + active);
        if (active && !reload)
        {
            if (currentFireRate <= 0 && currentClipSize >= shootAmount)
            {
                //shoots a bullet regardless if the raycast hits something or not
                currentClipSize -= shootAmount;
                currentFireRate = fireRate;
                //gets the accuracy
                direction = Cam.transform.forward;
                direction.x += Random.Range(-_realSpreadFactor, _realSpreadFactor);
                direction.y += Random.Range(-_realSpreadFactor, _realSpreadFactor);
                direction.z += Random.Range(-_realSpreadFactor, _realSpreadFactor);
                RaycastHit hit;
                Ray ray = new Ray(Cam.transform.position, direction);
                //particles
                if (hasParticles)
                {
                    ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
                    ps.Play();
                    AudioSource gunaudio = GetComponentInChildren<AudioSource>();
                    gunaudio.Play();
                }

                if (Physics.Raycast(ray, out hit, range, lm))
                {
                    //debug accuracy
                    //Instantiate(Resources.Load("Prefabs/Objects/debugCube"), hit.point, Quaternion.identity);
                    //Debug.Log("collider tag: " + hit.collider.tag + " Object Name: " + hit.collider.gameObject.name);
                    if (hit.collider.tag == "Body")
                    {
                        //Debug.Log("body");
                        if (hit.collider.GetComponent<PlayerFunctions>())
                        {
                            //hit.collider.gameObject.GetComponent<PlayerFunctions>().takeDamage(hit.collider, damage);
                            Playerhit(hit.collider.GetComponentInParent<PlayerFunctions>().gameObject.name, hit.distance, 0);
                        }
                        if (hit.collider.GetComponentInParent<ZombieAI>())
                        {
                            //hit zombie in body
                            hit.collider.GetComponentInParent<ZombieAI>().takeDamage(damage, false);
                            GetComponentInParent<PlayerFunctionZombies>().money += hit.collider.GetComponentInParent<ZombieAI>().reward;

                        }
                        //blood splatter
                        GameObject bs = (GameObject)Instantiate(Resources.Load("Prefabs/Effects/BloodSplatter"), hit.point, Quaternion.identity);
                        bs.transform.LookAt(ray.origin);
                    }
                    else if (hit.collider.tag == "Head")
                    {
                        //hit.collider.gameObject.GetComponent<PlayerFunctions>().takeDamage(hit.collider, damage);
                        if (hit.collider.GetComponent<PlayerFunctions>())
                        {
                            Playerhit(hit.collider.GetComponentInParent<PlayerFunctions>().gameObject.name, hit.distance, 1);
                        }
                        if (hit.collider.GetComponentInParent<ZombieAI>())
                        {
                            //hit zombie in head
                            hit.collider.GetComponentInParent<ZombieAI>().takeDamage(damage, true);
                            GetComponentInParent<PlayerFunctionZombies>().money += (hit.collider.GetComponentInParent<ZombieAI>().reward / 2 * 3);
                        }
                        //blood splatter
                        Instantiate(Resources.Load("Prefabs/Effects/BloodSplatter"), hit.point, Quaternion.identity);
                    }
                    else
                    {
                        //bullet hit effect
                        Instantiate(Resources.Load("Prefabs/Effects/BulletHitEffect"), hit.point, Quaternion.identity);
                    }
                }
            }
            else if (currentClipSize <= 0 && reload == false)
            {
                //reload
                reloadGun();
            }
        }
    }
    void Playerhit(string  _ID, double d, int part) { 
        GetComponentInParent<PlayerFunctions>().CmdDoDamage(_ID , part, damage);
    }
    public void reloadGun()
    {
        if (!reload && currentClipSize <= (maxClipSize-1)) {
            currentReloadSpeed = reloadSpeed;
            reload = true;
            guif.reload(this);
        }
    }
    public void cancelReload() {
        currentReloadSpeed = reloadSpeed;
    }
    void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.tag == "Terrain" && active == false) {
            //rb.isKinematic = true;
            //transform.rotation = Quaternion.Euler(new Vector3(-90, transform.rotation.y, transform.rotation.z));
        }
    }
}
