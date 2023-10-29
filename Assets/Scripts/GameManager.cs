using UnityEngine;
using Unity.Networking.Transport;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    //Simple singleton
    private bool loggedIn = false;

    public static GameManager Instance { get; private set; }
    public bool LoggedIn
    {
        get => loggedIn;
        set
        {
            loggedIn = value;
            uiManager.ConnectButton.interactable = value;
            uiManager.HostButton.interactable = value;
        }
    }

    public UIManager uiManager;
    public TeamManager teamManager;
    public TileGrid grid;
    public WebManager webManager;

    public Server server;
    public Client client;

    public bool gamePlayable = false;


    public void StartHosting()
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8007);

        webManager.StartSession();
    }

    public void Connect()
    {
        client.Init(uiManager.AdressInput.text, 8007);
        webManager.ConnectToSession(uiManager.SessionInputField.text);
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
        RegisterEvents();
    }
    private void OnDestroy()
    {
        UnregisterEvents();
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


    private void RegisterEvents()
    {
        NetUtility.S_PLAYER_WIN += OnPlayerWinServer;

        NetUtility.C_PLAYER_WIN += OnPlayerWinClient;

    }

    private void UnregisterEvents()
    {
        NetUtility.S_PLAYER_WIN -= OnPlayerWinServer;

        NetUtility.C_PLAYER_WIN -= OnPlayerWinClient;
    }

    public void OnPlayerWinClient(NetMessage msg)
    {
        NetPlayerWin playerWinNM = msg as NetPlayerWin;
        Team teamThatWon = (Team)playerWinNM.teamWon;
        uiManager.DisplayWonPanel(teamThatWon);

        if (teamThatWon == GameManager.Instance.teamManager.CurrentTeam)
        {
            GameManager.Instance.webManager.SendScoreToServer();
        }

        grid.RemoveTileGrid();
        gamePlayable = false;
    }

    public void OnPlayerWinServer(NetMessage msg, NetworkConnection cnn)
    {
        NetPlayerWin playerWinNM = msg as NetPlayerWin;
        Server.Instance.Broadcast(playerWinNM);
    }

    public void CloseGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }
}
