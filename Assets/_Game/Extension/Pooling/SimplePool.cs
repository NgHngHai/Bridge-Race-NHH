using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public static class SimplePool
{
    private static Dictionary<PoolType, Pool> poolInstance = new Dictionary<PoolType, Pool>();

    // Init new pool
    public static void Preload(GameUnit prefab, int amount, Transform parent)
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is null.");
            return;
        }

        if (!poolInstance.ContainsKey(prefab.PoolType) || poolInstance[prefab.PoolType] == null)
        {
            Pool p = new Pool();
            p.Preload(prefab, amount, parent);
            poolInstance[prefab.PoolType] = p;
        }
    }

    // Spawn unit from existing pool
    public static T Spawn<T>(PoolType poolType, Vector3 position, Quaternion rotation) where T : GameUnit
    {
        if (!poolInstance.ContainsKey(poolType))
        {
            Debug.LogError(poolType + " hasn't been preloaded.");
            return null;
        }

        return poolInstance[poolType].Spawn(position, rotation) as T;
    }

    // Despawn unit to existing pool
    public static void Despawn(GameUnit unit)
    {
        if (!poolInstance.ContainsKey(unit.PoolType))
        {
            Debug.LogError(unit.PoolType + " hasn't been preloaded.");
            return;
        }

        poolInstance[unit.PoolType].Despawn(unit);
    }

    // Collect all actives to an existing pool
    public static void Collect(PoolType poolType)
    {
        if (!poolInstance.ContainsKey(poolType))
        {
            Debug.LogError(poolType + " hasn't been preloaded.");
            return;
        }

        poolInstance[poolType].Collect();
    }

    // Collect all actives
    public static void CollectAll()
    {
        foreach (var item in poolInstance.Values)
        {
            item.Collect();
        }
    }

    // Destroy all actives and inactives of an existing pool
    public static void Release(PoolType poolType)
    {
        Debug.Log("Release " + poolType);

        if (!poolInstance.ContainsKey(poolType))
        {
            Debug.LogError(poolType + " hasn't been preloaded.");
            return;
        }

        poolInstance[poolType].Release();
    }

    // Destroy all actives and inactives
    public static void ReleaseAll()
    {
        foreach (var item in poolInstance.Values)
        {
            item.Release();
        }
    }

    public static void ClearPool()
    {
        poolInstance.Clear();
    }
}

public class Pool
{
    Transform parent;
    GameUnit prefab;
    
    // List for inactive units
    Queue<GameUnit> inactives = new Queue<GameUnit>();
    // List for active units
    List<GameUnit> actives = new List<GameUnit>();

    // Init pool
    public void Preload(GameUnit prefab, int amount, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < amount; i++)
        {
            Despawn(GameObject.Instantiate(prefab, parent));
        }
    }

    // Spawn a unit from pool
    public GameUnit Spawn(Vector3 position, Quaternion rotation)
    {
        GameUnit unit;

        if (inactives.Count <= 0)
        {
            unit = GameObject.Instantiate(prefab);
        }
        else
        {
            unit = inactives.Dequeue();
        }

        unit.TF.SetPositionAndRotation(position, rotation);
        //unit.TF.SetParent(newParent);
        actives.Add(unit);
        unit.gameObject.SetActive(true);

        return unit;
    }

    // Despawn a unit to pool
    public void Despawn(GameUnit unit)
    {
        if (unit != null && unit.gameObject.activeSelf)
        {
            actives.Remove(unit);
            inactives.Enqueue(unit);
            unit.gameObject.SetActive(false);
            unit.TF.SetParent(parent);
            unit.TF.position = Vector3.zero;
        }
    }

    // Despawn all actives
    public void Collect()
    {
        while (actives.Count > 0)
        {
            Despawn(actives[0]);
        }
    }

    // Destroy all actives and inactives
    public void Release()
    {
        Debug.Log("Release " + prefab.PoolType);

        Collect();
        while (inactives.Count > 0)
        {
            GameObject.Destroy(inactives.Dequeue().gameObject);
        }

        inactives.Clear();
    }
}