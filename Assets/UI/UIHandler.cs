using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance;

    /** MAIN LOBBY UI **/
    [Header("Lobby UI")]
    [SerializeField] GameObject lobbyUIPanel;
    [SerializeField] Button searchButton;
    [SerializeField] Button setName;
    [SerializeField] TextMeshProUGUI nameField;
    [SerializeField] GameObject nameFieldParent;
    [SerializeField] TextMeshProUGUI playerName;

    /** SEARCHING UI**/
    [SerializeField] GameObject SearchingUIPanel;

    /** MATCH UI **/
    [Header("Match UI")]
    [SerializeField] GameObject matchUIPanel;
    [SerializeField] Transform playerNameContainer;
    [SerializeField] GameObject playerNamePrefab;

    GameObject playerNameUI;

    void Start()
    {
        instance = this;
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
        setName.gameObject.SetActive(false);
        nameFieldParent.gameObject.SetActive(false);
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
}
