using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOverController : MonoBehaviour
{
    public static GameOverController instance;

    [SerializeField] private VisualTreeAsset _gameOverUxml;
    [SerializeField] private string _mainMenuSceneName = "MainMenuScene";

    private UIDocument _uiDocument;
    private bool _isGameOver;

    private void Awake()
    {
        instance = this;
        _uiDocument = GetComponent<UIDocument>();
        if (_uiDocument != null) _uiDocument.enabled = false;
    }

    public void TriggerGameOver()
    {
        if (_isGameOver || _gameOverUxml == null || _uiDocument == null) return;

        _isGameOver = true;
        Time.timeScale = 0f;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        _uiDocument.enabled = true;
        _uiDocument.visualTreeAsset = _gameOverUxml;

        var root = _uiDocument.rootVisualElement;
        root.Q<Button>("btn-checkpoint").clicked += OnCheckpointClicked;
        root.Q<Button>("btn-main-menu").clicked += OnMainMenuClicked;
    }

    private void OnCheckpointClicked()
    {
        if (PlayerRespawn.instance != null)
        {
            PlayerRespawn.instance.Respawn();
        }

        _uiDocument.enabled = false;
        Time.timeScale = 1f;
        _isGameOver = false;
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_mainMenuSceneName);
    }
}
