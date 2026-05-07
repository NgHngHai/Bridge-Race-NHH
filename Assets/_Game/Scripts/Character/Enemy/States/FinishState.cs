using UnityEngine;

public class FinishState : IState
{
    private bool isFirst;
    public FinishState(bool isFirst)
    {
        this.isFirst = isFirst;
    }

    public void OnEnter(Enemy enemy)
    {
        if (isFirst)
        {
            enemy.ChangeAnim(Constant.ANIM_WIN);
        }
        else
        {
            enemy.ChangeAnim(Constant.ANIM_LOSE);
        }
    }

    public void OnExecute(Enemy enemy)
    {

    }

    public void OnExit(Enemy enemy)
    {
    }
}
