using UnityEngine;

public enum ColorType
{
    None = 0,
    Gray = 1,
    Red = 2,
    Blue = 3,
    Yellow = 4,
    Green = 5,
    Orange = 6,
    Purple = 7,
}

[CreateAssetMenu(menuName = "ColorDataSO")]
public class ColorDataSO : ScriptableObject
{
    [SerializeField] Material[] materials;

    public Material GetMaterial(ColorType color)
    {
        return materials[(int)color];
    }

    public Color GetColor(ColorType color)
    {
        return materials[(int)color].color;
    }
}
