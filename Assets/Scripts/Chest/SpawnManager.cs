using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    private int chestIndex = 0;
    private int enemyIndex = 0;

    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject chestPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Cantidad de cofres")]
    [SerializeField] private int chestCount = 5;

    private PlayerController pc;
    private Difficulty difficulty;

    [Header("Debug Mode")]
    [SerializeField] private bool debug = false;

    private void OnEnable()
    {
        ChestHandler.OnAnyChestOpened += WakeOneEnemy;
    }

    private void OnDisable()
    {
        ChestHandler.OnAnyChestOpened -= WakeOneEnemy;
    }

    void Awake()
    {
        pc = FindFirstObjectByType<PlayerController>();
        if (pc == null)
            return;

        difficulty = pc.GetDifficulty();
    }

    void Start()
    {
        SpawnAll();
    }

    void SpawnAll()
    {
        if (spawnPoints.Length == 0)
            return;

        if (!debug)
            spawnPoints = Shuffle(spawnPoints);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform spawnPoint = spawnPoints[i];
            Transform parent = spawnPoint.parent;

            if (i < chestCount)
            {
                chestIndex++;

                Vector3 chestPos = spawnPoint.position + Vector3.down * 0.5f;
                Quaternion chestRot = spawnPoint.rotation * Quaternion.Euler(0f, -90f, 0f);

                GameObject chest = Instantiate(
                    chestPrefab,
                    chestPos,
                    chestRot,
                    parent
                );

                chest.name = $"Chest_{chestIndex:D2}";
            }
            else if (i < chestCount + GetEnemyCountByDifficulty())
            {
                enemyIndex++;

                GameObject enemy = Instantiate(
                    enemyPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation,
                    transform
                );

                enemy.name = $"Enemy_{enemyIndex:D2}";
            }
        }
    }

    public void WakeOneEnemy() {
        foreach (Transform child in transform) {
            EnemyBehaviour enemy = child.GetComponent<EnemyBehaviour>();

            if (enemy != null && enemy.CanBeForcedAwake()) {
                enemy.WakeUp();
                return;
            }else{
                Debug.Log(enemy+" ya despierto");
            }
        }
    }

    int GetEnemyCountByDifficulty()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                return 4;
            case Difficulty.Normal:
                return 7;
            case Difficulty.Hard:
                return 10;
            default:
                return 0;
        }
    }

    Transform[] Shuffle(Transform[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            int rnd = Random.Range(i, list.Length);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }

        return list;
    }
}
