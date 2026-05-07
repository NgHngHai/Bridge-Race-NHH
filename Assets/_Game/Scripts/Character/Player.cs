using Unity.VisualScripting;
using UnityEngine;

public class Player : Character
{
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotationSpeed = 10f;
    [SerializeField] private float bridgeCheckDistance = .3f;
    [Header("Bridge Check")]
    [SerializeField] private LayerMask blockingLayerMasks;
    [SerializeField] private float fanAngle = 180;

    private Collider[] hits;
    private int hitCount;
    public override void OnInit(ColorType colorType, string name = "")
    {
        base.OnInit(colorType, name);
        TF.rotation = Quaternion.identity;
    }

    public override void OnWin(int placement, Vector3 podiumPos)
    {
        base.OnWin(placement, podiumPos);
        if (placement == 1)
        {
            ChangeAnim(Constant.ANIM_WIN);
        }
        else
        {
            ChangeAnim(Constant.ANIM_LOSE);
        }
    }

    public override void OnLose()
    {
        base.OnLose();
        ChangeAnim(Constant.ANIM_LOSE);
    }

    public void Move(Vector3 input, Quaternion rotation)
    {
        if (IsKnocked || !GameManager.Instance.IsState(GameState.Gameplay))
        {
            return;
        }

        if(input.sqrMagnitude > 0)
        {
            ChangeAnim(Constant.ANIM_RUN);
        }
        else
        {
            ChangeAnim(Constant.ANIM_IDLE);
        }

        if (input != Vector3.zero)
        {
            // check in a fanAngle radius in front of the player
            hitCount = Physics.OverlapSphereNonAlloc(TF.position, bridgeCheckDistance, hits, blockingLayerMasks);
            Vector3 directionToBrick;
            for (int i = 0; i < hitCount; i++)
            {
                // ignore bricks below player to allow going down
                if (hits[i].transform.position.y < TF.position.y)
                {
                    continue;
                }

                Vector3 impactPoint = hits[i].ClosestPoint(TF.position);

                directionToBrick = (impactPoint - TF.position).normalized;
                directionToBrick.y = 0; // ignore vertical direction

                if (directionToBrick != Vector3.zero && Vector3.Angle(TF.forward, directionToBrick) < fanAngle / 2f)
                {
                    BridgeBrick brick = hits[i].GetComponent<BridgeBrick>();
                    if (brick != null && !brick.CompareColor(ColorType))
                    {
                        input.z = 0;
                        break;
                    }
                    else if (brick == null)
                    {
                        input.z = 0;
                        break;
                    }
                }
            }

            tf.rotation = Quaternion.Slerp(tf.rotation, rotation, rotationSpeed * Time.deltaTime);
            agent.Move(input * moveSpeed * Time.deltaTime);
        }
        else
        {
            agent.velocity = Vector3.zero;
        }
    }
}
