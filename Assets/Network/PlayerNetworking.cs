using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerNetworking : NetworkBehaviour
{
    public static PlayerNetworking localPlayer;
    [SyncVar] public string MatchId;
    NetworkMatchChecker networkMatchChecker;

    void Start()
    {
        if (isLocalPlayer)
        {
            localPlayer = this;
        }

        networkMatchChecker = GetComponent<NetworkMatchChecker>();
    }

    public void SearchForMatch()
    {
        // Need to put in logic to search for a match

        // if no matches available, create one
        string matchId = MatchMaker.GetRandomMatchId();
        CmdHostMatch(matchId);
    }

    // command, so running on the server version of player
    [Command]
    void CmdHostMatch(string matchId)
    {
        MatchId = matchId;
        if(MatchMaker.instance.HostMatch(matchId, gameObject))
        {
            // it hosted the game
            Debug.Log($"<color=green>Match hosted successfully!</color>");
            networkMatchChecker.matchId = matchId.ToGuid();
            TargetHostMatch(true, matchId);
        }
        else
        {
            Debug.Log($"<color=red>Match host failed!</color>");
            TargetHostMatch(false, matchId);
        }
    }

    // To specific cliend
    [TargetRpc]
    void TargetHostMatch(bool success, string matchId)
    {
        UIHandler.instance.HostSuccess(success);
    }
}
