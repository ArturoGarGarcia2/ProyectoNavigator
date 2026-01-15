using UnityEngine;
using System.Collections.Generic;

public class EnemyWakeManager : MonoBehaviour
{
    private List<EnemyBehaviour> inactiveEnemies = new List<EnemyBehaviour>();

    void Start()
    {
        EnemyBehaviour[] allEnemies = FindObjectsOfType<EnemyBehaviour>();

        foreach (var enemy in allEnemies)
        {
            if (enemy.IsInactive())
                inactiveEnemies.Add(enemy);
        }
    }

    void OnEnable()
    {
        ChestHandler.OnAnyChestOpened += WakeRandomEnemy;
    }

    void OnDisable()
    {
        ChestHandler.OnAnyChestOpened -= WakeRandomEnemy;
    }

    void WakeRandomEnemy()
    {
        if (inactiveEnemies.Count == 0)
            return;

        int index = Random.Range(0, inactiveEnemies.Count);
        EnemyBehaviour chosen = inactiveEnemies[index];

        chosen.WakeUp();
        inactiveEnemies.RemoveAt(index);
    }
}
