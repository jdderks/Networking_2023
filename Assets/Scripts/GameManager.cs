using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    //Simple singleton
    public static GameManager Instance { get; private set; }

    public UIManager uiManager;
    public TeamManager teamManager;
    public TileGrid grid;

    public Server server;
    public Client client;


    public void StartHosting()
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8007);
    }

    public void Connect()
    {
        client.Init(uiManager.AdressInput.text, 8007);
    }


    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void ActivateConnectedPanel()
    {
        uiManager.OnlinePanel.gameObject.SetActive(false);
        uiManager.ConnectedPanel.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        uiManager.HostPanel.SetActive(false);
        uiManager.ConnectedPanel.SetActive(false);
        uiManager.OnlinePanel.SetActive(false);
        grid.CreateTileGrid();
        
        //NetNextTeam nnt = new();
        //var startTeam = Team.Red;//Random.value > 0.5f ? (int)Team.Red : (int)Team.Blue;
        //nnt.team = (int)startTeam;
        //Client.Instance.SendToServer(nnt);

    }
}
