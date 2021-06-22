using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Security.Cryptography;
using System.Text;

[System.Serializable] // must be system serializable if we want it networked
public class Match
{
    public string MatchId;
    public bool Joinable = true;
    public SyncListGameObject Players = new SyncListGameObject();
    public Match(string matchId, GameObject player)
    {
        MatchId = matchId;
        Players.Add(player);
    }

    public Match() {} // needs blank ctor to serialize
}

[System.Serializable]
public class SyncListGameObject : SyncList<GameObject> {}

[System.Serializable]
public class SyncListMatch : SyncList<Match> { }


public class MatchMaker : NetworkBehaviour
{
    public static MatchMaker instance;
    public SyncListMatch matches = new SyncListMatch();
    public SyncList<string> matchIds = new SyncList<string>();

    public SyncDictionary<string, MatchManager> matchManagers = new SyncDictionary<string, MatchManager>();

    [SerializeField] GameObject matchManagerPrefab;
    [SerializeField] GameObject basePrefab;
    [SerializeField] GameObject pathfindingPrefab;
    [SerializeField] GameObject unitPrefab;

    private void Start()
    {
        instance = this;
    }

    public bool HostMatch(string matchId, GameObject player)
    {
        if (!matchIds.Contains(matchId))
        {
            matchIds.Add(matchId);
            matches.Add(new Match(matchId, player));
            ServerLog.Log(ServerLog.LogType.Debug, $"Match Generated!");
            return true;
        }

        ServerLog.Log(ServerLog.LogType.Error, $"Match ID Already Exists!");
        return false;
    }

    public Tuple<bool,string> JoinMatch(GameObject player)
    {
        // TODO: end up doing matchmaking stuffs here
        if (matchIds.Count > 0)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].Players.Count == 1)
                {
                    // found match
                    matches[i].Players.Add(player);
                    ServerLog.Log(ServerLog.LogType.Info, $"Match Joined! {matches[i].MatchId}");
                    return Tuple.Create(true, matches[i].MatchId);
                }
            }
        }

        // Could not find an open match, host one
        ServerLog.Log(ServerLog.LogType.Debug, $"No open matches!");
        return Tuple.Create(false, "none");
    }

    public static string GetRandomMatchId()
    {
        string id = string.Empty;
        for (int i = 0; i < 6; i++)
        {
            int random = UnityEngine.Random.Range(0, 36);
            if (random < 26)
            {
                // letter
                id += (char)(random + 65);
            }
            else
            {
                // remove 26 because we want a number between 0-9
                id += (random - 26).ToString();
            }
        }
        ServerLog.Log(ServerLog.LogType.Debug, $"Random match ID: {id}");
        Debug.Log($"Random match ID: {id}");
        return id;
    }

    public bool CancelLobby(string matchId)
    {
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].MatchId == matchId)
            {
                // TODO: delete the matchmanager

                // remove the match
                matchIds.Remove(matchId);
                matches.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public void DisconnectPlayer(PlayerNetworking player, string matchId)
    {
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].MatchId == matchId)
            {
                // found matching match
                int playerIndex = matches[i].Players.IndexOf(player.gameObject);
                matches[i].Players.RemoveAt(playerIndex);
                ServerLog.Log(ServerLog.LogType.Warn, $"{player} - Disconnected from match {matchId}. Terminating Game");
                // TODO: Handle win logic for player whom didn't disconnect
                foreach (var playerRemaining in matches[i].Players)
                {
                    var networkPlayer = playerRemaining.GetComponent<PlayerNetworking>();
                    ServerLog.Log(ServerLog.LogType.Warn, $"{networkPlayer} - Disconnected from match {matchId}.");
                    networkPlayer.TerminateMatch();
                }
                matches.RemoveAt(i);
                matchIds.Remove(matchId);

                if (matchManagers.Count > 0)
                {
                    if (matchManagers.TryGetValue(matchId, out MatchManager matchManager))
                    {
                        NetworkServer.Destroy(matchManager.gameObject);
                        matchManagers.Remove(matchId);
                    }
                }

                break;
            }
        }
    }

    public void BeginMatch(string matchId)
    {
        GameObject matchManager = Instantiate(matchManagerPrefab);
        // spawn on all clients
        NetworkServer.Spawn(matchManager);
        // this match manager will only apply to the players that have this matchid
        matchManager.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();
        MatchManager manager = matchManager.GetComponent<MatchManager>();

        GameObject pathfinding = Instantiate(pathfindingPrefab);
        NetworkServer.Spawn(pathfinding);
        pathfinding.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();
        int playerPos = 1;

        Vector3 targetBase1 = new Vector3(-6.71f, 1.2f, -14.016f);
        Vector3 targetBase2 = new Vector3(6.86f, 1.2f, 14.016f);

        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].MatchId == matchId)
            {
                if(!matchManagers.ContainsKey(matchId))
                {
                    matchManagers.Add(matchId, manager);
                    ServerLog.Log(ServerLog.LogType.Debug, $"Added Match Manager to MatchID: {matchId}");
                }
                // this is the match we need
                foreach (var player in matches[i].Players)
                {
                    PlayerNetworking playerNetwork = player.GetComponent<PlayerNetworking>();
                    // add this player to this specific match manager
                    ServerLog.Log(ServerLog.LogType.Debug, $"Added {playerNetwork.name} to match {playerNetwork.MatchId}.");
                    manager.AddPlayer(playerNetwork);
                    // tell each player to start the match
                    playerNetwork.StartMatch();
                    playerNetwork.playerPos = playerPos;
                    
                    playerPos++;
                }
                break;
            }
        }

        GameObject playerBase = Instantiate(basePrefab, targetBase1, Quaternion.identity);
        NetworkServer.Spawn(playerBase);
        playerBase.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();
    
        GameObject playerBase2 = Instantiate(basePrefab, targetBase2, Quaternion.identity);
        NetworkServer.Spawn(playerBase2);
        playerBase2.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();

        GameObject unitTest = Instantiate(unitPrefab, new Vector3(0.0f, 1.2f, 0.0f), Quaternion.identity);
        NetworkServer.Spawn(unitTest);
        unitTest.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();
        Unit unitScript = unitTest.GetComponent<Unit>();
        unitScript.pathfinder = pathfinding.GetComponent<Pathfinding>();
        unitScript.target = playerBase;
        unitScript.Init();
    }
}

public static class MatchExtensions
{
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }
}
