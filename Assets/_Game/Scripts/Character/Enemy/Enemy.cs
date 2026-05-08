using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    private IState currentState;

    [SerializeField] private Transform target;
    public Transform Target => target;
    public bool IsDestination => Vector3.Distance(tf.position, target.position) < 0.5f;

    private void Update()
    {
        if (IsKnocked || !GameManager.Instance.IsState(GameState.Gameplay))
        {
            return;
        }

        //if (currentState != null)
        {
            currentState?.OnExecute(this);
        }
    }

    public override void OnInit(ColorType colorType, string name = "")
    {
        ChangeState(null);

        base.OnInit(colorType, name);

        RandomAcceleration();
    }

    protected override void OnStartGame()
    {
        base.OnStartGame();
        ChangeState(new CollectState());
    }

    public override void OnWin(int placement, Vector3 podiumPos)
    {
        base.OnWin(placement, podiumPos);
        if (placement == 1)
        {
            ChangeState(new WinState());
        }
        else
        {
            ChangeState(new LoseState());
        }
    }

    public override void OnLose()
    {
        base.OnLose();
        ChangeState(new LoseState());
    }

    public bool FindBrick()
    {
        target = null;

        if (currentStage == null)
        {
            return false;
        }

        PickupBrick nearestBrick = currentStage.GetNearestBrick(tf.position, ColorType);

        if (nearestBrick != null)
        {
            target = nearestBrick.TF;
            SetDestination(target.position);
        }
        else
        {
            target = null;

            if (agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
            ChangeAnim(Constant.ANIM_IDLE);
        }
        return target != null;
    }

    public void FindBestBridge()
    {
        if (currentStage == null)
        {
            Debug.LogError("Enemy is not on a stage.");
            return;
        }

        target = currentStage.GetBestBridgeEndPoint(ColorType);

        if (target != null)
        {
            SetDestination(target.position);
        }
    }

    public void FindRandomPosition()
    {
        Vector3 destination = new Vector3(currentStage.TF.position.x + Random.Range(-Constant.STAGE_LENGTH / 2, Constant.STAGE_LENGTH / 2), 
                                          TF.position.y, 
                                          currentStage.TF.position.z + Random.Range(-Constant.STAGE_WIDTH / 2, Constant.STAGE_WIDTH / 2));

        SetDestination(destination);
    }

    protected override void OnKnockbackEnd()
    {
        base.OnKnockbackEnd();
        ChangeState(new CollectState());
    }

    public void PlayMoveAnim()
    {
        if (target != null)
        {
            ChangeAnim(Constant.ANIM_RUN);
        }
        else
        {
            Debug.LogWarning("Enemy has no target to move to.");
        }
    }

    public void Shove()
    {
        Transform player = LevelManager.Instance.Player.TF;
        if (player == null || !player.gameObject.activeSelf)
        {
            return;
        }

        target = player;
        SetDestination(target.position);
    }

    public override void SetStage(Platform stage)
    {
        base.SetStage(stage);
        //ChangeState(new CollectState());
    }

    public void ChangeState(IState newState)
    {
        //if (currentState != null)
        {
            currentState?.OnExit(this);
        }

        currentState = newState;
        currentState?.OnEnter(this);
    }

    public void SetDestination(Vector3 destination)
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(destination);
        }
    }

    public Vector3 GetVelocity()
    {
        return agent.velocity;
    }

    private void RandomAcceleration()
    {
        agent.acceleration = Random.Range(7f, 12f);
    }
}
