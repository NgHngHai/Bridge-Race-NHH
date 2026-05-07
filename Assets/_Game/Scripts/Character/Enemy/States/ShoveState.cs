using UnityEngine;

public class ShoveState : IState
{
    private float interval = 0.5f;
    private float timer;
    private Player player;
    public void OnEnter(Enemy enemy)
    {
        enemy.SetPriority(0);
        timer = interval;
        player = LevelManager.Instance.Player;
    }

    public void OnExecute(Enemy enemy)
    {
        timer += Time.deltaTime;

        if (player == null || 
            !player.gameObject.activeSelf || 
            !player.CompareStage(enemy.GetStage()) || 
            player.GetIsBuilding() || 
            player.GetBrickCount() == 0 ||
            enemy.GetBrickCount() <= player.GetBrickCount() ||
            player.IsKnocked)
        {
            OnNoTarget(enemy);
        }
        else
        {
            Chase(enemy);
        }
    }

    public void OnExit(Enemy enemy)
    {
        enemy.SetPriority(100);
    }

    public void OnNoTarget(Enemy enemy)
    {
        enemy.ChangeState(new BuildState());
    }

    public void Chase(Enemy enemy)
    {
        if (timer >= interval)
        {
            timer -= interval;

            enemy.Shove();
            enemy.PlayMoveAnim();
        }
    }
}
