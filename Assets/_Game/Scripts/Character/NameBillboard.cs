using UnityEngine;

public class NameBillboard : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    Transform tf;

    private void Start()
    {
        tf = transform;
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        tf.rotation = cameraTransform.rotation;
    }
}
