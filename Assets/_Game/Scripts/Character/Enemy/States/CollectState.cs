using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CollectState : IState
{
    private int maxCollectAmount = Random.Range(6, 18);
    private float stuckThreshold = .15f;
    private float timer;

    public void OnEnter(Enemy enemy)
    {
        timer = 0;
        enemy.SetPriority(100);
        enemy.FindBrick();
        enemy.PlayMoveAnim();
    }


    public void OnExecute(Enemy enemy)
    {   
        if (enemy.GetBrickCount() >= maxCollectAmount)
        {
            OnEnoughBricks(enemy);
        }
        else
        {
            Collect(enemy);
        }
    }

    public void OnExit(Enemy enemy)
    {
    }

    private void OnEnoughBricks(Enemy enemy)
    {
        if (enemy.GetBrickCount() > LevelManager.Instance.Player.GetBrickCount())
        {
            enemy.ChangeState(new ShoveState());
        }
        else
        {
            enemy.ChangeState(new BuildState());
        }
    }

    private void Collect(Enemy enemy)
    {
        // if no valid target, change to build if is carrying bricks, else run to random position
        if (enemy.Target == null || !enemy.Target.gameObject.activeSelf || Mathf.Abs(enemy.Target.position.y - enemy.TF.position.y) > 1f)
        {
            if (!enemy.FindBrick())
            {
                if (enemy.GetBrickCount() > 0)
                {
                    enemy.ChangeState(new BuildState());
                }
                else
                {

                    enemy.FindRandomPosition();
                }
            }
        }

        // unstuck after threshold time
        if (enemy.IsDestination || enemy.GetVelocity().sqrMagnitude < 0.1f)
        {
            timer += Time.deltaTime;

            if (timer >= stuckThreshold)
            {
                enemy.FindRandomPosition();
                timer = 0;
            }
        }
    }
}
