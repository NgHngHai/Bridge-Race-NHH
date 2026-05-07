using UnityEngine;

public class PickupBrick : GameUnit
{
    [SerializeField] ColorDataSO colorDataSO;
    [SerializeField] Renderer brickRenderer;

    private ColorType colorType;
    public ColorType ColorType => colorType;

    [SerializeField] private Platform currentPlatform;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constant.TAG_CHARACTER))
        {
            Character character = other.GetComponent<Character>();

            if (character.IsKnocked || !GameManager.Instance.IsState(GameState.Gameplay))
            {
                return;
            }

            if (CompareColorType(ColorType.Gray))
            {
                OnGrayTaken(character);
            }
            else if (character.CompareColorType(colorType))
            {
                OnColoredTaken(character);
            }
        }
    }

    public void OnInit(ColorType colorType, Platform platform)
    {
        ChangeColor(colorType);
        SetCurrentPlatform(platform);
    }

    public void ChangeColor(ColorType colorType)
    {
        this.colorType = colorType;
        brickRenderer.material = colorDataSO.GetMaterial(colorType);
    }

    public bool CompareColorType(ColorType colorType)
    {
        return this.colorType == colorType;
    }

    public void SetCurrentPlatform(Platform platform)
    {
        currentPlatform = platform;
    }

    private void OnGrayTaken(Character character)
    {
        character.AddBrick(TF.position);
        OnDespawn();
    }

    private void OnColoredTaken(Character character)
    {
        character.AddBrick(TF.position);
        currentPlatform.ReplaceBrick(TF.position, this);
        OnDespawn();
    }
}
