using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Name Data")]
    [SerializeField] private NameDataSO nameDataSO;

    [Header("Level Data")]
    [SerializeField] private List<GameObject> levelDataList = new List<GameObject>();
    private LevelData currentLevelPrefab;

    [Header("Level Objects")]
    // navmesh
    [SerializeField] private NavMeshSurface navMeshSurface;

    // stage prefabs
    [SerializeField] private List<Platform> stages = new List<Platform>();
    [SerializeField] private Platform stagePrefab;
    [SerializeField] private Bridge bridgePrefab;

    // finish line
    [SerializeField] private Bridge finishBridge;
    [SerializeField] private FinishLine finishLine;

    // camera
    [SerializeField] private CameraFollow cameraFollow;

    // enemy + player
    [Header("Enemy & Player")]
    [SerializeField] private float characterSpawnGap = 2.5f;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Player player;
    public Player Player => player;
    private string playerName;

    private List<Character> characters = new List<Character>();

    private int currentLevel;

    private void Start()
    {
        // player is always in scene
        characters.Add(player);
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

    }

    private void OnEnable()
    {
        GlobalEvents.OnPauseGame += OnPauseGame;
        GlobalEvents.OnResumeGame += OnResumeGame;
        GlobalEvents.OnEndGame += OnGameFinish;
    }

    private void OnDisable()
    {
        GlobalEvents.OnPauseGame -= OnPauseGame;
        GlobalEvents.OnResumeGame -= OnResumeGame;
        GlobalEvents.OnEndGame -= OnGameFinish;
    }

    public void OnInit()
    {
        cameraFollow.OnInit();
    }

    public void OnDespawn()
    {
        SimplePool.CollectAll();

        stages.Clear();
        ClearEnemies();

        // deactive to rebuild navmesh
        currentLevelPrefab.gameObject.SetActive(false);
        Destroy(currentLevelPrefab.gameObject);
    }

    public void OnPauseGame()
    {
        Physics.simulationMode = SimulationMode.Script;
    }

    public void OnResumeGame()
    {
        Physics.simulationMode = SimulationMode.FixedUpdate;
    }

    public void LoadLevel(int levelIndex)
    {
        navMeshSurface.RemoveData();

        Platform stage;
        currentLevelPrefab = Instantiate(levelDataList[levelIndex - 1], Vector3.zero, Quaternion.identity).GetComponent<LevelData>();
        for (int i = 0; i < currentLevelPrefab.StageCount; i++)
        {
            stage = currentLevelPrefab.GetStage(i);
            stages.Add(stage);
            stage.OnInit();
        }
        finishLine = currentLevelPrefab.FinishLine;
        finishLine.OnInit();

        navMeshSurface.BuildNavMesh();
        SpawnCharacters();
    }

    public void ReloadLevelDelayed(float delay)
    {
        CancelInvoke(nameof(ReloadLevel));
        Invoke(nameof(ReloadLevel), delay);
    }

    private void ReloadLevel()
    {
        OnDespawn();

        GlobalEvents.OnResumeGame?.Invoke();

        LoadLevel(currentLevel);

        GlobalEvents.OnLoadLevel?.Invoke(currentLevel);
    }

    public void OnGameFinish(Character winner)
    {
        Character secondPlace = null;
        Character thirdPlace = null;

        // look for second & third place (highest stage reached -> most score)
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i] != winner)
            {
                if (secondPlace == null || GetStageIndex(characters[i].GetStage()) > GetStageIndex(secondPlace.GetStage()))
                {
                    thirdPlace = secondPlace;
                    secondPlace = characters[i];
                    continue;
                }
                else if (GetStageIndex(characters[i].GetStage()) == GetStageIndex(secondPlace.GetStage()))
                {
                    if (secondPlace.CompareScoreCount(characters[i].ScoreCount))
                    {
                        thirdPlace = secondPlace;
                        secondPlace = characters[i];
                        continue;
                    }
                }

                if (thirdPlace == null || GetStageIndex(characters[i].GetStage()) > GetStageIndex(thirdPlace.GetStage()))
                {
                    thirdPlace = characters[i];
                }
                else if (GetStageIndex(characters[i].GetStage()) == GetStageIndex(thirdPlace.GetStage()))
                {
                    if (thirdPlace.CompareScoreCount(characters[i].ScoreCount))
                    {
                        thirdPlace = characters[i];
                    }
                }
            }
        }

        winner.OnWin(1, finishLine.GetPodium(0).TF.position);
        finishLine.GetPodium(0).ChangeColor(winner.ColorType);
        if (secondPlace != null)
        {
            secondPlace.OnWin(2, finishLine.GetPodium(1).TF.position);
            finishLine.GetPodium(1).ChangeColor(secondPlace.ColorType);
        }
        if (thirdPlace != null)
        {
            thirdPlace.OnWin(3, finishLine.GetPodium(2).TF.position);
            finishLine.GetPodium(2).ChangeColor(thirdPlace.ColorType);
        }

        cameraFollow.SetTarget(winner.TF);

        GameManager.Instance.ChangeState(GameState.Finish);

        if (winner is Player)
        {
            currentLevel++;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        }
    }

    private void SpawnCharacters()
    {
        Vector3 spawnPoint = new Vector3(-characterSpawnGap * 2.5f, .2f, -6.5f);

        for (int i = 1; i < Constant.CHARACTER_COUNT; i++)
        {
            Enemy newEnemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
            characters.Add(newEnemy);
        }

        ShuffleList(characters);

        nameDataSO.shuffleNames();
        string enemyName;

        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].TF.position = spawnPoint;
            spawnPoint.x += characterSpawnGap;

            if (characters[i] is Enemy)
            {
                enemyName = nameDataSO.GetName(i);
                characters[i].OnInit((ColorType)(i + 2), enemyName);
            }
            else if (characters[i] is Player)
            {
                characters[i].OnInit((ColorType)(i + 2));
            }
        }
    }

    private void ClearEnemies()
    {
        int index = 0;
        Enemy enemy;
        while (characters.Count > 1)
        {
            if (characters[index] is Enemy)
            {
                enemy = characters[index] as Enemy;
                characters.Remove(enemy);
                Destroy(enemy.gameObject);
            }
            else
            {
                index++;
            }
        }
    }

    public void UpdateStageColors(Platform stage, ColorType colorType)
    {
        int stageIndex = 0;

        stageIndex = GetStageIndex(stage);

        if (stageIndex < 0 || stageIndex >= stages.Count)
        {
            Debug.LogError("Invalid stage index: " + stageIndex);
            return;
        }

        stage.AddColor(stage.GetColorCount(), colorType);
        if (stageIndex > 0)
        {
            stages[stageIndex - 1].RemoveColor(colorType);
        }
    }

    public Platform GetStage(int index)
    {
        return stages[index];
    }

    public int GetStageIndex(Platform stage)
    {
        for (int i = 0; i < stages.Count; i++)
        {
            if (stages[i] == stage)
            {
                return i;
            }
        }
        Debug.LogError("Stage not found: " + stage.name);
        return -1;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
        Player.SetName(name, false);
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
