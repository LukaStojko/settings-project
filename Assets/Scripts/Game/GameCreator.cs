using UnityEngine;

public class GameCreator : MonoBehaviour
{
    void Start()
    {
        InputManager.Instance.EnableInGameControls();
        
        UIManager.Instance.CreateSettings();
        UIManager.Instance.SettingsCanvasHandler.ShowCanvas(false);
        
        UIManager.Instance.CreatePauseMenu();
        UIManager.Instance.PauseMenuCanvasHandler.ShowCanvas(false);
    }

    void OnDestroy()
    {
        
    }
}
