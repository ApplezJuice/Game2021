using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MatchManager : NetworkBehaviour
{
    List<PlayerNetworking> players = new List<PlayerNetworking>();

    public void AddPlayer(PlayerNetworking player)
    {
        players.Add(player);
    }
}
