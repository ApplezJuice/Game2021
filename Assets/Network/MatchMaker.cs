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
            Debug.Log($"<color=green>Match Generated!</color>");
            return true;
        }

        Debug.Log($"<color=red>Match ID Already Exists!</color>");
        return false;
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
        Debug.Log($"Random match ID: {id}");
        return id;
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
