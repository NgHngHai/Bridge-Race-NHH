using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [Header("Level Objects")]
    // navmesh
    [SerializeField] private NavMeshSurface navMeshSurface;

    // stage prefabs
    [SerializeField] private Platform stagePrefab;
    [SerializeField] private Bridge bridgePrefab;

    // finish line
    [SerializeField] private Bridge finishBridgePrefab;
    [SerializeField] private FinishLine finishLinePrefab;

    [Header("Level Params")]
    [SerializeField] private int stageCount;
    [SerializeField] private int[] stageBridgeCounts;
    [SerializeField] private LevelData levelData;

    [ContextMenu("Build Level")]
    public void BuildLevel()
    {
        levelData.ClearStages();
        Vector3 spawnPoint;
        int bridgeCount = 0;

        // Spawn stages
        for (int i = 0; i < stageCount; i++)
        {
            spawnPoint.x = 0;
            spawnPoint.y = i * Constant.STAGE_HEIGHT_DIF;
            spawnPoint.z = i * (Constant.STAGE_WIDTH + Constant.BRIDGE_LENGTH);

            Platform newStage = Instantiate(stagePrefab, spawnPoint, Quaternion.identity, levelData.transform);
            newStage.OnInit();
            levelData.AddStage(newStage);

            // Spawn bridges
            if (i < stageCount - 1)
            {
                bridgeCount = stageBridgeCounts[i];

                // bridge offset from stage
                spawnPoint.x -= (bridgeCount -1) * Constant.BRIDGE_GAP / 2;
                spawnPoint.z += Constant.STAGE_WIDTH / 2;

                for (int bridgeIndex = 0; bridgeIndex < bridgeCount; bridgeIndex++)
                {
                    Bridge newBridge = Instantiate(bridgePrefab, spawnPoint, Quaternion.identity, levelData.transform);
                    spawnPoint.x += Constant.BRIDGE_GAP;

                    newStage.AddBridge(newBridge);
                    newBridge.OnInit();
                }
            }
            // Spawn finish bridge & finish line
            else if (i == stageCount - 1)
            {
                // bridge offset from stage
                spawnPoint.z += Constant.STAGE_WIDTH / 2;
                Bridge newBridge = Instantiate(finishBridgePrefab, spawnPoint, Quaternion.identity, levelData.transform);

                newStage.AddBridge(newBridge);
                newBridge.OnInit();

                // finish line position
                spawnPoint.y += Constant.FINISH_BRIDGE_HEIGHT_DIF;
                spawnPoint.z += Constant.FINISH_BRIDGE_LENGTH;

                FinishLine finishLine = Instantiate(this.finishLinePrefab, spawnPoint, Quaternion.identity, levelData.transform);
                levelData.FinishLine = finishLine;
            }
        }

        navMeshSurface.BuildNavMesh();
    }
}
