using UnityEngine;

public class WinState : IState
{
    public void OnEnter(Enemy enemy)
    {
        enemy.ChangeAnim(Constant.ANIM_WIN);
    }

    public void OnExecute(Enemy enemy)
    {

    }

    public void OnExit(Enemy enemy)
    {
    }
}
