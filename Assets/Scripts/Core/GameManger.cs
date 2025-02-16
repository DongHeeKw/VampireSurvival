using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Game State
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public GameState CurrentGameState { get; private set; }
    
    // Game Progress
    public float GameTime { get; private set; }
    public int CurrentWave { get; private set; }
    public int KillCount { get; private set; }
    
    // Events
    public event Action<GameState> OnGameStateChanged;
    public event Action<int> OnWaveChanged;
    public event Action<int> OnKillCountChanged;
    public event Action OnGameOver;

    // Difficulty Settings
    [SerializeField] private float difficultyScalingFactor = 1.1f;
    [SerializeField] private float waveInterval = 30f;
    private float nextWaveTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        CurrentGameState = GameState.MainMenu;
        GameTime = 0f;
        CurrentWave = 0;
        KillCount = 0;
        nextWaveTime = waveInterval;
    }

    private void Update()
    {
        if (CurrentGameState != GameState.Playing) return;

        GameTime += Time.deltaTime;

        // Wave System
        if (GameTime >= nextWaveTime)
        {
            StartNextWave();
        }
    }

    public void StartGame()
    {
        InitializeGame();
        ChangeGameState(GameState.Playing);
        StartCoroutine(DifficultyScaling());
    }

    public void PauseGame()
    {
        if (CurrentGameState == GameState.Playing)
        {
            ChangeGameState(GameState.Paused);
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        if (CurrentGameState == GameState.Paused)
        {
            ChangeGameState(GameState.Playing);
            Time.timeScale = 1f;
        }
    }

    public void EndGame()
    {
        ChangeGameState(GameState.GameOver);
        Time.timeScale = 0f;
        
        // Save progress
        DataManager.Instance.UpdatePlayTime(GameTime);
        
        OnGameOver?.Invoke();
    }

    private void StartNextWave()
    {
        CurrentWave++;
        nextWaveTime = GameTime + waveInterval;
        OnWaveChanged?.Invoke(CurrentWave);
    }

    public void AddKill()
    {
        KillCount++;
        OnKillCountChanged?.Invoke(KillCount);
    }

    private void ChangeGameState(GameState newState)
    {
        CurrentGameState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    private IEnumerator DifficultyScaling()
    {
        while (CurrentGameState == GameState.Playing)
        {
            yield return new WaitForSeconds(60f); // Adjust difficulty every minute
            difficultyScalingFactor *= 1.1f; // Increase difficulty by 10%
        }
    }

    // Get current difficulty multiplier for enemies
    public float GetCurrentDifficulty()
    {
        return difficultyScalingFactor * (1f + CurrentWave * 0.1f);
    }
    
    private void OnApplicationQuit()
    {
        if (CurrentGameState == GameState.Playing)
        {
            DataManager.Instance.UpdatePlayTime(GameTime);
        }
    }
}