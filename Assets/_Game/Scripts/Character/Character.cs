using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    [SerializeField] private TextMeshPro nameDisplay;
    [SerializeField] private Animator animator;
    [SerializeField] private Renderer characterRenderer;
    [SerializeField] private ColorDataSO colorDataSO;
    [SerializeField] private Transform brickContainer;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private CharacterVFX characterVFX;

    // Temp, to test prefab
    //[SerializeField] private CharacterBrick characterBrickPrefab;

    [SerializeField] private ColorType colorType;
    [Header("Knockback params")]
    //[SerializeField] private float pushForce = 5f;
    [SerializeField] private float pushTime = .5f;
    //[SerializeField] private float dampening = .8f;

    public ColorType ColorType => colorType;

    private Stack<CharacterBrick> brickStack = new Stack<CharacterBrick>();
    private string currentAnim;
    protected Platform currentStage;
    private int scoreCount;
    public int ScoreCount => scoreCount;
    protected Transform tf;
    public Transform TF => tf;
    private bool isKnocked = false;
    public bool IsKnocked => isKnocked;

    private bool isBuilding = false;

    private float knockStartTime;
    private float knockRemainingTime;

    private string characterName;

    private Coroutine c;

    protected virtual void Awake()
    {
        tf = transform;
    }

    private void OnEnable()
    {
        GlobalEvents.OnStartGame += OnStartGame;
        GlobalEvents.OnPauseGame += OnPauseGame;
        GlobalEvents.OnResumeGame += OnResumeGame;
    }

    private void OnDisable()
    {
        GlobalEvents.OnStartGame -= OnStartGame;
        GlobalEvents.OnPauseGame -= OnPauseGame;
        GlobalEvents.OnResumeGame -= OnResumeGame;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Constant.TAG_CHARACTER))
        {
            return;
        }

        Character character = other.GetComponent<Character>();

        if (CompareBrickCount(character.GetBrickCount()))
        {
            character.Knocked();
        }
    }

    public virtual void OnInit(ColorType colorType, string name = "")
    {
        if (string.IsNullOrEmpty(name))
        {
            if (string.IsNullOrEmpty(characterName))
            {
                SetName("Player", true);
            }
        }
        else
        {
            SetName(name, false);
        }

        ToggleAgent(false);
        ChangeColor(colorType);
        ClearBrick(false);
        ChangeAnim(Constant.ANIM_IDLE);

        ChangeColor(colorType);

        characterVFX.ChangeColor(colorType);
    }

    protected virtual void OnStartGame()
    {
        ToggleAgent(true);
    }

    protected virtual void OnPauseGame()
    {
        animator.speed = 0;
        knockRemainingTime = pushTime - (Time.time - knockStartTime);
        StopAgent(true);
    }

    protected virtual void OnResumeGame()
    {
        animator.speed = 1;
        Invoke(nameof(OnKnockbackEnd), knockRemainingTime);
        StopAgent(false);
    }

    public void AddBrick(Vector3 pickupPos)
    {
        scoreCount++;
        CharacterBrick characterBrick = SimplePool.Spawn<CharacterBrick>(PoolType.CharacterBrick, pickupPos, Random.rotation);
        brickStack.Push(characterBrick);
        
        float offset = brickStack.Count * Constant.BRICK_HEIGHT;

        characterBrick.TF.SetParent(brickContainer);
        characterBrick.OnInit(this, colorType, offset, pickupPos);
    }

    public bool RemoveBrick()
    {
        if (brickStack.Count > 0)
        {
            CharacterBrick characterBrick = brickStack.Pop();
            characterBrick.Despawn(0);
            return true;
        }
        return false;
    }

    public void ClearBrick(bool isKnocked, ColorType colorType = ColorType.Gray)
    {
        Vector3 brickPosition;

        if (isKnocked)
        {
            GrayBrick brick;
            scoreCount = Mathf.Max(0, scoreCount - brickStack.Count);

            while (brickStack.Count > 0) { 
                RemoveBrick();

                brickPosition.x = tf.position.x + Random.Range(-1f, 1f);
                brickPosition.y = tf.position.y + Random.Range(1.5f, 2.5f);
                brickPosition.z = tf.position.z + Random.Range(-1f, 1f);

                brick = SimplePool.Spawn<GrayBrick>(PoolType.GrayBrick, brickPosition, Random.rotation);
                brick.OnInit(colorType);
                brick.ApplyForce((brickPosition - tf.position).normalized);
            }
        }
        else
        {
            while (brickStack.Count > 0)
            { 
                RemoveBrick();
            }
        }
    }

    public void Knocked()
    {
        if (isKnocked || isBuilding)
            return;

        isKnocked = true;
        knockStartTime = Time.time;

        ClearBrick(true);
        ChangeAnim(Constant.ANIM_FALL);

        agent.velocity = Vector3.zero;
        StopAgent(true);

        Invoke(nameof(OnKnockbackEnd), pushTime);
    }

    protected virtual void OnKnockbackEnd()
    {
        if(!GameManager.Instance.IsState(GameState.Gameplay))
        {
            return;
        }

        StopAgent(false);
        agent.ResetPath();
        isKnocked = false;
        ChangeAnim(Constant.ANIM_IDLE);
    }

    public virtual void OnWin(int placement, Vector3 podiumPos)
    {
        agent.Warp(podiumPos);
        tf.rotation = Quaternion.Euler(0, 180, 0);
        ClearBrick(true, colorType);
        StopAgent(true);
        agent.ResetPath();
    }

    public virtual void OnLose()
    {
        ClearBrick(false);
        StopAgent(true);
        agent.ResetPath();
    }

    protected void StopAgent(bool value)
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = value;    
        }
    }

    public void ChangeColor(ColorType colorType)
    {
        this.colorType = colorType;
        characterRenderer.material = colorDataSO.GetMaterial(colorType);
    }

    public void ChangeAnim(string animName)
    {
        if (animName == currentAnim)
            return;
        if (currentAnim != null)
        {
            animator.ResetTrigger(currentAnim);
        }
        currentAnim = animName;
        animator.SetTrigger(currentAnim);
    }

    public bool CompareColorType(ColorType colorType)
    {
        return this.colorType == colorType;
    }

    public void SetIsBuilding(bool value)
    {
        isBuilding = value;
    }

    public bool GetIsBuilding()
    {
        return isBuilding;
    }

    public bool CompareBrickCount(int count)
    {
        return brickStack.Count > count;
    }

    public int GetBrickCount()
    {
        return brickStack.Count; 
    }

    public virtual void SetStage(Platform stage)
    {
        currentStage = stage;
    }

    public Platform GetStage()
    {
        return currentStage;
    }

    public bool CompareStage(Platform stage)
    {
        return currentStage == stage;
    }

    public bool CompareScoreCount(int score)
    {
        return scoreCount < score;
    }

    public void SetPriority(int priority)
    {
        agent.avoidancePriority = priority;
    }

    public void SetName(string name, bool isDefault)
    {
        if (!isDefault)
        {
            characterName = name;
        }
        nameDisplay.text = name;
    }

    public void ToggleAgent(bool value)
    {
        agent.enabled = value;
    }

    public void PlayVFX(Vector3 position)
    {
        characterVFX.TF.position = position;
        characterVFX.PlayVFX();
    }
}