using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UIState
{
    Loadout = 0,
    Friends = 1,
    Clan = 2,
    News = 3
}

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance;

    UIState uiState {get; set;}

    /** MAIN LOBBY UI **/
    [Header("Lobby UI")]
    [SerializeField] GameObject lobbyUIPanel;
    [SerializeField] Button searchButton;
    [SerializeField] Button setName;
    [SerializeField] TextMeshProUGUI nameField;
    [SerializeField] GameObject nameFieldParent;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI playerRank;
    [SerializeField] Animator animator;

    /** SEARCHING UI**/
    [SerializeField] GameObject SearchingUIPanel;

    /** MATCH UI **/
    [Header("Match UI")]
    [SerializeField] GameObject matchUIPanel;
    [SerializeField] Transform playerNameContainer;
    [SerializeField] GameObject playerNamePrefab;
    [SerializeField] TextMeshProUGUI playerGold;


    GameObject playerNameUI;

    void Start()
    {
        instance = this;
    }

    void SetUIState(UIState state)
    {
        if (uiState == state) { return; }

        var previousState = uiState;
        previousState = state;

        switch(state)
        {
            case UIState.Loadout:
                // slide to the right
            break;
            case UIState.Friends:
                if (previousState == UIState.Clan || previousState == UIState.News)
                {
                    // slide to the right
                }
                else
                {
                    // slide to the left
                }
            break;
            case UIState.Clan:
                if (previousState == UIState.Friends || previousState == UIState.Loadout)
                {
                    // slide to the left
                }
                else
                {
                    // slide to the right
                }
            break;
            case UIState.News:
                // slide to the left
            break;
        }
    }

    public void LoadoutButton()
    {
        SetUIState(UIState.Loadout);
    }

    public void FriendsButton()
    {
        SetUIState(UIState.Friends);
    }

    public void ClanButton()
    {
        SetUIState(UIState.Clan);
    }

    public void NewsButton()
    {
        SetUIState(UIState.News);
    }

    public void CloseApplication()
    {
        Application.Quit();
    }

    public void SearchForMatch()
    {
        lobbyUIPanel.SetActive(false);
        SearchingUIPanel.SetActive(true);
        PlayerNetworking.localPlayer.SearchForMatch();
    }

    public void CancelSearch()
    {
        if (playerNameUI)
            Destroy(playerNameUI);

        PlayerNetworking.localPlayer.CancelSearch();
    }

    public void ReturnToLobby()
    {
        SearchingUIPanel.SetActive(false);
        lobbyUIPanel.SetActive(true);
    }

    public void SetPlayerName()
    {
        PlayerNetworking.localPlayer.SetName(nameField.text);
        playerName.text = nameField.text;
        setName.enabled = false;
        nameField.enabled = false;
        //setName.gameObject.SetActive(false);
        //nameFieldParent.gameObject.SetActive(false);
        animator.SetBool("NameSet", true);
    }

    public void HostSuccess(bool success)
    {
        if(success)
        {
            if (playerNameUI)
                Destroy(playerNameUI);
            playerNameUI = SpawnPlayerName(PlayerNetworking.localPlayer);
        }
    }

    public void JoinSuccess(bool success)
    {
        if (success)
        {
            if (playerNameUI)
                Destroy(playerNameUI);
            playerNameUI = SpawnPlayerName(PlayerNetworking.localPlayer);
        }
    }

    public GameObject SpawnPlayerName(PlayerNetworking player)
    {
        GameObject newNameUI = Instantiate(playerNamePrefab, playerNameContainer);
        newNameUI.GetComponent<TextMeshProUGUI>().text = player.Name;
        return newNameUI;
    }

    public void LoadMatchUI()
    {
        lobbyUIPanel.SetActive(false);
        SearchingUIPanel.SetActive(false);
        matchUIPanel.SetActive(true);
    }

    public void DisconnectFromMatch()
    {
        if (playerNameUI)
            Destroy(playerNameUI);

        PlayerNetworking.localPlayer.DisconnectMatch();

        lobbyUIPanel.SetActive(true);
        SearchingUIPanel.SetActive(false);
        matchUIPanel.SetActive(false);
    }

    public void MatchTerminated()
    {
        if (playerNameUI)
            Destroy(playerNameUI);

        lobbyUIPanel.SetActive(true);
        SearchingUIPanel.SetActive(false);
        matchUIPanel.SetActive(false);
    }

    public void UpdateGoldUI(string goldText)
    {
        playerGold.text = ($"Gold: {goldText}");
    }
}
