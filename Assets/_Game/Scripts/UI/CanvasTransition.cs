using UnityEngine;

public class CanvasTransition : UICanvas
{
    [SerializeField] private Animator animator;
    private string currentAnim;

    public void PlayTransition()
    {
        ChangeAnim(Constant.UI_TRANSITION_ANIM);
        Invoke(nameof(OpenMainMenu), Constant.UI_TRANSITION_TIME / 2);
    }

    private void OpenMainMenu()
    {
        UIManager.Instance.OpenUI<CanvasMainMenu>();
    }

    private void ChangeAnim(string animName)
    {
        if (currentAnim != null)
        {
            animator.ResetTrigger(currentAnim);
        }
        currentAnim = animName;
        animator.SetTrigger(currentAnim);
    }
}
