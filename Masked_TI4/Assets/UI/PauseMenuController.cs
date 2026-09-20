using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    [Header("Arquivos UXML das Telas")]
    [SerializeField] private VisualTreeAsset _pauseMenuUxml;
    [SerializeField] private VisualTreeAsset _settingsScreenUxml;

    [Header("Configuração de Cenas")]
    [SerializeField] private string _mainMenuSceneName = "MainMenuScene";

    private UIDocument _uiDocument;
    private VisualElement _screenContainer;
    private bool _isPaused = false;
    private bool _isTransitioning = false;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        _uiDocument.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_isTransitioning)
        {
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (_pauseMenuUxml == null) return;

        _isPaused = true;
        Time.timeScale = 0f;
        _uiDocument.enabled = true;
        GameController.instance.LivrarCursor();

        LoadPauseMenu();
    }

    private void LoadPauseMenu()
    {
        _uiDocument.visualTreeAsset = _pauseMenuUxml;

        var root = _uiDocument.rootVisualElement;
        _screenContainer = root.Q<VisualElement>("screen-container");

        var btnResume = root.Q<Button>("btn-resume");
        var btnSettings = root.Q<Button>("btn-settings");
        var btnMainMenu = root.Q<Button>("btn-main-menu");
        var btnQuit = root.Q<Button>("btn-quit");

        if (btnResume != null) btnResume.clicked += ResumeGame;
        if (btnSettings != null) btnSettings.clicked += OnSettingsClicked;
        if (btnMainMenu != null) btnMainMenu.clicked += OnMainMenuClicked;
        if (btnQuit != null) btnQuit.clicked += OnQuitClicked;

        StartCoroutine(FadeScreen(0f, 1f, 0.2f));
    }

    public void ResumeGame()
    {
        if (_isTransitioning) return;
        GameController.instance.TravarCursor();
        StartCoroutine(ResumeRoutine());
    }

    private IEnumerator ResumeRoutine()
    {
        _isTransitioning = true;
        yield return FadeScreen(1f, 0f, 0.2f);

        _uiDocument.enabled = false;
        Time.timeScale = 1f;
        _isPaused = false;
        _isTransitioning = false;
    }

    private void OnSettingsClicked()
    {
        if (_isTransitioning || _settingsScreenUxml == null) return;
        StartCoroutine(OpenSettingsRoutine());
    }

    private IEnumerator OpenSettingsRoutine()
    {
        _isTransitioning = true;
        yield return FadeScreen(1f, 0f, 0.2f);

        _uiDocument.visualTreeAsset = _settingsScreenUxml;
        var root = _uiDocument.rootVisualElement;
        _screenContainer = root.Q<VisualElement>("screen-container");

        var slider = root.Q<Slider>("volume-slider");
        if (slider != null)
        {
            slider.value = AudioListener.volume;
            slider.RegisterValueChangedCallback(evt =>
            {
                AudioListener.volume = evt.newValue;
            });
        }

        var btnBack = root.Q<Button>("btn-back");
        if (btnBack != null)
        {
            btnBack.clicked += () => StartCoroutine(BackToPauseRoutine());
        }

        yield return FadeScreen(0f, 1f, 0.2f);
        _isTransitioning = false;
    }

    private IEnumerator BackToPauseRoutine()
    {
        _isTransitioning = true;
        yield return FadeScreen(1f, 0f, 0.2f);
        LoadPauseMenu();
        _isTransitioning = false;
    }

    private void OnMainMenuClicked()
    {
        if (_isTransitioning) return;
        StartCoroutine(LoadMainMenuRoutine());
    }

    private void OnQuitClicked()
    {
        if (_isTransitioning) return;
        StartCoroutine(QuitGameRoutine());
    }

    private IEnumerator LoadMainMenuRoutine()
    {
        _isTransitioning = true;
        yield return FadeScreen(1f, 0f, 0.4f);

        Time.timeScale = 1f;
        SceneManager.LoadScene(_mainMenuSceneName);
    }

    private IEnumerator QuitGameRoutine()
    {
        _isTransitioning = true;
        yield return FadeScreen(1f, 0f, 0.4f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator FadeScreen(float startAlpha, float endAlpha, float duration)
    {
        if (_screenContainer != null)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                _screenContainer.style.opacity = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                yield return null;
            }
            _screenContainer.style.opacity = endAlpha;
        }
    }
}