using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("UI Prefabs to Instantiate")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject gameUICanvas;

    [Header("Canvas References")]
    [SerializeField] private BaseCanvasHandler _mainMenuCanvasHandler;
    [SerializeField] private BaseCanvasHandler _pauseMenuCanvasHandler;
    [SerializeField] private BaseCanvasHandler _settingsCanvasHandler;
    [SerializeField] private BaseCanvasHandler _gameUICanvasHandler;
    
    public BaseCanvasHandler MainMenuCanvasHandler  => _mainMenuCanvasHandler;
    public BaseCanvasHandler PauseMenuCanvasHandler  => _pauseMenuCanvasHandler;
    public BaseCanvasHandler SettingsCanvasHandler  => _settingsCanvasHandler;
    public BaseCanvasHandler GmeUICanvasHandler  => _gameUICanvasHandler;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InputManager.Instance.PlayerInputActions.InGame.OpenPauseMenu.performed += OpenPauseMenuAction;
    }
    private void OnDestroy()
    {
        InputManager.Instance.PlayerInputActions.InGame.OpenPauseMenu.performed -= OpenPauseMenuAction;
    }

    #region  UI Creation

    public void CreateMainMenu()
    {
        if (_mainMenuCanvasHandler || !mainMenuCanvas)
            return;
        
        GameObject canvasObject = Instantiate(mainMenuCanvas);
        _mainMenuCanvasHandler = canvasObject.GetComponent<BaseCanvasHandler>();
    }

    public void CreatePauseMenu()
    {
        if (_pauseMenuCanvasHandler|| !pauseMenuCanvas)
            return;
        
        GameObject canvasObject = Instantiate(pauseMenuCanvas);
        _pauseMenuCanvasHandler = canvasObject.GetComponent<BaseCanvasHandler>();
    }

    public void CreateSettings()
    {
        if (_settingsCanvasHandler|| !settingsCanvas)
            return;
        
        GameObject canvasObject = Instantiate(settingsCanvas);
        _settingsCanvasHandler = canvasObject.GetComponent<BaseCanvasHandler>();
    }

    public void CreateGameMenu()
    {
        if (_gameUICanvasHandler|| !gameUICanvas)
            return;
        
        GameObject canvasObject = Instantiate(gameUICanvas);
        _gameUICanvasHandler = canvasObject.GetComponent<BaseCanvasHandler>();
    }

    #endregion
    
    #region UI Controls

    public void OpenPauseMenu()
    {
        Time.timeScale = 0;
        InputManager.Instance.PlayerInputActions.InGame.Disable();
        
        InputManager.Instance.PlayerInputActions.PauseMenuUI.Enable();
        _pauseMenuCanvasHandler.gameObject.SetActive(true);
        _pauseMenuCanvasHandler.ShowCanvas(true);
    }
    public void ClosePauseMenu()
    {
        _pauseMenuCanvasHandler.ShowCanvas(false);
        _pauseMenuCanvasHandler.gameObject.SetActive(false);
        InputManager.Instance.PlayerInputActions.PauseMenuUI.Disable();
        
        InputManager.Instance.PlayerInputActions.InGame.Enable();
        Time.timeScale = 1;
    }
    
    public void OpenSettingsMenu()
    {
        Time.timeScale = 0;
        InputManager.Instance.PlayerInputActions.InGame.Disable();
        
        InputManager.Instance.PlayerInputActions.PauseMenuUI.Enable();
        _settingsCanvasHandler.gameObject.SetActive(true);
        _settingsCanvasHandler.ShowCanvas(true);
    }
    public void CloseSettingsMenu()
    {
        InputManager.Instance.PlayerInputActions.PauseMenuUI.ClosePauseMenu.Enable();
        
        _settingsCanvasHandler.ShowCanvas(false);
        _settingsCanvasHandler.gameObject.SetActive(false);
        InputManager.Instance.PlayerInputActions.PauseMenuUI.Disable();
        
        InputManager.Instance.PlayerInputActions.InGame.Enable();
        Time.timeScale = 1;
    }

    public void SwitchFromPauseToSettings()
    {
        _pauseMenuCanvasHandler.ShowCanvas(false);
        InputManager.Instance.PlayerInputActions.PauseMenuUI.ClosePauseMenu.Disable();
        
        UIManager.Instance.SettingsCanvasHandler.ShowCanvas(true);
    }
    public void SwitchFromSettingsToPause()
    {
        _pauseMenuCanvasHandler.ShowCanvas(true);
        InputManager.Instance.PlayerInputActions.PauseMenuUI.ClosePauseMenu.Enable();
        _settingsCanvasHandler.ShowCanvas(false);
    }
    
    #endregion
    
    #region UI Controls Actions
    
    private void OpenPauseMenuAction(InputAction.CallbackContext ctx)
    {
        OpenPauseMenu();
    }
    
    #endregion
}