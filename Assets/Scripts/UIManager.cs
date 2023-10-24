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
    Login = 2
}


public class UIManager : MonoBehaviour
{
    [Header("Object references")]
    [SerializeField] private GameObject onlinePanel;
    [SerializeField] private GameObject connectedPanel;
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private GameObject wonPanel;
    [SerializeField] private TextMeshProUGUI wonPanelText;
    [SerializeField] private TextMeshProUGUI currentTurnText;
    [SerializeField] private TextMeshProUGUI waitingOrConnectedText;
    [SerializeField] private TMP_InputField adressInput;
    [SerializeField] private Image assignedColorImage;

    [Header("Register related fields")]
    [SerializeField] public TMP_InputField usernameInputField;
    [SerializeField] public TMP_InputField passwordInputField;
    [SerializeField] public TMP_InputField mailadressInputField;

    [SerializeField] public TMP_InputField birthDayInputField;
    [SerializeField] public TMP_InputField birthMonthInputField;
    [SerializeField] public TMP_InputField birthYearInputField;

    [SerializeField] public Toggle anonymousBox;

    [SerializeField] public TextMeshProUGUI registrationStatusText;

    //These objects with be swapped between UIstates
    [Space, SerializeField] private GameObject registerObjects;
    [SerializeField] private GameObject loginObjects;
    [SerializeField] private GameObject otherObjects;

    [Header("UI States")]
    [SerializeField] private UIState currentUiState = UIState.Main;

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

    public void SwitchToMainUI()
    {
        currentUiState = UIState.Main;
        otherObjects.SetActive(true);
        registerObjects.SetActive(false);
        loginObjects.SetActive(false);
    }

    public void SwitchToLoginUI()
    {
        currentUiState = UIState.Login;
        loginObjects.SetActive(true);
        otherObjects.SetActive(false);
        registerObjects.SetActive(false);
    }

    public void SwitchToRegisterUI()
    {
        currentUiState = UIState.Register;
        loginObjects.SetActive(false);
        otherObjects.SetActive(false);
        registerObjects.SetActive(true);
    }

    public void DisplayWonPanel(Team team)
    {

        wonPanel.SetActive(true);
        wonPanelText.text = team.ToString() + " has won!";
    }

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


    
}
