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

    /** MATCH UI **/
    [Header("Match UI")]
    [SerializeField] GameObject matchUIPanel;
    [SerializeField] Transform playerNameContainer;
    [SerializeField] GameObject playerNamePrefab;

    void Start()
    {
        instance = this;
    }

    public void SearchForMatch()
    {
        searchButton.interactable = false;
        PlayerNetworking.localPlayer.SearchForMatch();
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
            searchButton.interactable = false;
            SpawnPlayerName(PlayerNetworking.localPlayer);
        }
        else
        {
            searchButton.interactable = true;
        }
    }

    public void JoinSuccess(bool success)
    {
        if (success)
        {
            searchButton.interactable = false;
            SpawnPlayerName(PlayerNetworking.localPlayer);
        }
        else
        {
            searchButton.interactable = true;
        }
    }

    public void SpawnPlayerName(PlayerNetworking player)
    {
        GameObject newNameUI = Instantiate(playerNamePrefab, playerNameContainer);
        newNameUI.GetComponent<TextMeshProUGUI>().text = player.Name;
    }

    public void LoadMatchUI()
    {
        lobbyUIPanel.SetActive(false);
        matchUIPanel.SetActive(true);
    }
}
