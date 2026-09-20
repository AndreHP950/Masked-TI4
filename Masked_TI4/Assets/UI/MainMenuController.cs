using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [Header("Arquivos UXML das Telas")]
    [SerializeField] private VisualTreeAsset _startScreenUxml;
    [SerializeField] private VisualTreeAsset _optionsScreenUxml;
    [SerializeField] private VisualTreeAsset _settingsScreenUxml;
    [SerializeField] private VisualTreeAsset _creditsScreenUxml;

    [Header("Configuração de Gameplay")]
    [SerializeField] private string _gameplaySceneName = "GameplayScene";

    private UIDocument _uiDocument;
    private VisualElement _screenContainer;
    private bool _isTransitioning = false;
    private bool _isInStartScreen = true;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void Start()
    {
        LoadStartScreen();
    }

    private void Update()
    {
        if (_isInStartScreen && !_isTransitioning && Input.anyKeyDown)
        {
            StartCoroutine(SmoothTransitionToOptions());
        }
    }

    private void LoadStartScreen()
    {
        if (_startScreenUxml == null) return;

        _isInStartScreen = true;
        _uiDocument.visualTreeAsset = _startScreenUxml;

        var root = _uiDocument.rootVisualElement;
        _screenContainer = root.Q<VisualElement>("screen-container");

        root.RegisterCallback<PointerDownEvent>(evt =>
        {
            if (_isInStartScreen && !_isTransitioning)
            {
                StartCoroutine(SmoothTransitionToOptions());
            }
        });
    }

    private IEnumerator SmoothTransitionToOptions()
    {
        _isTransitioning = true;

        yield return FadeScreen(1f, 0f, 0.4f);

        LoadOptionsMenu();

        yield return FadeScreen(0f, 1f, 0.5f);

        _isTransitioning = false;
    }

    private void LoadOptionsMenu()
    {
        if (_optionsScreenUxml == null) return;

        _isInStartScreen = false;
        _uiDocument.visualTreeAsset = _optionsScreenUxml;

        var root = _uiDocument.rootVisualElement;
        _screenContainer = root.Q<VisualElement>("screen-container");

        var btnStart = root.Q<Button>("btn-start");
        var btnSettings = root.Q<Button>("btn-settings");
        var btnCredits = root.Q<Button>("btn-credits");
        var btnQuit = root.Q<Button>("btn-quit");

        if (btnStart != null) btnStart.clicked += () => OnOptionClicked(_gameplaySceneName);
        if (btnSettings != null) btnSettings.clicked += OnSettingsClicked;
        if (btnCredits != null) btnCredits.clicked += OnCreditsClicked;
        if (btnQuit != null) btnQuit.clicked += OnQuitClicked;
    }

    private void OnOptionClicked(string sceneName)
    {
        if (_isTransitioning) return;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private void OnSettingsClicked()
    {
        if (_isTransitioning) return;
        StartCoroutine(OpenSettingsRoutine());
    }

    private IEnumerator OpenSettingsRoutine()
    {
        _isTransitioning = true;
        yield return FadeScreen(1f, 0f, 0.3f);

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
            btnBack.clicked += () => StartCoroutine(BackToOptionsRoutine());
        }

        yield return FadeScreen(0f, 1f, 0.3f);
        _isTransitioning = false;
    }

    private IEnumerator BackToOptionsRoutine()
    {
        _isTransitioning = true;
        yield return FadeScreen(1f, 0f, 0.3f);
        LoadOptionsMenu();
        yield return FadeScreen(0f, 1f, 0.3f);
        _isTransitioning = false;
    }

    private void OnCreditsClicked()
    {
        if (_isTransitioning || _creditsScreenUxml == null) return;
        StartCoroutine(OpenSubMenuRoutine(_creditsScreenUxml, null));
    }

    private IEnumerator OpenSubMenuRoutine(VisualTreeAsset targetUxml, System.Action setupAction)
    {
        _isTransitioning = true;
        yield return FadeScreen(1f, 0f, 0.3f);

        _uiDocument.visualTreeAsset = targetUxml;
        var root = _uiDocument.rootVisualElement;
        _screenContainer = root.Q<VisualElement>("screen-container");

        setupAction?.Invoke();

        var btnBack = root.Q<Button>("btn-back");
        if (btnBack != null)
        {
            btnBack.clicked += () => StartCoroutine(BackToOptionsRoutine());
        }

        yield return FadeScreen(0f, 1f, 0.3f);
        _isTransitioning = false;
    }

    private void OnQuitClicked()
    {
        if (_isTransitioning) return;
        StartCoroutine(QuitGameRoutine());
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        _isTransitioning = true;
        yield return FadeScreen(1f, 0f, 0.6f);

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator QuitGameRoutine()
    {
        _isTransitioning = true;
        yield return FadeScreen(1f, 0f, 0.6f);

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
                elapsed += Time.deltaTime;
                _screenContainer.style.opacity = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                yield return null;
            }
            _screenContainer.style.opacity = endAlpha;
        }
    }
}