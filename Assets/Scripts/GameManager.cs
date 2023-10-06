using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    //Simple singleton
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject onlinePanel;
    [SerializeField] private GameObject connectedPanel;

    [SerializeField] private TMP_InputField adressInput;
    [SerializeField] private Image assignedColorImage;
    public Image AssignedColorImage { get => assignedColorImage; set => assignedColorImage = value; }


    public TeamManager teamManager;

    public Server server;
    public Client client;


    public void StartHosting()
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8007);
    }    

    public void Connect()
    {
        client.Init(adressInput.text, 8007);
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

    internal void ActivateConnectedPanel()
    {
        onlinePanel.gameObject.SetActive(false);
        connectedPanel.gameObject.SetActive(true);
    }
}
