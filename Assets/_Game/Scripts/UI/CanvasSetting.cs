using UnityEngine;

public class CanvasSetting : UICanvas
{
    private CanvasTransition canvasTransition;
    private CanvasCountdown canvasCountdown;

    public void ResumeButton()
    {
        //GameManager.Instance.OnResumeGame();

        //UIManager.Instance.OpenUI<CanvasGameplay>();
        //UIManager.Instance.OpenUI<CanvasInput>();

        canvasCountdown = UIManager.Instance.OpenUI<CanvasCountdown>();
        canvasCountdown.StartCountdown();

        CloseDirectly();
    }

    public void MainMenuButton()
    {
        GameManager.Instance.OnReturnToMainMenu();

        canvasTransition = UIManager.Instance.OpenUI<CanvasTransition>();
        canvasTransition.PlayTransition();

        UIManager.Instance.CloseUIDirectly<CanvasInput>();
        UIManager.Instance.CloseUI<CanvasGameplay>(Constant.UI_TRANSITION_TIME / 2);
        UIManager.Instance.CloseUI<CanvasTransition>(Constant.UI_TRANSITION_TIME);
        Close(Constant.UI_TRANSITION_TIME / 2);
    }
}
