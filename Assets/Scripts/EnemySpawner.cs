using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;       // Reference to the enemy prefab
    public Transform playerTransform;   // Reference to the player’s Transform
    public float spawnRadius = 10f;     // Minimum distance from the player to spawn enemies
    public int initialSpawnCount = 1;   // Number of enemies to spawn in the first wave

    private int currentWave = 1;        // Tracks the current wave number
    private int activeEnemies = 0;      // Tracks the number of active enemies

    public void InitializeSpawner()
    {
        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
                Debug.Log("Player found and spawner initialized.");
            }
            else
            {
                Debug.LogError("Player not found! Ensure your player GameObject has the 'Player' tag.");
                return;
            }
        }

        SpawnEnemies(initialSpawnCount); // Start the first wave
    }

    public void OnEnemyKilled()
    {
        activeEnemies--; // Decrement the active enemies count
        Debug.Log($"Enemy killed. Active enemies: {activeEnemies}");

        // Check if all enemies are destroyed
        if (activeEnemies <= 0)
        {
            Debug.Log("All enemies killed. Spawning next wave.");
            currentWave++; // Increment the wave
            SpawnEnemies(currentWave); // Spawn the next wave
        }
    }

    public void SpawnEnemies(int count)
    {
        Debug.Log($"Spawning {count} enemies for wave {currentWave}.");

        GameObject enemiesManager = GameObject.Find("EnemiesManager");
        if (enemiesManager == null)
        {
            enemiesManager = new GameObject("EnemiesManager");
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.transform.parent = enemiesManager.transform; // Parent the enemy
            activeEnemies++; // Increment active enemies count
        }
    }

    Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition;

        do
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            float distance = Random.Range(spawnRadius, spawnRadius + 5f);
            spawnPosition = playerTransform.position + new Vector3(Mathf.Cos(angle) * distance, 0, 0); // Only vary the X position

            RaycastHit2D hit = Physics2D.Raycast(spawnPosition + Vector3.up * 10f, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                spawnPosition.y = hit.point.y; // Align Y position with the ground
            }
        } while (Vector3.Distance(spawnPosition, playerTransform.position) < spawnRadius);

        return spawnPosition;
    }

    public void ResetSpawner()
    {
        Debug.Log("Spawner reset. Starting new cycle.");
        currentWave = 1;          // Reset wave to 1
        activeEnemies = 0;        // Reset active enemies count
        SpawnEnemies(currentWave); // Start with the first wave again
    }

    public void RestartSpawning()
    {
        Debug.Log("Spawner restarting. Starting new cycle.");
        currentWave = 1;          // Reset wave to 1
        activeEnemies = 0;        // Reset active enemies count
        SpawnEnemies(currentWave); // Start with the first wave
    }
}
