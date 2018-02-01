using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGM : MonoBehaviour {

    public GameObject zombie;
    public GameObject ZoneHolder;

    public List<GameObject> spawnPointZones = new List<GameObject>();
    public List<bool> spawnPointZonesBool = new List<bool>();

    //active spawn point pool
    public List<Transform> activeSpawnPointPool = new List<Transform>();

    //match settings
    public float roundOneZombies = 10f;
    double lastMaxZombies;
    public float ZombieScaler = 1.3f;
    public float maxWaitTime = 30f;

    //match stats
    public int roundnumber = 0;
    public double ZombiesLeftToSpawn;
    public int zombiesActive = 0;
    public bool waitingForNextRound = false;
    public double currentWaitTime = 0f;
    bool spawnZombies = false;

    //zombie spawntime
    float maxzombieSpawnTime = 1.3f;
    float currentzombieSpawnTime;

    //zombie difficulty
    int zombieMoveSpeed = 4;
    int zombiemaxHealth = 40;

    public bool testrun = true;

    void Start()
    {
        currentzombieSpawnTime = maxzombieSpawnTime;
        //adds all the zones to the zone list
        foreach (Transform child in ZoneHolder.transform)
        {
            spawnPointZones.Add(child.gameObject);
            spawnPointZonesBool.Add(false);
            
        }
        //sets the starting zone to true
        spawnPointZonesBool[0] = true;
        updateSpawns();
        //artificially starts the first round
        lastMaxZombies = roundOneZombies;
        if (!testrun) {
            startRound();
        }
    }
    void Update()
    {
        //time till next round
        if (waitingForNextRound)
        {
            if (currentWaitTime <= 0f)
            {    
                startRound();
                currentWaitTime = maxWaitTime;
            }
            //reduce time
            currentWaitTime -= Time.deltaTime;
        }
        //should be spawning zombies ?
        if (spawnZombies && waitingForNextRound == false && ZombiesLeftToSpawn > 0)
        {
            waitingForNextRound = false;
            if (currentzombieSpawnTime <= 0f)
            {
                //actually spawning a zombie
                int i = Random.Range(0, activeSpawnPointPool.Count);
                GameObject z = Instantiate(zombie, activeSpawnPointPool[i].position, activeSpawnPointPool[i].rotation);
                z.GetComponent<ZombieAI>().setStats(zombiemaxHealth, zombieMoveSpeed);
                ZombiesLeftToSpawn -= 1;
                zombiesActive += 1;
                currentzombieSpawnTime = maxzombieSpawnTime;
            }
            else
            {
                currentzombieSpawnTime -= Time.deltaTime;
            }
        }
        //if there are no zombies active or left to spawn then end the round
        if(ZombiesLeftToSpawn <= 0 && zombiesActive <= 0 && waitingForNextRound == false && !testrun)
        {
            endRound();
        }
    }
    void startRound()
    {
        roundnumber++;
        //calculate how many zombies should spawn this round
        ZombiesLeftToSpawn = lastMaxZombies * ZombieScaler;
        lastMaxZombies = ZombiesLeftToSpawn;
        //sets the difficulty of the zombies
        if (roundnumber == 3)
        {
            Debug.Log("round 3");
            zombiemaxHealth = 60;
            zombieMoveSpeed = 6;
        }
        //start spawning zombies
        spawnZombies = true;
        waitingForNextRound = false;
    }
    void endRound()
    {
        currentWaitTime = maxWaitTime;
        Debug.Log("endround");
        updateSpawns();
        //start countdown
        waitingForNextRound = true;
    }
    //finds which spawn zones are activated and then add the spawnpoints to the pool
    public void updateSpawns()
    {
        //Debug.Log("updateSpawn");
        for (int i = 0; i < spawnPointZones.Count; i++)
        {
            //if the zone is active then add its spawns to the pool
            if (spawnPointZonesBool[i])
            {
                foreach (Transform child in spawnPointZones[i].transform)
                {
                    if(!activeSpawnPointPool.Contains(child))
                    activeSpawnPointPool.Add(child);
                }
            }
        }
    }

}