using UnityEngine;

public class GameUnit : MonoBehaviour
{
    public PoolType PoolType;

    private Transform tf;
    public Transform TF
    {
        get
        {
            if (tf == null)
                tf = transform;

            return tf;
        }
    }

    private void OnEnable()
    {
        GlobalEvents.OnPauseGame += OnPauseGame;
        GlobalEvents.OnResumeGame += OnResumeGame;
    }

    private void OnDisable()
    {
        GlobalEvents.OnPauseGame -= OnPauseGame;
        GlobalEvents.OnResumeGame -= OnResumeGame;
    }

    public virtual void OnPauseGame()
    {

    }

    public virtual void OnResumeGame()
    {

    }

    public void Despawn(float delay)
    {
        if (delay > 0)
            Invoke(nameof(OnDespawn), delay);
        else
            OnDespawn();
    }

    protected virtual void OnDespawn()
    {
        SimplePool.Despawn(this);
    }
}
