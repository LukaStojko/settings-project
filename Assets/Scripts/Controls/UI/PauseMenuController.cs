using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private PauseMenuCanvasHandler pauseMenuCanvasHandler;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private void Start()
    {
        //InputManager.Instance.PlayerInputActions.InGame.OpenPauseMenu.performed += OpenPauseMenuAction;
        InputManager.Instance.PlayerInputActions.PauseMenuUI.ClosePauseMenu.performed += ClosePauseMenuAction;
    }

    private void OnDestroy()
    {
        //InputManager.Instance.PlayerInputActions.InGame.OpenPauseMenu.performed -= OpenPauseMenuAction;
        InputManager.Instance.PlayerInputActions.PauseMenuUI.ClosePauseMenu.performed -= ClosePauseMenuAction;
    }
    
    private void OpenPauseMenuAction(InputAction.CallbackContext ctx)
    {
        OpenPauseMenu();
    }

    public void ClosePauseMenuAction(InputAction.CallbackContext ctx)
    {
        ClosePauseMenu();
    }

    public void OpenPauseMenu()
    {
        UIManager.Instance.OpenPauseMenu();
    }
    
    public void ClosePauseMenu()
    {
        UIManager.Instance.ClosePauseMenu();
    }

    public void OpenSettings()
    {
        UIManager.Instance.SwitchFromPauseToSettings();
    }
}
