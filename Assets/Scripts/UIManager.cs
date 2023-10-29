using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;

public enum UIState
{
    Main = 0,
    Register = 1,
    Login = 2,
    HighScores = 3
}


public class UIManager : MonoBehaviour
{

    #region variables
    [Header("Object references")]
    [SerializeField] private GameObject onlinePanel;
    [SerializeField] private GameObject connectedPanel;
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private GameObject wonPanel;
    [SerializeField] private TextMeshProUGUI wonPanelText;
    [SerializeField] private TextMeshProUGUI amountOfTurnsWonText;
    [SerializeField] private TextMeshProUGUI currentTurnText;
    [SerializeField] private TextMeshProUGUI waitingOrConnectedText;
    [SerializeField] private TMP_InputField adressInput;
    [SerializeField] private Image assignedColorImage;
    [SerializeField] private TextMeshProUGUI sessionNameText;
    [SerializeField] private TMP_InputField sessionInputField;

    [Header("Login related fields")]
    [SerializeField] public TMP_InputField loginUsernameInputField;
    [SerializeField] public TMP_InputField loginPasswordInputField;

    [Header("Register related fields")]
    [SerializeField] public TMP_InputField registrationUsernameInputField;
    [SerializeField] public TMP_InputField registrationPasswordInputField;
    [SerializeField] public TMP_InputField mailadressInputField;

    [SerializeField] public TMP_InputField birthDayInputField;
    [SerializeField] public TMP_InputField birthMonthInputField;
    [SerializeField] public TMP_InputField birthYearInputField;

    [SerializeField] public Toggle anonymousBox;

    [SerializeField] public TextMeshProUGUI registrationStatusText;

    //These objects will be swapped between UIstates
    [Space, SerializeField] private GameObject registerObjects;
    [SerializeField] private GameObject loginObjects;
    [SerializeField] private GameObject mainUIObjects;
    [SerializeField] private GameObject highscoreObjects;

    [SerializeField] private TextMeshProUGUI currentlyLoggedInText;

    [Space, Header("Menu buttons"), SerializeField] private Button hostButton;
    [SerializeField] private Button connectButton;

    [Header("UI States")]
    [SerializeField] private UIState currentUiState = UIState.Main;


    [Space, Header("Leaderboard items"), SerializeField]
    public GameObject leaderBoardContentObject;
    public GameObject leaderBoardPrefab;
    public List<GameObject> instantiatedLeaderBoardGameObjects = new();

    #endregion

    #region properties
    public Image AssignedColorImage { get => assignedColorImage; set => assignedColorImage = value; }
    public GameObject HostPanel { get => hostPanel; set => hostPanel = value; }
    public GameObject ConnectedPanel { get => connectedPanel; set => connectedPanel = value; }
    public GameObject OnlinePanel { get => onlinePanel; set => onlinePanel = value; }
    public TMP_InputField AdressInput { get => adressInput; set => adressInput = value; }
    public TextMeshProUGUI CurrentTurnText { get => currentTurnText; set => currentTurnText = value; }
    public TextMeshProUGUI WaitingOrConnectedText { get => waitingOrConnectedText; set => waitingOrConnectedText = value; }
    public UIState CurrentUiState
    {
        get => currentUiState;
        set => currentUiState = value;
    }
    public TextMeshProUGUI CurrentlyLoggedInText { get => currentlyLoggedInText; set => currentlyLoggedInText = value; }
    public Button HostButton { get => hostButton; set => hostButton = value; }
    public Button ConnectButton { get => connectButton; set => connectButton = value; }
    public TextMeshProUGUI SessionNameText { get => sessionNameText; set => sessionNameText = value; }
    public TMP_InputField SessionInputField { get => sessionInputField; set => sessionInputField = value; }
    public TextMeshProUGUI AmountOfTurnsWonText { get => amountOfTurnsWonText; set => amountOfTurnsWonText = value; }

    #endregion

    #region UISwitching
    public void SwitchToMainUI()
    {
        currentUiState = UIState.Main;
        mainUIObjects.SetActive(true);
        registerObjects.SetActive(false);
        loginObjects.SetActive(false);
        highscoreObjects.SetActive(false);
    }

    public void SwitchToLoginUI()
    {
        currentUiState = UIState.Login;
        loginObjects.SetActive(true);
        mainUIObjects.SetActive(false);
        registerObjects.SetActive(false);
        highscoreObjects.SetActive(false);
    }

    public void SwitchToRegisterUI()
    {
        currentUiState = UIState.Register;
        registerObjects.SetActive(true);
        loginObjects.SetActive(false);
        mainUIObjects.SetActive(false);
        highscoreObjects.SetActive(false);
    }

    public void SwitchToHighScoresUI()
    {
        currentUiState = UIState.HighScores;
        highscoreObjects.SetActive(true);
        loginObjects.SetActive(false);
        mainUIObjects.SetActive(false);
        registerObjects.SetActive(false);
    }
    public void DisplayWonPanel(Team team)
    {
        wonPanel.SetActive(true);
        wonPanelText.text = team.ToString() + " has won!";
    }
    #endregion

    #region datehelper
    internal string GetDateInputs()
    {
        string birthYear = birthYearInputField.text;
        string birthMonth = birthMonthInputField.text;
        string birthDay = birthDayInputField.text;

        //Formatting fixes
        if (birthMonth.Length < 2 && birthMonth.Length > 0)
            birthMonth = "0" + birthMonth;
        if (birthDay.Length < 2 && birthDay.Length > 0)
            birthDay = "0" + birthDay;

        string stringDate = birthYear + "-" + birthMonth + "-" + birthDay;
        return stringDate;
    }
    #endregion

    #region leaderboard
    public void UpdateLeaderBoard(List<ScoreObject> ScoreObjects)
    {
        for (int i = 0; i < instantiatedLeaderBoardGameObjects.Count; i++)
        {
            Destroy(instantiatedLeaderBoardGameObjects[i]); //Destroy all objects
        }
        instantiatedLeaderBoardGameObjects = new(); //Clear the list

        for (int i = 0; i < ScoreObjects.Count; i++)
        {
            GameObject ScoreGameObject = Instantiate(leaderBoardPrefab, leaderBoardContentObject.transform);
            instantiatedLeaderBoardGameObjects.Add(ScoreGameObject);

            var scoreContent = ScoreGameObject.GetComponent<ScoreContent>();

            scoreContent.NumberText.text = i.ToString(); 
            scoreContent.PlayerNameText.text = ScoreObjects[i].name; 
            scoreContent.ScoreText.text = ScoreObjects[i].score.ToString(); 
        }
    }
    #endregion

}

[Serializable]
public struct ScoreObject
{
    public string name;
    public string score;
}