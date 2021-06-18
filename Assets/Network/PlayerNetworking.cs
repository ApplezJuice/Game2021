using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerNetworking : NetworkBehaviour
{
    public static PlayerNetworking localPlayer;
    [SyncVar] public string MatchId;
    [SyncVar] public string Name;
    NetworkMatchChecker networkMatchChecker;

    void Start()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();

        if (isLocalPlayer)
        {
            localPlayer = this;
        }
        else
        {
            // not local player stuff will happen here. (Spawn in other player avatar in UI etc.)
            UIHandler.instance.SpawnPlayerName(this);
        }

    }

    public void SetName(string name)
    {
        CmdUpdateName(name);
    }

    [Command]
    void CmdUpdateName(string name)
    {
        Name = name;
    }

    public void SearchForMatch()
    {
        // Set Player name before searching

        // Need to put in logic to search for a match
        CmdJoinMatch();
    }

    // command, so running on the server version of player
    [Command]
    void CmdHostMatch(string matchId)
    {
        MatchId = matchId;
        if(MatchMaker.instance.HostMatch(matchId, gameObject))
        {
            // it hosted the game
            ServerLog.Log(ServerLog.LogType.Info, "Match hosted successfully!");
            networkMatchChecker.matchId = matchId.ToGuid();
            TargetHostMatch(true, matchId);
        }
        else
        {
            ServerLog.Log(ServerLog.LogType.Error, "Match host failed!");
            TargetHostMatch(false, matchId);
        }
    }

    // To specific cliend
    [TargetRpc]
    void TargetHostMatch(bool success, string matchId)
    {
        UIHandler.instance.HostSuccess(success);
    }

    // command, so running on the server version of player
    [Command]
    void CmdJoinMatch()
    {
        var match = MatchMaker.instance.JoinMatch(gameObject);
        if (match.Item1)
        {
            // player joined a game
            //ServerLog.Log(ServerLog.LogType.Info, "Match joined successfully!");
            MatchId = match.Item2;
            networkMatchChecker.matchId = match.Item2.ToGuid();
            TargetJoinMatch(true);
        }
        else
        {
            //ServerLog.Log(ServerLog.LogType.Debug, "No open matches!");
            TargetJoinMatch(false);
        }
    }

    // To specific client
    [TargetRpc]
    void TargetJoinMatch(bool success)
    {
        if (!success)
        {
            // if no matches available, create one
            string matchId = MatchMaker.GetRandomMatchId();
            CmdHostMatch(matchId);
        }
        else
        {
            // begin match
            CmdBeginMatch();
        }

        UIHandler.instance.HostSuccess(success);
    }

    // command, so running on the server version of player
    [Command]
    void CmdBeginMatch()
    {
        ServerLog.Log(ServerLog.LogType.Debug, "Match beginning!");
        MatchMaker.instance.BeginMatch(MatchId);
    }

    public void StartMatch()
    {
        TargetBeginMatch();
    }

    // To all clients
    [TargetRpc]
    void TargetBeginMatch()
    {
        ServerLog.Log(ServerLog.LogType.Debug, $"{MatchId} match beginning!");
        Debug.Log("Match starting on TargetRpc!");
        // load match scene
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
        UIHandler.instance.LoadMatchUI();
    }
}
