using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance;

    [SerializeField] Button searchButton;

    void Start()
    {
        instance = this;
    }

    public void SearchForMatch()
    {
        searchButton.interactable = false;
        PlayerNetworking.localPlayer.SearchForMatch();
    }

    public void HostSuccess(bool success)
    {
        if(success)
        {
            searchButton.interactable = false;
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
        }
        else
        {
            searchButton.interactable = true;
        }
    }
}
