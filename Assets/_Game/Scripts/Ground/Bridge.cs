using UnityEngine;
using System.Collections.Generic;

public class Bridge : MonoBehaviour
{
    [SerializeField] private Transform brickContainer;
    [SerializeField] private Transform barrier;
    [SerializeField] private BridgeBrick brickPrefab;
    [SerializeField] private int stepCount = 20;
    public int StepCount => stepCount;
    [SerializeField] private Transform endPoint;
    public Transform EndPoint => endPoint;

    private List<BridgeBrick> brickList = new List<BridgeBrick>();

    public void OnInit()
    {
        InitBridge();
    }

    private void InitBridge()
    {
        Vector3 spawnPos;
        BridgeBrick brick;
        for (int i = 0; i < stepCount; i++)
        {
            spawnPos.x = brickContainer.position.x;
            spawnPos.y = brickContainer.position.y + i * Constant.BRIDGE_BRICK_HEIGHT;
            spawnPos.z = brickContainer.position.z + i * Constant.BRIDGE_BRICK_WIDTH;

            brick = GameObject.Instantiate(brickPrefab, spawnPos, Quaternion.identity, brickContainer);

            brickList.Add(brick);
        }
    }

    public int GetColorCount(ColorType colorType)
    {
        int count = 0;

        for (int i = 0; i < brickList.Count; i++)
        {
            if (brickList[i].CompareColor(colorType))
            {
                count++;
            }
        }

        return count;
    }

    public bool GetFilledState()
    {
        for (int i = 0; i < brickList.Count; i++)
        {
            if (brickList[i].CompareColor(ColorType.None))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsSingleColor()
    {
        ColorType color = brickList[0].CurrentColor;

        for (int i = 0; i < brickList.Count; i++)
        {
            if (!brickList[i].CompareColor(color))
            {
                return false;
            }
        }

        return true;
    }

    public int GetFirstColorCount()
    {
        ColorType color = brickList[0].CurrentColor;
        if (color == ColorType.None)
        {
            return 0;
        }

        int count = 0;

        for (int i = 0; i < brickList.Count; i++)
        {
            if (brickList[i].CompareColor(color))
            {
                count++;
            }
        }

        return count;
    }

    public void ResetBridge()
    {
        for (int i = 0; i < brickList.Count; i++)
        {
            brickList[i].OnInit();
        }
    }

    public void ClearBridge()
    {
        BridgeBrick brick;
        while (brickList.Count > 0)
        {
            brick = brickList[0];
            brickList.Remove(brick);
            Destroy(brick.gameObject);
        }
    }
}
