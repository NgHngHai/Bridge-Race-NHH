using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform joystickOuter;
    [SerializeField] private RectTransform joystickInner;
    [SerializeField] private Player player;

    //private bool isDragging = false;
    private Vector2 startPoint;
    private Vector2 currentPoint;
    private Vector2 direction;
    private Vector3 inputVector;
    private float moveRange;

    private float moveThreshold = 0.01f;

    private void Start()
    {
        moveRange = joystickOuter.sizeDelta.x / 2f;
        player = LevelManager.Instance.Player;
    }

    private void Update()
    {
        if (inputVector.sqrMagnitude > moveThreshold)
        {
            player.Move(inputVector, GetRotation());
        }
        else
        {
            player.Move(Vector3.zero, GetRotation());
        }
    }

    private void OnDisable()
    {
        DisableJoystick();
    }

    private Quaternion GetRotation()
    {
        if (inputVector.sqrMagnitude < moveThreshold)
            return Quaternion.identity;
        
        float angle = Mathf.Atan2(inputVector.x, inputVector.z) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, angle, 0);
    }

    private void EnableJoystick()
    {
        float x = startPoint.x, y = startPoint.y;
        joystickOuter.gameObject.SetActive(true);

        // clamp to screen
        if (startPoint.x < 150)
        {
            x = 150;
        }
        else if (startPoint.x > Screen.width - 150)
        {
            x = Screen.width - 150;
        }

        if (startPoint.y < 150)
        {
            y = 150;
        }
        else if (startPoint.y > Screen.height - 150)
        {
            y = Screen.height - 150;
        }

        startPoint = new Vector2(x, y);
        joystickOuter.anchoredPosition = startPoint;
    }

    private void DisableJoystick()
    {
        joystickInner.anchoredPosition = Vector2.zero;
        joystickOuter.gameObject.SetActive(false);

        inputVector = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPoint = eventData.position;

        EnableJoystick();
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //direction = eventData.position - startPoint;

        //currentPoint = Vector2.ClampMagnitude(direction, moveRange);

        //joystickInner.anchoredPosition = currentPoint;
        //inputVector = new Vector3(currentPoint.x, 0, currentPoint.y) / moveRange;

        if (GameManager.Instance.IsState(GameState.Pause))
        {
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickOuter,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            currentPoint = Vector2.ClampMagnitude(localPoint, moveRange);

            joystickInner.anchoredPosition = currentPoint;
            inputVector = new Vector3(currentPoint.x, 0, currentPoint.y) / moveRange;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        DisableJoystick();
    }
}
