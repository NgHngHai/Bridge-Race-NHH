using UnityEngine;

public class CharacterVFX : MonoBehaviour
{
    [SerializeField] private ColorDataSO colorDataSO;
    [SerializeField] private ParticleSystem particle;

    private Transform tf;
    public Transform TF => tf;

    private ColorType colorType;
    public ColorType ColorType => colorType;

    private void Awake()
    {
        tf = transform;
    }

    public void ChangeColor(ColorType colorType)
    {
        this.colorType = colorType;
        var main = particle.main;
        main.startColor = colorDataSO.GetColor(colorType);
    }

    public void PlayVFX()
    {
        particle.Play();
    }
}
