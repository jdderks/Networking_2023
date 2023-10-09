using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject onlinePanel;
    [SerializeField] private GameObject connectedPanel;
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private TextMeshProUGUI currentTurnText;

    [SerializeField] private TMP_InputField adressInput;
    [SerializeField] private Image assignedColorImage;
    public Image AssignedColorImage { get => assignedColorImage; set => assignedColorImage = value; }
    public GameObject HostPanel { get => hostPanel; set => hostPanel = value; }
    public GameObject ConnectedPanel { get => connectedPanel; set => connectedPanel = value; }
    public GameObject OnlinePanel { get => onlinePanel; set => onlinePanel = value; }
    public TMP_InputField AdressInput { get => adressInput; set => adressInput = value; }
    public TextMeshProUGUI CurrentTurnText { get => currentTurnText; set => currentTurnText = value; }
}
