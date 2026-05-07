using UnityEngine;
using System.Collections;

public class CharacterBrick : GameUnit
{
    [SerializeField] private ColorDataSO colorDataSO;
    [SerializeField] private Renderer brickRenderer;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private float flySpeed;
    [SerializeField] private float dampening;
    [SerializeField] private float flyOffsetAmount;
    [SerializeField] private float rotateSpeed;

    private Character character;

    private ColorType colorType;
    public ColorType ColorType => colorType;

    [SerializeField] private Coroutine c;

    public void OnInit(Character character, ColorType colorType, float offset, Vector3 startPosition)
    {
        this.character = character;
        ChangeColor(colorType);
        StartFly(startPosition, Vector3.up * offset);
    }

    protected override void OnDespawn()
    {
        if (c != null)
        {
            StopCoroutine(c);
        }
        trailRenderer.emitting = false;
        TF.position = Vector3.zero;
        TF.localPosition = Vector3.zero;
        TF.rotation = Quaternion.identity;
        character = null;
        base.OnDespawn();
    }

    public void StartFly(Vector3 startPosition, Vector3 destination)
    {
        if (c != null)
        {
            StopCoroutine(c);
        }
        c = StartCoroutine(FlyTo(startPosition, destination));
    }

    private IEnumerator FlyTo(Vector3 startPosition, Vector3 destination)
    {
        TF.position = startPosition;
        trailRenderer.emitting = true;
        float threshold = .2f;

        Vector3 currentVelocity = new Vector3(
            Random.Range(-flyOffsetAmount, flyOffsetAmount),
            0,
            Random.Range(-flyOffsetAmount, flyOffsetAmount)
        );

        while (Vector3.Distance(TF.localPosition, destination) > threshold)
        {
            if (GameManager.Instance.IsState(GameState.Pause))
            {
                yield return null;
                continue;
            }

            TF.localPosition += currentVelocity * Time.deltaTime;
            currentVelocity *= dampening;

            TF.localPosition = Vector3.MoveTowards(
                TF.localPosition,
                destination,
                flySpeed * Time.deltaTime
            );

            TF.localRotation = Quaternion.RotateTowards(TF.localRotation, Quaternion.identity, rotateSpeed * Time.deltaTime);

            yield return null;
        }

        trailRenderer.emitting = false;
        TF.localPosition = destination;
        TF.localRotation = Quaternion.identity;
        character.PlayVFX(TF.position);
    }

    public void ChangeColor(ColorType colorType)
    {
        this.colorType = colorType;
        brickRenderer.material = colorDataSO.GetMaterial(colorType);
    }
}
