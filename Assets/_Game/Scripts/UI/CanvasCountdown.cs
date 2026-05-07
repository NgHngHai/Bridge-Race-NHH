using TMPro;
using UnityEngine;

public class CanvasCountdown : UICanvas
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float countdownTime = 3f;
    private bool isCountingDown = false;

    private void Update()
    {
        if (isCountingDown)
        {
            countdownTime -= Time.deltaTime;
            if (countdownTime > 0)
            {
                countdownText.text = Mathf.Ceil(countdownTime).ToString();

                countdownText.transform.localScale = Vector3.Lerp(Vector3.one * 0.3f, Vector3.one, countdownTime - Mathf.Floor(countdownTime));
            }
            else
            {
                countdownText.transform.localScale = Vector3.one * 2;
                countdownText.text = "GO!";

                if (GameManager.Instance.IsState(GameState.MainMenu))
                {
                    GameManager.Instance.OnStartGame();
                }
                else if (GameManager.Instance.IsState(GameState.Pause))
                {
                    GameManager.Instance.OnResumeGame();
                }

                UIManager.Instance.OpenUI<CanvasGameplay>();
                UIManager.Instance.OpenUI<CanvasInput>();

                Close(.2f);
            }
        }
    }

    public void StartCountdown()
    {
        countdownTime = 3f;
        isCountingDown = true;
    }
}