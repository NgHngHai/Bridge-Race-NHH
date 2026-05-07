using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameUnit gameUnit = other.GetComponent<GameUnit>();

        gameUnit?.Despawn(0f);
    }
}
