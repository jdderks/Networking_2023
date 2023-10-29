using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Team
{
    None = 0,
    Red = 1,
    Blue = 2//,
    //Yellow = 3,
    //Green = 4
}

public class TeamManager : MonoBehaviour
{
    private int playerCount = -1;

    [SerializeField] private Team teamPlayingAs = Team.None;
    [SerializeField] private Team currentTeamTurn = Team.None;

    [SerializeField] private int amountOfTurns = 0;

    public Team CurrentTeam { get => teamPlayingAs; set => teamPlayingAs = value; }
    public Team CurrentTeamTurn { get => currentTeamTurn; set => currentTeamTurn = value; }
    public int AmountOfTurns { get => amountOfTurns; set => amountOfTurns = value; }

    public static Color GetTeamColor(Team team)
    {
        switch (team)
        {
            case Team.None:
                Debug.LogWarning("Isn't connected to anything.");
                return Color.black;
            case Team.Red:
                return Color.red;
            case Team.Blue:
                return Color.blue;
            //case Team.Yellow:
            //    return Color.yellow;
            //case Team.Green:
            //    return Color.green;
            default:
                return Color.white;
        }
    }


    #region Networking Related
    private void Awake()
    {
        RegisterEvents();
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    public void StartGame() //Referenced in Unity Inspector
    {
        var startGameNM = new NetStartGame();
        Client.Instance.SendToServer(startGameNM);

        var assignTeamNM = new NetAssignTeam();
        //assignTeamNM.randomTeam = true;
        assignTeamNM.teamToAssign = (int)Team.None;
        Client.Instance.SendToServer(assignTeamNM);
    }

    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;

        NetUtility.C_WELCOME += OnWelcomeClient;

        NetUtility.C_START_GAME += OnStartGameClient;

        NetUtility.S_START_GAME += OnStartGameServer;

        NetUtility.C_ASSIGN_TEAM += OnAssignTeamClient;

        NetUtility.S_ASSIGN_TEAM += OnAssignTeamServer;
    }



    private void UnregisterEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;

        NetUtility.C_WELCOME -= OnWelcomeClient;

        NetUtility.C_START_GAME -= OnStartGameClient;

        NetUtility.S_START_GAME -= OnStartGameServer;

        NetUtility.C_ASSIGN_TEAM -= OnAssignTeamClient;

        NetUtility.S_ASSIGN_TEAM -= OnAssignTeamServer;
    }

    //Server
    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
        //Client has connected, assign a team and return
        NetWelcome welcome = msg as NetWelcome;
        if (playerCount == -1) playerCount = 0;
        welcome.AssignedTeam = ++playerCount;

        if (Server.Instance.IsActive)
        {
            int connectionAmount = Server.Instance.AmountOfConnections;
            GameManager.Instance.uiManager.WaitingOrConnectedText.text = "Clients connected: " + connectionAmount;
        }

        Server.Instance.SendToClient(cnn, welcome);
    }

    private void OnStartGameServer(NetMessage msg, NetworkConnection cnn)
    {
        NetStartGame startGame = msg as NetStartGame;

        Server.Instance.Broadcast(startGame);
    }

    private void OnAssignTeamServer(NetMessage msg, NetworkConnection cnn)
    {
        var nat = msg as NetAssignTeam;
        if (nat.teamToAssign == (int)Team.None) //If assign team is null, select a random team
        {
            nat.teamToAssign = Random.value < 0.5f ? (int)Team.Red : (int)Team.Blue;
            Server.Instance.Broadcast(nat);
        }
        else
        {
            Server.Instance.Broadcast(nat);
        }
    }

    //Client
    private void OnWelcomeClient(NetMessage msg)
    {
        NetWelcome nw = msg as NetWelcome;

        var assignedTeam = nw.AssignedTeam;
        teamPlayingAs = (Team)assignedTeam;
        GameManager.Instance.ActivateConnectedPanel();
        GameManager.Instance.uiManager.AssignedColorImage.color = GetTeamColor(teamPlayingAs);
        Debug.Log($"My assigned team = {(Team)nw.AssignedTeam}");
    }

    private void OnStartGameClient(NetMessage msg)
    {
        //Here is what happens when the game starts on the client
        NetStartGame nsg = msg as NetStartGame;
        GameManager.Instance.gamePlayable = true;
        GameManager.Instance.StartGame();
    }

    private void OnAssignTeamClient(NetMessage msg)
    {
        Debug.Log("NetAssignTeam message received");
        var nat = msg as NetAssignTeam;
        CurrentTeamTurn = (Team)nat.teamToAssign;
        GameManager.Instance.uiManager.CurrentTurnText.gameObject.SetActive(true);
        GameManager.Instance.uiManager.CurrentTurnText.text = currentTeamTurn.ToString();
    }
    #endregion

}

