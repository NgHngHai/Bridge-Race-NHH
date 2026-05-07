using UnityEngine;

public class CanvasVictory : UICanvas
{
    private CanvasTransition canvasTransition;
    public void NextLevelButton()
    {
        GameManager.Instance.OnReturnToMainMenu();

        canvasTransition = UIManager.Instance.OpenUI<CanvasTransition>();
        canvasTransition.PlayTransition();

        UIManager.Instance.CloseUIDirectly<CanvasInput>();
        UIManager.Instance.CloseUI<CanvasTransition>(Constant.UI_TRANSITION_TIME);
        Close(Constant.UI_TRANSITION_TIME / 2);
    }
}