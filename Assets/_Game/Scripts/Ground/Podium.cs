using UnityEngine;

public class Podium : MonoBehaviour
{
    [SerializeField] ColorDataSO colorDataSO;
    [SerializeField] Renderer podiumRenderer;

    private Transform tf;
    public Transform TF => tf = tf == null ? tf = transform : tf;

    private ColorType colorType;

    private void Start()
    {
        tf = transform;
    }

    public void OnInit(ColorType colorType)
    {
        ChangeColor(colorType);
    }

    public void ChangeColor(ColorType colorType)
    {
        this.colorType = colorType;
        podiumRenderer.material = colorDataSO.GetMaterial(colorType);
    }
}
