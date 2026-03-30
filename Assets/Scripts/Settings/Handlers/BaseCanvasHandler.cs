using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseCanvasHandler : MonoBehaviour
{
    [SerializeField] private GameObject canvasGameObject;
    public bool IsOpen { get; private set; }

    [Header("Default State")]
    [SerializeField] private List<GameObject> defaultActiveObjects;
    [SerializeField] private List<GameObject> defaultInactiveObjects;

    // Events
    public event Action OnAfterCanvasOpenedEvent;
    public event Action OnAfterCanvasClosedEvent;

    #region Public API
    
    public void ShowCanvas(bool show)
    {
        if (show)
            EnableCanvas();
        else
            DisableCanvas();
    }

    public void ToggleCanvas()
    {
        if(!IsOpen)
            EnableCanvas();
        else
            DisableCanvas();
    }

    #endregion
    
    private void EnableCanvas()
    {
        //OnBeforeCanvasOpened();

        canvasGameObject.SetActive(true);
        ResetToDefaultState();
        
        IsOpen = true;

        OnAfterCanvasOpenedEvent?.Invoke();
    }

    private void DisableCanvas()
    {
        //OnBeforeCanvasClosed();

        canvasGameObject.SetActive(false);
        
        IsOpen = false;

        OnAfterCanvasClosedEvent?.Invoke();
    }

    private void ResetToDefaultState()
    {
        foreach (GameObject obj in defaultActiveObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        foreach (GameObject obj in defaultInactiveObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}