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

    [SerializeField] GameObject matchManagerPrefab;

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

    public void BeginMatch(string matchId)
    {
        GameObject matchManager = Instantiate(matchManagerPrefab);
        // spawn on all clients
        NetworkServer.Spawn(matchManager);
        // this match manager will only apply to the players that have this matchid
        matchManager.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();
        MatchManager manager = matchManager.GetComponent<MatchManager>();

        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].MatchId == matchId)
            {
                // this is the match we need
                foreach (var player in matches[i].Players)
                {
                    PlayerNetworking playerNetwork = player.GetComponent<PlayerNetworking>();
                    // add this player to this specific match manager
                    ServerLog.Log(ServerLog.LogType.Debug, $"Added {playerNetwork.name} to match {playerNetwork.MatchId}");
                    manager.AddPlayer(playerNetwork);
                    // tell each player to start the match
                    playerNetwork.StartMatch();
                }
                break;
            }
        }
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
