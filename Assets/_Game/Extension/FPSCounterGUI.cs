using UnityEngine;

public class FPSCounterGUI : MonoBehaviour
{
    float deltaTime = 0f;
    private int fontSize = 30;
    GUIStyle style;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.fontSize = fontSize;
        }

        float fps = 1.0f / deltaTime;
        GUI.Label(new Rect(10, 10, 1500, 250), $"FPS: {fps:F1}", style);
    }
}