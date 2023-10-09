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
    Blue = 2,
    Yellow = 3,
    Green = 4
}

public class TeamManager : MonoBehaviour
{
    private int playerCount = -1;

    [SerializeField] private Team teamPlayingAs = Team.None;

    public Team CurrentTeam
    {
        get => teamPlayingAs;
        set => teamPlayingAs = value;
    }

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
            case Team.Yellow:
                return Color.yellow;
            case Team.Green:
                return Color.green;
            default:
                return Color.white;
        }
    }

    public void SetNextTeamTurn()
    {
        //if (currentTeamTurn == Team.Red)
        //{
        //    currentTeamTurn = Team.Blue;
        //}
        //else if (currentTeamTurn == Team.Blue)
        //{
        //    currentTeamTurn = Team.Blue;
        //}
        ////Below code is if I want more than 2 clients to be able to connect, requires rewrite.
        //int currentTeamValue = (int)currentTeamTurn;
        //int totalTeams = Enum.GetValues(typeof(Team)).Length - 1;

        //int nextTeamValue = (currentTeamValue % totalTeams) + 1;

        //currentTeamTurn = (Team)nextTeamValue;
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

    public void StartGame()
    {
        var startGameNM = new NetStartGame();
        Client.Instance.SendToServer(startGameNM);
    }

    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;

        NetUtility.C_WELCOME += OnWelcomeClient;

        NetUtility.C_START_GAME += OnStartGameClient;

        NetUtility.S_START_GAME += OnStartGameServer;
    }



    private void UnregisterEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;

        NetUtility.C_WELCOME -= OnWelcomeClient;

        NetUtility.C_START_GAME -= OnStartGameClient;

        NetUtility.S_START_GAME -= OnStartGameServer;
    }

    //Server
    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
        //Client has connected, assign a team and return
        NetWelcome welcome = msg as NetWelcome;
        if (playerCount == -1) playerCount = 0;
        welcome.AssignedTeam = ++playerCount;

        Server.Instance.SendToClient(cnn, welcome);
    }

    private void OnStartGameServer(NetMessage msg, NetworkConnection cnn)
    {
        NetStartGame startGame = msg as NetStartGame;

        Server.Instance.Broadcast(startGame);
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
        GameManager.Instance.StartGame();
    }
    
    #endregion

}

