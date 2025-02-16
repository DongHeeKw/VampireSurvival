using UnityEngine;
using System;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    // Player Data
    private PlayerData playerData;
    
    // Game Settings
    private GameSettings gameSettings;

    private const string PLAYER_DATA_KEY = "CUBE_PLAYER_DATA";
    private const string SETTINGS_KEY = "CUBE_SETTINGS";

    [Serializable]
    public class PlayerData
    {
        public int currentLevel;
        public float playTime;
        public int totalKills;
        public int highestWave;
        public List<CubeData> unlockedCubes;
        public int currentExp;
        public int gold;
    }

    [Serializable]
    public class CubeData
    {
        public string cubeId;
        public int level;
        public string attribute;
        public bool isUnlocked;
    }

    [Serializable]
    public class GameSettings
    {
        public float masterVolume = 1f;
        public float sfxVolume = 1f;
        public float bgmVolume = 1f;
        public bool vibrationEnabled = true;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllData()
    {
        // Load Player Data
        string playerDataJson = PlayerPrefs.GetString(PLAYER_DATA_KEY, "");
        if (string.IsNullOrEmpty(playerDataJson))
        {
            playerData = new PlayerData
            {
                currentLevel = 1,
                playTime = 0f,
                totalKills = 0,
                highestWave = 0,
                unlockedCubes = new List<CubeData>(),
                currentExp = 0,
                gold = 0
            };
        }
        else
        {
            playerData = JsonUtility.FromJson<PlayerData>(playerDataJson);
        }

        // Load Settings
        string settingsJson = PlayerPrefs.GetString(SETTINGS_KEY, "");
        if (string.IsNullOrEmpty(settingsJson))
        {
            gameSettings = new GameSettings();
        }
        else
        {
            gameSettings = JsonUtility.FromJson<GameSettings>(settingsJson);
        }
    }

    public void SaveAllData()
    {
        string playerDataJson = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString(PLAYER_DATA_KEY, playerDataJson);

        string settingsJson = JsonUtility.ToJson(gameSettings);
        PlayerPrefs.SetString(SETTINGS_KEY, settingsJson);
        
        PlayerPrefs.Save();
    }

    // Player Data Methods
    public void AddExp(int amount)
    {
        playerData.currentExp += amount;
        SaveAllData();
    }

    public void AddGold(int amount)
    {
        playerData.gold += amount;
        SaveAllData();
    }

    public void UnlockCube(string cubeId, string attribute)
    {
        CubeData newCube = new CubeData
        {
            cubeId = cubeId,
            level = 1,
            attribute = attribute,
            isUnlocked = true
        };
        
        playerData.unlockedCubes.Add(newCube);
        SaveAllData();
    }

    public void UpdatePlayTime(float sessionTime)
    {
        playerData.playTime += sessionTime;
        SaveAllData();
    }

    // Settings Methods
    public void SetMasterVolume(float volume)
    {
        gameSettings.masterVolume = Mathf.Clamp01(volume);
        SaveAllData();
    }

    public void SetSFXVolume(float volume)
    {
        gameSettings.sfxVolume = Mathf.Clamp01(volume);
        SaveAllData();
    }

    public void SetBGMVolume(float volume)
    {
        gameSettings.bgmVolume = Mathf.Clamp01(volume);
        SaveAllData();
    }

    public void SetVibration(bool enabled)
    {
        gameSettings.vibrationEnabled = enabled;
        SaveAllData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveAllData();
        }
    }

    private void OnApplicationQuit()
    {
        SaveAllData();
    }
}