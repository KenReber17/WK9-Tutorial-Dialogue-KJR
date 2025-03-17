using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int score = 0;
    public TextMeshProUGUI scoreText; // Drag your UI Text element here

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this object when loading a new scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreDisplay();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreDisplay();
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt("PlayerScore", score);
    }

    public void LoadScore()
    {
        score = PlayerPrefs.GetInt("PlayerScore", 0); // Default to 0 if no score saved
        UpdateScoreDisplay();
    }

    // New method to handle diamond pickup
    public void OnDiamondPickup()
    {
        AddScore(20); // Adds 20 points when a diamond is picked up
    }
}