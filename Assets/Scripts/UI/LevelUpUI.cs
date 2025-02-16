using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelUpUI : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeOption
    {
        public string statName;
        public Sprite icon;
        public string description;
        public Color color = Color.white;
    }

    [Header("UI References")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private GameObject optionPrefab;
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button closeButton;
    
    [Header("Options")]
    [SerializeField] private List<UpgradeOption> upgradeOptions;
    [SerializeField] private int optionsToShow = 3;
    
    [Header("Animation")]
    [SerializeField] private Animator panelAnimator;
    [SerializeField] private float optionDelay = 0.2f;
    
    private CubeStats cubeStats;
    private List<GameObject> spawnedOptions = new List<GameObject>();
    private bool isShowing;

    private void Start()
    {
        InitializeUI();
        SubscribeToEvents();
    }

    private void InitializeUI()
    {
        cubeStats = GameObject.FindGameObjectWithTag("Player").GetComponent<CubeStats>();
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideLevelUpPanel);
        }

        // Initially hide the panel
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
    }

    private void SubscribeToEvents()
    {
        if (cubeStats != null)
        {
            cubeStats.OnLevelUp += ShowLevelUpPanel;
        }
    }

    private void ShowLevelUpPanel(int level)
    {
        if (isShowing) return;
        isShowing = true;

        // Pause the game
        GameManager.Instance.PauseGame();

        // Show panel
        levelUpPanel.SetActive(true);
        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger("Show");
        }

        // Update level text
        if (levelText != null)
        {
            levelText.text = $"Level {level}!";
        }

        // Clear previous options
        ClearOptions();

        // Generate new random options
        StartCoroutine(SpawnOptionsWithDelay());
    }

    private System.Collections.IEnumerator SpawnOptionsWithDelay()
    {
        List<UpgradeOption> shuffledOptions = new List<UpgradeOption>(upgradeOptions);
        ShuffleList(shuffledOptions);

        for (int i = 0; i < Mathf.Min(optionsToShow, shuffledOptions.Count); i++)
        {
            SpawnOption(shuffledOptions[i]);
            yield return new WaitForSecondsRealtime(optionDelay);
        }
    }

    private void SpawnOption(UpgradeOption option)
    {
        GameObject optionObj = Instantiate(optionPrefab, optionsContainer);
        spawnedOptions.Add(optionObj);

        // Setup option UI
        Button button = optionObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => SelectUpgrade(option));
        }

        // Set icon
        Image icon = optionObj.GetComponentInChildren<Image>();
        if (icon != null)
        {
            icon.sprite = option.icon;
            icon.color = option.color;
        }

        // Set texts
        TextMeshProUGUI[] texts = optionObj.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length >= 2)
        {
            texts[0].text = option.statName;
            texts[1].text = option.description;
        }

        // Animate spawn
        optionObj.transform.localScale = Vector3.zero;
        LeanTween.scale(optionObj, Vector3.one, 0.3f).setEaseOutBack();
    }

    private void SelectUpgrade(UpgradeOption option)
    {
        // Apply upgrade
        if (cubeStats != null)
        {
            cubeStats.UpgradeStat(option.statName);
        }

        // Play selection animation
        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger("Select");
        }

        // Hide panel after short delay
        Invoke(nameof(HideLevelUpPanel), 0.5f);
    }

    private void HideLevelUpPanel()
    {
        if (!isShowing) return;

        // Animate hide
        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger("Hide");
        }

        // Resume game
        GameManager.Instance.ResumeGame();

        // Clear options after animation
        Invoke(nameof(ClearOptions), 0.5f);
        
        isShowing = false;
    }

    private void ClearOptions()
    {
        foreach (var option in spawnedOptions)
        {
            Destroy(option);
        }
        spawnedOptions.Clear();
    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void OnDestroy()
    {
        if (cubeStats != null)
        {
            cubeStats.OnLevelUp -= ShowLevelUpPanel;
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(HideLevelUpPanel);
        }
    }
}