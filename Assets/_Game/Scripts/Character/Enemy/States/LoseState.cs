using UnityEngine;

public class LoseState : IState
{
    public void OnEnter(Enemy enemy)
    {
        enemy.ChangeAnim(Constant.ANIM_LOSE);
    }

    public void OnExecute(Enemy enemy)
    {

    }

    public void OnExit(Enemy enemy)
    {
    }
}
