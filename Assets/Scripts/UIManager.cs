using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

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
        //disable and enable correct items
    }

    public void SwitchToLoginUI()
    {
        currentUiState = UIState.Login;
        //disable and enable correct items
    }
    public void SwitchToRegisterUI()
    {
        currentUiState = UIState.Register;
        //disable and enable correct items
    }


    public void DisplayWonPanel(Team team)
    {

        wonPanel.SetActive(true);
        wonPanelText.text = team.ToString() + " has won!";
    }
}
