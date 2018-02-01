using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour {

    ZombieGM zgm;

    int maxhealth = 100;
    public int currethealth = 100;
    public int reward = 10;

    public BoxCollider attackcollider;
    Vector3 targetLocation;
    public bool inAttackRange = false;
    public bool canAttack = false;
    public bool canMove = true;
    public bool isMoving = false;
    public float attackmaxcd = 3.0f;
    public float attackcd;
    public float maxtimeactive = 1.0f;
    public float timeactive;

    public NavMeshAgent agent;

	// Use this for initialization
	void Start () {
        zgm = GameObject.FindGameObjectWithTag("ZombieGM").GetComponent<ZombieGM>();
        
    }
	
	// Update is called once per frame
	void Update () {
        agent.destination = targetLocation = GameObject.FindGameObjectWithTag("Player").transform.position;
        //if in attack range
        if (agent.remainingDistance < agent.stoppingDistance && agent.remainingDistance != 0f)
        {
            inAttackRange = true;
            //Debug.Log("in range");
            attack();
        }
        else
        {
            inAttackRange = false;
        }
        //if can move and not in attack range
        if (canMove && !inAttackRange)
        {
            //Debug.Log("moving");
            isMoving = true;
        }
        
        
        //attack
        if (attackcd > 0f)
        {
            canAttack = false;
            attackcd -= Time.deltaTime;
        }
        else
        {
            canAttack = true;
        }

        if (attackcollider.enabled == true)
        {
            agent.Stop();
            timeactive = timeactive - Time.deltaTime;
            if (timeactive <= 0f)
            {
                attackcollider.enabled = false;
                timeactive = maxtimeactive;
                attackcd = attackmaxcd;
                //Debug.Log("Resume now");
                agent.Resume();

            }
        }
    }
    public void attack()
    {

        if (inAttackRange && canAttack)
        {
            attackcollider.enabled = true;
            timeactive = maxtimeactive;
        }
        else
        {

            agent.Resume();
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log(collider.name);
        if (collider.name == "Player Zombies")
        {
            //calls the damage
            //Debug.Log("DAMAGE the player");
            //takes care of resetting the attack
            attackcollider.enabled = false;
            attackcd = attackmaxcd;
            timeactive = maxtimeactive;
            agent.Resume();
        }
    }
    public void setStats(int _maxHealth, int _moveSpeed)
    {
        maxhealth = _maxHealth;
        agent.speed = _moveSpeed;
        currethealth = maxhealth;
    }
    public void takeDamage(int damageTaken, bool headshot)
    {
        if (headshot)
        {
            damageTaken = damageTaken * 3;
        }
        currethealth -= damageTaken;

        //if health = 0 then u die
        if (currethealth <= 0)
        {
            die();
        }
    }
    void die()
    {
        zgm.zombiesActive -= 1;

        Destroy(gameObject);
    }
}
