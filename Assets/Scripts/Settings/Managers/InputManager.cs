using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public PlayerInputActions PlayerInputActions { get; private set; }

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create input actions
        PlayerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    // ------------------------
    // Action Map Switching
    // ------------------------
    
    public void EnableInGameControls()
    {
        PlayerInputActions.InGame.Enable();
    }
    public void DisableInGameControls()
    {
        PlayerInputActions.InGame.Disable();
    }
    
    public void EnablePauseMenuControls()
    {
        PlayerInputActions.PauseMenuUI.Enable();
    }
    public void DisablePauseMenuControls()
    {
        PlayerInputActions.PauseMenuUI.Disable();
    }
    
    public void EnableMainMenuControls()
    {
        PlayerInputActions.MainMenuUI.Enable();
    }
    public void DisableMainMenuControls()
    {
        PlayerInputActions.MainMenuUI.Disable();
    }
}
