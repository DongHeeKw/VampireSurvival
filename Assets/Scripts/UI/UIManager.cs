using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [System.Serializable]
    public class UIScreen
    {
        public string screenName;
        public GameObject screenObject;
        public bool keepAlive;
    }

    [Header("UI Screens")]
    [SerializeField] private List<UIScreen> screens;
    [SerializeField] private string initialScreen = "MainMenu";
    
    [Header("Common UI Elements")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeSpeed = 1f;
    
    private UIScreen currentScreen;
    private Stack<string> screenHistory = new Stack<string>();
    private Dictionary<string, UIScreen> screenDictionary = new Dictionary<string, UIScreen>();

    // Events
    public event Action<string> OnScreenChanged;
    public event Action OnBackButtonPressed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ShowScreen(initialScreen);
    }

    private void Update()
    {
        // Handle back button (Android)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }

    private void InitializeUI()
    {
        // Initialize screen dictionary
        foreach (var screen in screens)
        {
            screenDictionary[screen.screenName] = screen;
            if (!screen.keepAlive)
            {
                screen.screenObject.SetActive(false);
            }
        }
    }

    public void ShowScreen(string screenName, bool remember = true)
    {
        if (!screenDictionary.ContainsKey(screenName))
        {
            Debug.LogWarning($"Screen {screenName} not found!");
            return;
        }

        StartCoroutine(TransitionToScreen(screenName, remember));
    }

    private System.Collections.IEnumerator TransitionToScreen(string screenName, bool remember)
    {
        // Fade out
        if (fadePanel != null)
        {
            yield return FadeOut();
        }

        // Hide current screen
        if (currentScreen != null && !currentScreen.keepAlive)
        {
            currentScreen.screenObject.SetActive(false);
        }

        // Remember current screen for back navigation
        if (remember && currentScreen != null)
        {
            screenHistory.Push(currentScreen.screenName);
        }

        // Show new screen
        UIScreen newScreen = screenDictionary[screenName];
        newScreen.screenObject.SetActive(true);
        currentScreen = newScreen;

        // Fade in
        if (fadePanel != null)
        {
            yield return FadeIn();
        }

        OnScreenChanged?.Invoke(screenName);
    }

    private System.Collections.IEnumerator FadeOut()
    {
        fadePanel.gameObject.SetActive(true);
        while (fadePanel.alpha < 1)
        {
            fadePanel.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    private System.Collections.IEnumerator FadeIn()
    {
        while (fadePanel.alpha > 0)
        {
            fadePanel.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        fadePanel.gameObject.SetActive(false);
    }

    public void GoBack()
    {
        if (screenHistory.Count > 0)
        {
            string previousScreen = screenHistory.Pop();
            ShowScreen(previousScreen, false);
        }
    }

    private void HandleBackButton()
    {
        OnBackButtonPressed?.Invoke();
        
        if (currentScreen.screenName != "MainMenu")
        {
            GoBack();
        }
        else
        {
            // Show quit game confirmation
            ShowQuitConfirmation();
        }
    }

    private void ShowQuitConfirmation()
    {
        // TODO: Implement quit confirmation dialog
    }

    public void PauseGame()
    {
        ShowScreen("PauseMenu");
        GameManager.Instance.PauseGame();
    }

    public void ResumeGame()
    {
        GoBack();
        GameManager.Instance.ResumeGame();
    }

    public bool IsScreenActive(string screenName)
    {
        return currentScreen != null && currentScreen.screenName == screenName;
    }

    public void ShowMessage(string message, float duration = 2f)
    {
        StartCoroutine(ShowMessageCoroutine(message, duration));
    }

    private System.Collections.IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        // TODO: Implement floating message system
        yield return new WaitForSeconds(duration);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}