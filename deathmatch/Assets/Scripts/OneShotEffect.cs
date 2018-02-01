using UnityEngine;
using System.Collections;

public class OneShotEffect : MonoBehaviour
{
    private ParticleSystem ps;


    public void Start()
    {
        ps = GetComponent<ParticleSystem>();
        transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform);
    }

    public void Update()
    {
        if (ps)
        {
            if (!ps.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}