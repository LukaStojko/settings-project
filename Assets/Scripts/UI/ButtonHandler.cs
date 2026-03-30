using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    [Header("Objects to Enable")]
    [SerializeField] private List<GameObject> enableObjects = new List<GameObject>();

    [Header("Objects to Disable")]
    [SerializeField] private List<GameObject> disableObjects = new List<GameObject>();

    public void ApplyVisibility()
    {
        // Enable objects
        foreach (var obj in enableObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // Disable objects
        foreach (var obj in disableObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}
