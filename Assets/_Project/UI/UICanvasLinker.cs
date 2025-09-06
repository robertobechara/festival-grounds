using UnityEngine;

// This component ensures the UI Canvas finds and links to the main camera at runtime.
[RequireComponent(typeof(Canvas))]
public class UICanvasLinker : MonoBehaviour
{
    private void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        
        // Find the main camera in the scene (it must be tagged "MainCamera")
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            // Set the Canvas to render in the context of the main camera.
            canvas.worldCamera = mainCamera;
            
            // Optional but good practice for input:
            // This ensures UI click events (like for buttons) are correctly processed.
            canvas.planeDistance = 10; 
        }
        else
        {
            Debug.LogError("UICanvasLinker: Could not find a camera tagged 'MainCamera' in the scene!", this);
        }
    }
}
