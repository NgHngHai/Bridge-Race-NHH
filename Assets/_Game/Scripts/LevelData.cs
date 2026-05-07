using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] private List<Platform> stages;
    public int StageCount => stages.Count;
    [SerializeField] private FinishLine finishLine;
    public FinishLine FinishLine { get => finishLine; set => finishLine = value; }

    public void AddStage(Platform stage)
    {
        stages.Add(stage);
    }

    public Platform GetStage(int index)
    {
        return stages[index];
    }

    public void ClearStages()
    {
        stages.Clear();
    }
}
