using UnityEngine;

public class BridgeBrick : MonoBehaviour
{
    [SerializeField] ColorDataSO colorDataSO;
    [SerializeField] Renderer brickRenderer;

    private ColorType currentColor;
    public ColorType CurrentColor => currentColor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constant.TAG_CHARACTER))
        {
            Character character = other.GetComponent<Character>();

            OnCharacterEnter(character);
        }
    }

    public void OnInit()
    {
        ChangeColor(ColorType.None);
    }

    public void ChangeColor(ColorType colorType)
    {
        this.currentColor = colorType;
        brickRenderer.material = colorDataSO.GetMaterial(colorType);
    }

    public bool CompareColor(ColorType colorType)
    {
        return currentColor == colorType;
    }

    private void OnCharacterEnter(Character character)
    {
        character.SetIsBuilding(true);

        if (!character.CompareColorType(currentColor))
        {
            if (character.RemoveBrick())
            {
                ChangeColor(character.ColorType);
            }
        }
    }
}
