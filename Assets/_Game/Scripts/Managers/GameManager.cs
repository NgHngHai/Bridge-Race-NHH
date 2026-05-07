using UnityEngine;

public enum GameState
{
    MainMenu = 0,
    Gameplay = 1,
    Pause = 2,
    Finish = 3
}

public class GameManager : Singleton<GameManager>
{

    [SerializeField]private GameState currentState;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        OnInit();
    }

    private void OnInit()
    {
        ChangeState(GameState.MainMenu);
        UIManager.Instance.OpenUI<CanvasTransition>();
        UIManager.Instance.OpenUI<CanvasMainMenu>();

        LevelManager.Instance.LoadLevel(LevelManager.Instance.GetCurrentLevel());

        LevelManager.Instance.OnInit();

        UIManager.Instance.CloseUI<CanvasTransition>(Constant.UI_TRANSITION_TIME);
    }

    public void OnStartGame()
    {
        ChangeState(GameState.Gameplay);
        GlobalEvents.OnStartGame?.Invoke();
    }

    public void OnPauseGame()
    {
        ChangeState(GameState.Pause);
        GlobalEvents.OnPauseGame?.Invoke();
    }

    public void OnResumeGame()
    {
        ChangeState(GameState.Gameplay);
        GlobalEvents.OnResumeGame?.Invoke();
    }

    public void OnFinishGame(Character character)
    {
        ChangeState(GameState.Finish);
        GlobalEvents.OnEndGame?.Invoke(character);

        if (character is Player)
        {
            UIManager.Instance.CloseAllUI();
            UIManager.Instance.OpenUI<CanvasVictory>();
        }
        else
        {
            UIManager.Instance.CloseAllUI();
            UIManager.Instance.OpenUI<CanvasDefeat>();
        }
    }

    public void OnReturnToMainMenu()
    {
        ChangeState(GameState.MainMenu);

        GlobalEvents.OnReturnToMenu?.Invoke();

        LevelManager.Instance.ReloadLevelDelayed(Constant.UI_TRANSITION_TIME / 3);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }
    public bool IsState(GameState state)
    {
        return currentState == state;
    }
}