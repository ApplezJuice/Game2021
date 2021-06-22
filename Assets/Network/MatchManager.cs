using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MatchManager : NetworkBehaviour
{
    List<PlayerNetworking> players = new List<PlayerNetworking>();
    public MatchManagerSpec matchManagerSpec;
    bool matchInProgress = false;
    float goldtimer;

    void Start()
    {
        matchInProgress = true;
        // TEMP
        SpawnUnit();
        SpawnBases();
    }

    void SpawnBases()
    {
        
    }

    void SpawnUnit()
    {

    }

    void Update()
    {
        float t = Time.time;
        float dt = Time.deltaTime;

        if (matchInProgress)
        {
            // do match things

            goldtimer += dt;
                /*
                * RESOURCES
                */
            if (goldtimer >= matchManagerSpec.goldRefreshTimer)
            {
                foreach(var player in players)
                {
                    player.AddGold();
                    goldtimer = 0f;
                }
            }
        }
    }

    public void AddPlayer(PlayerNetworking player)
    {
        players.Add(player);
    }

    public void MatchEnded()
    {
        matchInProgress = false;
    }
}
