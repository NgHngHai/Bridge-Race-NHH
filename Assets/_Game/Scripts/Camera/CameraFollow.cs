using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Vector3 originalOffset;
    [SerializeField] private Vector3 additionalOffset;
    [SerializeField] private float offsetScale;
    [SerializeField] private float speed;

    [SerializeField] private Transform target;
    private Transform tf;

    private void OnEnable()
    {
        GlobalEvents.OnLoadLevel += OnInit;
    }

    private void OnDisable()
    {
        GlobalEvents.OnLoadLevel -= OnInit;
    }

    private void Start()
    {
        tf = transform;
    }

    public void OnInit(int levelIndex = 0)
    {
        target = player.TF;
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.IsState(GameState.Gameplay))
        {
            additionalOffset = originalOffset.normalized * player.GetBrickCount() * offsetScale;
        }
        else
        {
            additionalOffset = Vector3.zero;
        }
        tf.position = Vector3.Lerp(tf.position, target.position + originalOffset + additionalOffset, speed * Time.deltaTime);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
