using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private Color highHealthColor = Color.green;
    
    [Header("Experience UI")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI expText;
    
    [Header("Stats UI")]
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI speedText;
    
    [Header("Wave Info")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI killCountText;
    
    [Header("Attribute UI")]
    [SerializeField] private Image attributeIcon;
    [SerializeField] private TextMeshProUGUI attributeText;
    
    [Header("Animation")]
    [SerializeField] private Animator hudAnimator;
    [SerializeField] private float updateSpeed = 5f;

    // Components
    private CubeStats cubeStats;
    private CubeAttribute cubeAttribute;
    
    // Cached values for smooth updates
    private float targetHealth;
    private float targetExp;

    private void Start()
    {
        InitializeReferences();
        SubscribeToEvents();
        UpdateAllUI();
    }

    private void Update()
    {
        UpdateSmoothValues();
    }

    private void InitializeReferences()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            cubeStats = player.GetComponent<CubeStats>();
            cubeAttribute = player.GetComponent<CubeAttribute>();
        }
    }

    private void SubscribeToEvents()
    {
        if (cubeStats != null)
        {
            cubeStats.OnHealthChanged += UpdateHealth;
            cubeStats.OnExperienceGained += UpdateExperience;
            cubeStats.OnLevelUp += UpdateLevel;
            cubeStats.OnStatUpgraded += UpdateStats;
        }

        if (cubeAttribute != null)
        {
            cubeAttribute.OnAttributeChanged += UpdateAttribute;
        }

        GameManager.Instance.OnWaveChanged += UpdateWave;
        GameManager.Instance.OnKillCountChanged += UpdateKillCount;
    }

    private void UpdateSmoothValues()
    {
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetHealth, Time.deltaTime * updateSpeed);
        }

        if (expSlider != null)
        {
            expSlider.value = Mathf.Lerp(expSlider.value, targetExp, Time.deltaTime * updateSpeed);
        }
    }

    private void UpdateHealth(float current, float max)
    {
        targetHealth = current / max;
        
        if (healthText != null)
        {
            healthText.text = $"{Mathf.Ceil(current)}/{Mathf.Ceil(max)}";
        }

        if (healthFillImage != null)
        {
            float healthPercentage = current / max;
            healthFillImage.color = Color.Lerp(lowHealthColor, highHealthColor, healthPercentage);
        }

        // Low health warning
        if (hudAnimator != null && current / max < 0.3f)
        {
            hudAnimator.SetTrigger("LowHealth");
        }
    }

    private void UpdateExperience(float experience)
    {
        if (cubeStats == null) return;

        float maxExp = cubeStats.GetRequiredExperience();
        targetExp = experience / maxExp;
        
        if (expText != null)
        {
            expText.text = $"{Mathf.Floor(experience)}/{Mathf.Ceil(maxExp)}";
        }
    }

    private void UpdateLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Lv.{level}";
        }

        if (hudAnimator != null)
        {
            hudAnimator.SetTrigger("LevelUp");
        }
    }

    private void UpdateStats(string statName, float value)
    {
        switch (statName.ToLower())
        {
            case "attack":
                if (attackText != null)
                    attackText.text = $"ATK: {Mathf.Ceil(value)}";
                break;
            case "defense":
                if (defenseText != null)
                    defenseText.text = $"DEF: {Mathf.Ceil(value)}";
                break;
            case "speed":
                if (speedText != null)
                    speedText.text = $"SPD: {Mathf.Ceil(value)}";
                break;
        }
    }

    private void UpdateAttribute(CubeAttribute.AttributeType attributeType)
    {
        if (attributeIcon != null)
        {
            // TODO: Set attribute icon based on type
        }

        if (attributeText != null)
        {
            attributeText.text = attributeType.ToString();
        }
    }

    private void UpdateWave(int wave)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave {wave}";
        }

        if (hudAnimator != null)
        {
            hudAnimator.SetTrigger("NewWave");
        }
    }

    private void UpdateKillCount(int kills)
    {
        if (killCountText != null)
        {
            killCountText.text = $"Kills: {kills}";
        }
    }

    private void UpdateTimer()
    {
        if (timerText != null)
        {
            float gameTime = GameManager.Instance.GameTime;
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void UpdateAllUI()
    {
        if (cubeStats != null)
        {
            UpdateHealth(cubeStats.GetCurrentHealth(), cubeStats.GetMaxHealth());
            UpdateExperience(cubeStats.GetExperience());
            UpdateLevel(cubeStats.GetLevel());
            UpdateStats("attack", cubeStats.GetAttack());
            UpdateStats("defense", cubeStats.GetDefense());
            UpdateStats("speed", cubeStats.GetSpeed());
        }

        if (cubeAttribute != null)
        {
            UpdateAttribute(cubeAttribute.CurrentAttribute);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (cubeStats != null)
        {
            cubeStats.OnHealthChanged -= UpdateHealth;
            cubeStats.OnExperienceGained -= UpdateExperience;
            cubeStats.OnLevelUp -= UpdateLevel;
            cubeStats.OnStatUpgraded -= UpdateStats;
        }

        if (cubeAttribute != null)
        {
            cubeAttribute.OnAttributeChanged -= UpdateAttribute;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveChanged -= UpdateWave;
            GameManager.Instance.OnKillCountChanged -= UpdateKillCount;
        }
    }
}