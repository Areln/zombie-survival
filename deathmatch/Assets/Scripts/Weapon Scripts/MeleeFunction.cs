using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class MeleeFunction : MonoBehaviour
{

    public bool active;
    public double fireRate;
    double currentFireRate;
    public int damage;
    public float range = 3.0f;
    public int position;
    public bool shouldShow;

    Rigidbody rb;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            //timers

            if (currentFireRate >= 0)
            {
                currentFireRate -= Time.deltaTime;
            }
        }
        if (shouldShow == false)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void fire(Camera Cam)
    {
        //Debug.Log(currentFireRate + " | " + currentClipSize + " | " + active);
        if (active)
        {

                currentFireRate = fireRate;
                RaycastHit hit;
                Ray ray = new Ray(Cam.transform.position, Cam.transform.forward);
                if (Physics.Raycast(ray, out hit, range))
                {
                    if (hit.collider.tag == "Player")
                    {
                        Debug.Log(hit.collider.name);
                        //hit.collider.gameObject.GetComponent<PlayerFunctions>().takeDamage(hit.collider, damage);
                        CmdPlayerHit(hit.collider.name);
                    }
                }
        }
    }
    void CmdPlayerHit(string _ID)
    {
        Debug.Log(_ID + "Has been hit");
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Terrain" && active == false)
        {
            //rb.isKinematic = true;
            //transform.rotation = Quaternion.Euler(new Vector3(-90, transform.rotation.y, transform.rotation.z));
        }
    }
}
