using UnityEngine;
using System;
using System.Collections.Generic;

public class CubeStats : MonoBehaviour
{
    [System.Serializable]
    public class Stat
    {
        public string name;
        public float baseValue;
        public float currentValue;
        public float maxValue = 999f;
        public int level;
        public float growthRate = 1.1f;
        public float upgradePrice = 100f;

        public float CalculateUpgradePrice()
        {
            return upgradePrice * Mathf.Pow(growthRate, level);
        }
    }

    [Header("Base Stats")]
    [SerializeField] private Stat maxHealth = new Stat { name = "Max Health", baseValue = 100f };
    [SerializeField] private Stat attack = new Stat { name = "Attack", baseValue = 10f };
    [SerializeField] private Stat defense = new Stat { name = "Defense", baseValue = 5f };
    [SerializeField] private Stat speed = new Stat { name = "Speed", baseValue = 5f };
    
    [Header("Current Status")]
    [SerializeField] private float currentHealth;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float experience = 0f;
    
    // Components
    private CubeAttribute cubeAttribute;
    private CubeEvolution cubeEvolution;

    // Events
    public event Action<float, float> OnHealthChanged; // current, max
    public event Action<int> OnLevelUp;
    public event Action<float> OnExperienceGained;
    public event Action<string, float> OnStatUpgraded;
    public event Action OnDeath;

    private void Awake()
    {
        cubeAttribute = GetComponent<CubeAttribute>();
        cubeEvolution = GetComponent<CubeEvolution>();
        InitializeStats();
    }

    private void Start()
    {
        LoadStats();
        currentHealth = maxHealth.currentValue;
    }

    private void InitializeStats()
    {
        // Initialize current values
        maxHealth.currentValue = maxHealth.baseValue;
        attack.currentValue = attack.baseValue;
        defense.currentValue = defense.baseValue;
        speed.currentValue = speed.baseValue;
    }

    private void LoadStats()
    {
        // Load saved stat levels
        maxHealth.level = PlayerPrefs.GetInt("Stat_Health_Level", 1);
        attack.level = PlayerPrefs.GetInt("Stat_Attack_Level", 1);
        defense.level = PlayerPrefs.GetInt("Stat_Defense_Level", 1);
        speed.level = PlayerPrefs.GetInt("Stat_Speed_Level", 1);

        // Recalculate current values based on levels
        RecalculateStats();
    }

    private void SaveStats()
    {
        PlayerPrefs.SetInt("Stat_Health_Level", maxHealth.level);
        PlayerPrefs.SetInt("Stat_Attack_Level", attack.level);
        PlayerPrefs.SetInt("Stat_Defense_Level", defense.level);
        PlayerPrefs.SetInt("Stat_Speed_Level", speed.level);
        PlayerPrefs.Save();
    }

    public void RecalculateStats()
    {
        // Get multipliers from attribute and evolution
        float attributeMultiplier = cubeAttribute ? cubeAttribute.GetDamageMultiplier() : 1f;
        float evolutionMultiplier = cubeEvolution ? cubeEvolution.GetCurrentStatMultiplier() : 1f;

        // Calculate each stat with level, attribute, and evolution bonuses
        maxHealth.currentValue = CalculateStatValue(maxHealth);
        attack.currentValue = CalculateStatValue(attack) * attributeMultiplier;
        defense.currentValue = CalculateStatValue(defense) * evolutionMultiplier;
        speed.currentValue = CalculateStatValue(speed) * cubeAttribute.GetSpeedMultiplier();

        // Ensure current health doesn't exceed new max health
        currentHealth = Mathf.Min(currentHealth, maxHealth.currentValue);
        
        // Notify health change
        OnHealthChanged?.Invoke(currentHealth, maxHealth.currentValue);
    }

    private float CalculateStatValue(Stat stat)
    {
        return stat.baseValue * Mathf.Pow(stat.growthRate, stat.level - 1);
    }

    public void UpgradeStat(string statName)
    {
        Stat stat = GetStatByName(statName);
        if (stat == null) return;

        float price = stat.CalculateUpgradePrice();
        // TODO: Check if player has enough currency to upgrade
        
        stat.level++;
        RecalculateStats();
        SaveStats();
        
        OnStatUpgraded?.Invoke(statName, stat.currentValue);
    }

    private Stat GetStatByName(string statName)
    {
        switch (statName.ToLower())
        {
            case "health":
                return maxHealth;
            case "attack":
                return attack;
            case "defense":
                return defense;
            case "speed":
                return speed;
            default:
                return null;
        }
    }

    public void TakeDamage(float damage)
    {
        float actualDamage = Mathf.Max(1, damage - defense.currentValue);
        currentHealth = Mathf.Max(0, currentHealth - actualDamage);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth.currentValue);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth.currentValue, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth.currentValue);
    }

    public void GainExperience(float amount)
    {
        experience += amount;
        OnExperienceGained?.Invoke(experience);

        // Check for level up
        float requiredExp = CalculateRequiredExperience();
        while (experience >= requiredExp)
        {
            LevelUp();
            experience -= requiredExp;
            requiredExp = CalculateRequiredExperience();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        OnLevelUp?.Invoke(currentLevel);
        
        // Apply level up bonuses
        maxHealth.baseValue *= 1.1f;
        attack.baseValue *= 1.1f;
        defense.baseValue *= 1.1f;
        
        RecalculateStats();
        
        // Heal on level up
        currentHealth = maxHealth.currentValue;
        OnHealthChanged?.Invoke(currentHealth, maxHealth.currentValue);
    }

    private float CalculateRequiredExperience()
    {
        // Experience curve: each level requires more experience
        return 100f * Mathf.Pow(1.2f, currentLevel - 1);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        // Additional death logic
    }

    // Getter methods for UI and other components
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth.currentValue;
    public float GetAttack() => attack.currentValue;
    public float GetDefense() => defense.currentValue;
    public float GetSpeed() => speed.currentValue;
    public int GetLevel() => currentLevel;
    public float GetExperience() => experience;
    public float GetRequiredExperience() => CalculateRequiredExperience();
}