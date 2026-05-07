using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField] private bool isDestroyOnClose = false;

    private void Awake()
    {
        RectTransform rect = GetComponent<RectTransform>();
        float ratio = (float)Screen.width / (float)Screen.height;
        if (ratio > 2.1f)
        {
            Vector2 leftBottom = rect.offsetMin;
            Vector2 rightTop = rect.offsetMax;

            leftBottom.y = 0f;
            rightTop.y = -100f;

            rect.offsetMin = leftBottom;
            rect.offsetMax = rightTop;
        }
    }

    // called before set active
    public virtual void Setup()
    {

    }

    public virtual void Open(float time)
    {
        Invoke(nameof(OpenDirectly), time);
    }

    // called after set active
    public virtual void OpenDirectly()
    {
        gameObject.SetActive(true);
    }

    // set inactive after time
    public virtual void Close(float time)
    {
        Invoke(nameof(CloseDirectly), time);
    }

    // set inactive immediately
    public virtual void CloseDirectly()
    {
        if (isDestroyOnClose)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
