using TMPro;
using UnityEngine;

public class CanvasMainMenu : UICanvas
{
    [SerializeField] private TMP_InputField playerNameInput;
    private CanvasCountdown canvasCountdown;


    public void PlayButton()
    {
        //GameManager.Instance.OnStartGame();

        //UIManager.Instance.OpenUI<CanvasGameplay>();
        //UIManager.Instance.OpenUI<CanvasInput>();


        canvasCountdown = UIManager.Instance.OpenUI<CanvasCountdown>();
        canvasCountdown.StartCountdown();

        string playerName = playerNameInput.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            LevelManager.Instance.SetPlayerName(playerName);
        }

        CloseDirectly();
    }
}
