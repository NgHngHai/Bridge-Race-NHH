using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private List<Bridge> bridges = new List<Bridge>();
    [SerializeField] private Vector2Int gridDimensions;
    [SerializeField] private ColorType[,] pickupBrickGrid;
    [Tooltip("Size of each grid cell")]
    [SerializeField] private float gridSize = 1;
    [Tooltip("Bottom left corner of the grid")]
    [SerializeField] private Transform gridAnchor;
    [SerializeField] private float spawnTimer;

    private float timer;
    private int maxBricks;
    private Transform tf;
    public Transform TF => tf;

    [SerializeField]private int brickCount;
    private List <PickupBrick> pickupBricks = new List<PickupBrick>();
    // dictionary to keep track of unlocked colors
    private Dictionary<ColorType, int> unlockedColors = new Dictionary<ColorType, int>();
    // queue for next brick spawn timer
    private Queue<float> brickTimerQueue = new Queue<float>();

    private void Update()
    {
        if (!GameManager.Instance.IsState(GameState.Pause))
        {
            timer += Time.deltaTime;
            if (brickTimerQueue.Count > 0)
            {
                SpawnNewBrick();

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constant.TAG_CHARACTER))
        {
            Character character = other.GetComponent<Character>();

            OnCharacterEnter(character);
        }
    }

    public void OnInit()
    {
        tf = transform;
        maxBricks = gridDimensions.x * gridDimensions.y;
        pickupBrickGrid = new ColorType[gridDimensions.x, gridDimensions.y];
        brickCount = 0;
        
        for (int i = 0; i < bridges.Count; i++)
        {
            bridges[i].OnInit();
        }

        timer = Time.time;
    }

    public void OnDespawn()
    {
        ClearGrid();
        ClearBridge();
    }

    private void SpawnBrick(ColorType colorType, Vector3 position)
    {
        brickCount++;

        PickupBrick pickupBrick = SimplePool.Spawn<PickupBrick>(PoolType.PickupBrick, position, Quaternion.identity);
        pickupBrick.OnInit(colorType, this);
        pickupBricks.Add(pickupBrick);
    }

    public void MassSpawnBricks(ColorType colorType)
    {
        int amount = maxBricks / Constant.CHARACTER_COUNT;

        for (int i = 0; i < amount; i++)
        {
            SpawnNewBrick(colorType, true);
        }
    }

    private void SpawnNewBrick(ColorType spawnColor = ColorType.None, bool initSpawn = false)
    {
        if (brickCount >= maxBricks || unlockedColors.Count == 0)
            return;

        if (initSpawn || timer - spawnTimer > brickTimerQueue.Peek())
        {
            if (!initSpawn)
            {
                timer -= spawnTimer;
                brickTimerQueue.Dequeue();
            }

            if (spawnColor == ColorType.None)
            {
                spawnColor = GetRandomColor();
            }

            Vector2Int spawnIndex = GetRandomIndex();
            Vector3 spawnPosition = ConvertGridToWorld(spawnIndex);

            pickupBrickGrid[spawnIndex.x, spawnIndex.y] = spawnColor;
            SpawnBrick(spawnColor, spawnPosition);
        }
    }

    public void ReplaceBrick(Vector3 position, PickupBrick brick)
    {
        brickCount--;

        Vector2Int gridIndex = ConvertWorldToGrid(position);

        pickupBrickGrid[gridIndex.x, gridIndex.y] = ColorType.None;
        pickupBricks.Remove(brick);

        EnqueueBrickCooldown();
    }

    private void EnqueueBrickCooldown()
    {
        brickTimerQueue.Enqueue(Time.time);
    }

    private void ClearGrid()
    {
        PickupBrick brick;

        while (pickupBricks.Count > 0) 
        {
            brick = pickupBricks[0];
            pickupBricks.Remove(brick);
            brick.Despawn(0);
        }

        brickTimerQueue.Clear();
        unlockedColors.Clear();
        brickCount = 0;
    }

    private ColorType GetRandomColor()
    {
        int randomIndex = Random.Range(0, unlockedColors.Count);
        int index = 0;
        ColorType colorType = ColorType.Red;
        foreach (var color in unlockedColors)
        {
            if (index == randomIndex)
            {
                colorType = color.Key;
            }
            index++;
        }

        return colorType;
    }

    public void AddColor(int index, ColorType colorType)
    {
        unlockedColors.Add(colorType, index);
    }

    public void RemoveColor(ColorType colorType)
    {
        unlockedColors.Remove(colorType);
    }

    public int GetColorCount()
    {
        return unlockedColors.Count;
    }

    public PickupBrick GetNearestBrick(Vector3 position, ColorType colorType)
    {
        PickupBrick nearestBrick = null;
        float nearestDistance = Mathf.Infinity;

        for (int i = 0; i < pickupBricks.Count; i++)
        {
            if (pickupBricks[i].ColorType == colorType)
            {
                float distance = Vector3.Distance(position, pickupBricks[i].TF.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestBrick = pickupBricks[i];
                }
            }
        }

        return nearestBrick;
    }

    public Transform GetBestBridgeEndPoint(ColorType colorType)
    {
        Bridge bestBridge = null;
        int maxColorCount = 0;
        int minBrickCount;
        //float nearestDistance = Mathf.Infinity;

        // bestBridge = most color
        for (int i = 0; i < bridges.Count; i++)
        {
            if (bridges[i].GetColorCount(colorType) > maxColorCount)
            {
                maxColorCount = bridges[i].GetColorCount(colorType);
                bestBridge = bridges[i];
            }
        }

        // if null, bestBridge = filled
        if (bestBridge == null)
        {
            minBrickCount = 1000;
            for (int i = 0; i < bridges.Count; i++)
            {
                if (bridges[i].GetFilledState())
                {
                    if (bridges[i].IsSingleColor())
                    {
                        bestBridge = bridges[i];
                    }
                }
            }
        }

        // if null, bestBridge = least bricks placed
        if (bestBridge == null)
        {
            minBrickCount = 1000;
            for (int i = 0; i < bridges.Count; i++)
            {
                if (bridges[i].GetFirstColorCount() < minBrickCount)
                {
                    minBrickCount = bridges[i].GetFirstColorCount();
                    bestBridge = bridges[i];
                }
            }
        }

        return bestBridge.EndPoint;
    }

    private Vector2Int GetRandomIndex()
    {
        Vector3 randomPosition = Vector3.zero;
        Vector2Int randomGridIndex = Vector2Int.zero;
        ColorType colorInCell;

        if (brickCount >= maxBricks)
            return Vector2Int.one * -1;

        do
        {

            randomGridIndex.x = Random.Range(0, gridDimensions.x);
            randomGridIndex.y = Random.Range(0, gridDimensions.y);

            colorInCell = pickupBrickGrid[randomGridIndex.x, randomGridIndex.y];
        }
        while (colorInCell != ColorType.None);

        return randomGridIndex;
    }

    private Vector3 ConvertGridToWorld(Vector2Int gridIndex)
    {
        Vector3 worldPos = Vector3.zero;
        worldPos.x = gridAnchor.position.x + gridIndex.x * gridSize;
        worldPos.y = gridAnchor.position.y;
        worldPos.z = gridAnchor.position.z + gridIndex.y * gridSize;

        return worldPos;
    }

    private Vector2Int ConvertWorldToGrid(Vector3 worldPosition)
    {
        Vector2Int gridIndex = Vector2Int.zero;
        gridIndex.x = Mathf.RoundToInt((worldPosition.x - gridAnchor.position.x) / gridSize);
        gridIndex.y = Mathf.RoundToInt((worldPosition.z - gridAnchor.position.z) / gridSize);
        return gridIndex;                
    }

    public void AddBridge(Bridge bridge)
    {
        bridges.Add(bridge);
    }

    public void ClearBridge()
    {
        Bridge bridge;
        while (bridges.Count > 0)
        {
            bridge = bridges[0];
            bridges.Remove(bridge);
            Destroy(bridge.gameObject);
        }
    }

    private void OnCharacterEnter(Character character)
    {
        character.SetStage(this);
        character.SetIsBuilding(false);

        if (!unlockedColors.ContainsKey(character.ColorType))
        {
            LevelManager.Instance.UpdateStageColors(this, character.ColorType);
            MassSpawnBricks(character.ColorType);
        }
    }
}
