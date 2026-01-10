using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject chestPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Cantidad de cofres")]
    [SerializeField] private int chestCount = 5;

    private PlayerController pc;
    private Difficulty difficulty;

    void Awake()
    {
        pc = FindFirstObjectByType<PlayerController>();
        if (pc == null)
        {
            Debug.LogError("PlayerController no encontrado", this);
            return;
        }

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

        spawnPoints = Shuffle(spawnPoints);

        for (int i = 0; i < spawnPoints.Length; i++){
        // Debug.Log("chestC: "+chestCount);
        // Debug.Log("chestC + enemies: "+(chestCount+GetEnemyCountByDifficulty()));
            if(i < chestCount)
                Instantiate(chestPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            else if(i >= chestCount && i < (chestCount + GetEnemyCountByDifficulty()))
                Instantiate(enemyPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
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
