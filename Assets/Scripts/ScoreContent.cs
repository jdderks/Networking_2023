using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreContent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public TextMeshProUGUI PlayerNameText { get => playerNameText; set => playerNameText = value; }
    public TextMeshProUGUI ScoreText { get => scoreText; set => scoreText = value; }
    public TextMeshProUGUI NumberText { get => numberText; set => numberText = value; }
}
