using UnityEngine;

public class BuildState : IState
{
    private int minBrickThreshold = Random.Range(2, 5);

    public void OnEnter(Enemy enemy)
    {
    }

    public void OnExecute(Enemy enemy)
    {
        if (enemy.GetBrickCount() < minBrickThreshold)
        {
            OnOutOfBricks(enemy);  
        }
        else
        {
            Build(enemy);
        }
    }

    public void OnExit(Enemy enemy)
    {
    }

    private void OnOutOfBricks(Enemy enemy)
    {
        enemy.ChangeState(new CollectState());
    }

    private void Build(Enemy enemy)
    {
        enemy.FindBestBridge();
        enemy.PlayMoveAnim();
    }
}
