using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int score = 0;
    public int Score => score;

    [Header("Star Rating Thresholds")]
    [SerializeField] private int oneStarThreshold = 40;
    [SerializeField] private int twoStarsThreshold = 70;
    [SerializeField] private int threeStarsThreshold = 100;

    public event Action<int> OnScoreChanged; //возможно для вывода в реальном времени

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        ResetScore();
    }

    public void AddScore(int points)
    {
        score += points;
        OnScoreChanged?.Invoke(score);
    }

    public void SubtractScore(int points)
    {
        score -= points;
        if (score < 0)
            score = 0;
        OnScoreChanged?.Invoke(score);
    }

    public void ResetScore()
    {
        score = 0;
        OnScoreChanged?.Invoke(score);
    }

    public int GetStarRating()
    {
        if (score >= threeStarsThreshold)
            return 3;
        else if (score >= twoStarsThreshold)
            return 2;
        else if (score >= oneStarThreshold)
            return 1;
        else
            return 0;
    }
}