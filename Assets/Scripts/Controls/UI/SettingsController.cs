using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private SettingsCanvasHandler settingsCanvasHandler;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private void Start()
    {
        //InputManager.Instance.PlayerInputActions.InGame.OpenSettingsMenu.performed += OpenSettingsMenuAction;
        InputManager.Instance.PlayerInputActions.PauseMenuUI.CloseSettingsMenu.performed += CloseSettingsMenuAction;
    }

    private void OnDestroy()
    {
        //InputManager.Instance.PlayerInputActions.InGame.OpenSettingsMenu.performed -= OpenSettingsMenuAction;
        InputManager.Instance.PlayerInputActions.PauseMenuUI.CloseSettingsMenu.performed -= CloseSettingsMenuAction;
    }
    
    private void OpenSettingsMenuAction(InputAction.CallbackContext ctx)
    {
        OpenSettingsMenu();
    }

    public void CloseSettingsMenuAction(InputAction.CallbackContext ctx)
    {
        CloseSettingsMenu();
    }
    
    public void OpenSettingsMenu()
    {
        //InputManager.Instance.PlayerInputActions.InGame.Disable();
        //InputManager.Instance.PlayerInputActions.PauseMenuUI.Enable();
        
        InputManager.Instance.PlayerInputActions.PauseMenuUI.ClosePauseMenu.Disable();
        settingsCanvasHandler.ShowCanvas(true);
    }

    public void CloseSettingsMenu()
    {
        if (IsConfirmWindowOpen())
        {
            OnLeavePressed();
            return;
        }
        
        if (HasUnsavedChanges())
        {
            ShowConfirmWindow();
            return;
        }

        HandleCloseLogic();
    }
    
    private void HandleCloseLogic()
    {
        if (IsOnMainSettings())
        {
            CloseEntireMenu();
            return;
        }

        if (IsInSubMenu())
        {
            ReturnToMainSettings();
        }
    }

    private bool IsOnMainSettings()
    {
        return settingsCanvasHandler.SettingsButtonGroup.activeSelf;
    }

    private bool IsInSubMenu()
    {
        return settingsCanvasHandler.AudioSettingsGroup.activeSelf
               || settingsCanvasHandler.VideoSettingsGroup.activeSelf
               || settingsCanvasHandler.ControlsSettingsGroup.activeSelf
               || settingsCanvasHandler.AccessabilitySettingsGroup.activeSelf;
    }
    
    private bool HasUnsavedChanges()
    {
        return AudioManager.Instance.IsDirty
               || VideoManager.Instance.IsDirty
               || LocalizationManager.Instance.IsDirty;
    }
    
    private bool IsConfirmWindowOpen()
    {
        return settingsCanvasHandler.ConfirmSettingsWindow.activeSelf;
    }

    private void CloseEntireMenu()
    {
        UIManager.Instance.SwitchFromSettingsToPause();
    }

    private void ReturnToMainSettings()
    {
        settingsCanvasHandler.SettingsButtonGroup.SetActive(true);

        SetSubMenusActive(false);
    }

    private void SetSubMenusActive(bool state)
    {
        settingsCanvasHandler.AudioSettingsGroup.SetActive(state);
        settingsCanvasHandler.VideoSettingsGroup.SetActive(state);
        settingsCanvasHandler.ControlsSettingsGroup.SetActive(state);
        settingsCanvasHandler.AccessabilitySettingsGroup.SetActive(state);
    }
    
    private void ShowConfirmWindow()
    {
        settingsCanvasHandler.ConfirmSettingsWindow.SetActive(true);

        // Disable interaction underneath
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    public void HideConfirmWindow()
    {
        settingsCanvasHandler.ConfirmSettingsWindow.SetActive(false);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    
    public void OnLeavePressed()
    {
        HideConfirmWindow();
        
        // Discard changes
        AudioManager.Instance.Revert();
        VideoManager.Instance.Revert();
        LocalizationManager.Instance.Revert();

        // Go back to main settings
        ReturnToMainSettings();
    }

    public void OnSavePressed()
    {
        HideConfirmWindow();
        
        // Save changes
        AudioManager.Instance.Save();
        VideoManager.Instance.Save();
        LocalizationManager.Instance.Save();

        // Go back to main settings
        ReturnToMainSettings();
    }
}
