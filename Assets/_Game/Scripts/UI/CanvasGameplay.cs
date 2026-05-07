using TMPro;
using UnityEngine;

public class CanvasGameplay : UICanvas
{
    [SerializeField] private TextMeshProUGUI levelName;

    public override void Setup()
    {

        UpdateLevelName();
    }

    public void UpdateLevelName()
    {
        int currentLevel = LevelManager.Instance.GetCurrentLevel();
        levelName.text = "Level " + currentLevel;
    }

    public void SettingsButton()
    {
        GameManager.Instance.OnPauseGame();

        CloseDirectly();
        UIManager.Instance.OpenUI<CanvasSetting>();
    }
}