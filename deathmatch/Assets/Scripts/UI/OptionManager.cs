using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour {

    public string playername = "NooB";
    public float sensitivity = 3f;

    void Start() {
        DontDestroyOnLoad(transform.gameObject);
    }
    public void SetName(string _name) {
        Debug.Log("Set Name: " + _name);
        playername = _name;
    }
    public void SetSens(float _Value)
    {
        sensitivity = _Value;
    }
}
