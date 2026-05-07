using UnityEngine;
using UnityEngine.TextCore.Text;

public class GrayBrick : PickupBrick
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float forceMagnitude = 2;
    [SerializeField] private float pickupDelay = 1;
    [SerializeField] private bool isPickupAllowed = false;

    private float startTime;
    private float remainingDelay;

    protected override void OnTriggerEnter(Collider other)
    {
        if (isPickupAllowed)
        {
            base.OnTriggerEnter(other);
        }
    }
    public void OnInit(ColorType colorType)
    {
        ChangeColor(colorType);
        SetCurrentPlatform(null);
        startTime = Time.time;
        Invoke(nameof(AllowPickup), pickupDelay);
        isPickupAllowed = false;
    }

    public override void OnPauseGame()
    {
        base.OnPauseGame();
        remainingDelay = pickupDelay - (Time.time - startTime);
    }

    public override void OnResumeGame()
    {
        base.OnResumeGame();
        Invoke(nameof(AllowPickup), remainingDelay);
    }

    private void AllowPickup()
    {
        if (GameManager.Instance.IsState(GameState.Pause))
        {
            return;
        }
        isPickupAllowed = true;
    }

    public void ApplyForce(Vector3 direction)
    {
        Vector3 force = direction  * forceMagnitude + Vector3.up;
        rb.AddForce(force, ForceMode.Impulse);
    }
}
