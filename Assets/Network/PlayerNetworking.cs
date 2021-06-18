using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetworking : NetworkBehaviour
{
    public static PlayerNetworking localPlayer;
    [SyncVar] public string MatchId;
    [SyncVar] public string Name;
    NetworkMatchChecker networkMatchChecker;

    [SyncVar] public bool inGame;
    [SyncVar] public int gold;

    GameObject enemyPlayerNameUI;

    void Awake()
    {
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isLocalPlayer)
        {
            localPlayer = this;
        }
        else
        {
            // not local player stuff will happen here. (Spawn in other player avatar in UI etc.)
            enemyPlayerNameUI = UIHandler.instance.SpawnPlayerName(this);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        ClientDisconnect();
        
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        ServerDisconnect();
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

    public void CancelSearch()
    {
        CmdCancelSearch();
    }

    [Command]
    void CmdCancelSearch()
    {
        if (!string.IsNullOrEmpty(MatchId))
        {
            // is host, remove match lobby
            if (MatchMaker.instance.CancelLobby(MatchId))
            {
                // removed lobby
                ServerLog.Log(ServerLog.LogType.Warn, $"Canceled match search!");
                MatchId = string.Empty;
                networkMatchChecker.matchId = string.Empty.ToGuid();
                TargetReturnToLobby();
            }
            else
            {
                ServerLog.Log(ServerLog.LogType.Error, $"Could not cancel search..");
            }
        }
    }

    [TargetRpc]
    void TargetReturnToLobby()
    {
        UIHandler.instance.ReturnToLobby();
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
        // load match scene
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
        CmdPlayerInGame();
    }

    [Command]
    void CmdPlayerInGame()
    {
        inGame = true;
        TargetLoadMatchUI();
    }

    [TargetRpc]
    void TargetLoadMatchUI()
    {
        UIHandler.instance.LoadMatchUI();
    }

    /* 
     * DISCONNECT 
     */
    public void DisconnectMatch()
    {
        CmdDisconnectMatch();
    }

    public void TerminateMatch()
    {
        TargetTerminateMatch();
    }

    [TargetRpc]
    void TargetTerminateMatch()
    {
        UIHandler.instance.MatchTerminated();
        CmdNotInGame();
    }

    [Command]
    void CmdNotInGame()
    {
        inGame = false;
    }

    [Command]
    void CmdDisconnectMatch()
    {
        inGame = false;
        ServerDisconnect();
    }

    void ServerDisconnect()
    {
        MatchMaker.instance.DisconnectPlayer(this, MatchId);
        networkMatchChecker.matchId = string.Empty.ToGuid();
        RpcDisconnectMatch();
    }

    [ClientRpc]
    void RpcDisconnectMatch()
    {
        ClientDisconnect();
    }

    void ClientDisconnect()
    {
        if (enemyPlayerNameUI)
            Destroy(enemyPlayerNameUI);
    }

    public void AddGold()
    {
        CmdAddPlayerGold();
    }

    [Command]
    void CmdAddPlayerGold()
    {
        gold += 10;
        ServerLog.Log(ServerLog.LogType.Warn, $"Added gold. Total: {gold}");
        TargetUpdateGold(gold.ToString());
    }

    [TargetRpc]
    void TargetUpdateGold(string gold)
    {
        UIHandler.instance.UpdateGoldUI($"Gold: {gold}");
    }
}
