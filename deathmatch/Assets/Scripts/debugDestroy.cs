using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugDestroy : MonoBehaviour {
    public float timeleft = 10f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timeleft -= Time.deltaTime;
        if (timeleft <= 0)
        {
            Destroy(gameObject);
        }
	}
}
