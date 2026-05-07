using UnityEngine;

public class PoolController : MonoBehaviour
{
    [SerializeField] private PoolAmount[] poolAmounts;

    void Awake()
    {
        SimplePool.ClearPool();

        for (int i = 0; i < poolAmounts.Length; i++)
        {
            SimplePool.Preload(poolAmounts[i].prefab, poolAmounts[i].amount, poolAmounts[i].parent);
        }
    }
}

[System.Serializable]
public class PoolAmount
{
    public GameUnit prefab;
    public int amount;
    public Transform parent;
}

public enum PoolType
{
    PickupBrick = 0,
    CharacterBrick = 1,
    GrayBrick = 2,
}