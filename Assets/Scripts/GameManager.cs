using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject player;
    public GameObject enemies;
    public GameObject gameUI;
    public GameObject crystals;
    public GameObject pauseMenu;

    public GameObject crystalPrefab; // Reference to the crystal prefab
    public TextMeshProUGUI timerText;     // Timer TextMeshPro element
    public TextMeshProUGUI killCountText; // Kill count TextMeshPro element
    public TextMeshProUGUI crystalCountText; // Crystal count TextMeshPro element

    public int startMinutes = 5;
    public EnemySpawner enemySpawner; // Reference to the EnemySpawner script

    private float timeRemaining;
    private int enemyKillCount = 0;  // Track the number of enemies killed
    private int crystalCount = 0;   // Track the number of crystals collected
    private bool isPaused = false;
    private bool isGameOver = false;
    private bool isTimerRunning = false;

    private void Start()
    {
        canvas.SetActive(true);
        player.SetActive(false);
        enemies.SetActive(false);
        gameUI.SetActive(false);
        pauseMenu.SetActive(false);
        crystals.SetActive(false);

        timeRemaining = startMinutes * 60;
        UpdateTimerDisplay();
        UpdateKillCountDisplay();
        UpdateCrystalCountDisplay();
    }

    public void StartGame()
    {
        canvas.SetActive(false);
        player.SetActive(true);
        enemies.SetActive(true);
        gameUI.SetActive(true);
        crystals.SetActive(true);

        isTimerRunning = true;
        // Notify the EnemySpawner to start spawning enemies
        enemySpawner.InitializeSpawner();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    private void Update()
    {
        // Always check for the "P" key to toggle pause
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
            return;
        }

        // Special attack when player has 10 crystals and right-clicks
        if (crystalCount >= 10 && Input.GetMouseButtonDown(1))
        {
            SpecialAttack();
        }

        if (isPaused || isGameOver) return;

        if (isTimerRunning && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else if (isTimerRunning && timeRemaining <= 0)
        {
            timeRemaining = 0;
            UpdateTimerDisplay();
            TriggerGameOver();
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("Time Left: {0:00}:{1:00}", minutes, seconds);
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        isTimerRunning = false;
        Debug.Log("Game Over! Timer reached 0.");
        canvas.SetActive(true);
        player.SetActive(false);
        enemies.SetActive(false);
        gameUI.SetActive(false);
        crystals.SetActive(false);
    }

    public void IncrementKillCount()
    {
        enemyKillCount++;
        Debug.Log($"Enemy killed! Total kills: {enemyKillCount}");
        UpdateKillCountDisplay();

        // Spawn a crystal for every 10 enemies killed
        if (enemyKillCount % 10 == 0)
        {
            SpawnCrystal();
        }
    }

    private void UpdateKillCountDisplay()
    {
        killCountText.text = $"Enemies Killed: {enemyKillCount}";
    }

    private void UpdateCrystalCountDisplay()
    {
        crystalCountText.text = $"Crystals: {crystalCount}";
    }

    private void SpawnCrystal()
    {
        // Ensure boundaries are aligned with the player's movement
        float leftBoundary = player.GetComponent<PlayerMovement>().leftBoundary;
        float rightBoundary = player.GetComponent<PlayerMovement>().rightBoundary;

        // Randomly spawn within the player's boundaries
        float spawnX = Random.Range(leftBoundary, rightBoundary);
        float spawnY = player.transform.position.y; // Keep it near the player's Y position or ground level

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
        Instantiate(crystalPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"Crystal spawned at {spawnPosition}!");
    }


    public void CollectCrystal()
    {
        crystalCount++;
        Debug.Log($"Crystal collected! Total crystals: {crystalCount}");
        UpdateCrystalCountDisplay();
    }

    private void SpecialAttack()
    {
        crystalCount -= 10;
        UpdateCrystalCountDisplay();
        Debug.Log("Special attack triggered! All enemies destroyed.");

        // Destroy all enemies
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }

        // Reset spawner after a brief pause
        StartCoroutine(PauseAndRestartSpawner(3f)); // 3-second pause
    }

    private IEnumerator PauseAndRestartSpawner(float duration)
    {
        Debug.Log("Pausing enemy spawner...");
        enemySpawner.enabled = false; // Temporarily disable the spawner
        yield return new WaitForSeconds(duration);

        Debug.Log("Restarting enemy spawner...");
        enemySpawner.enabled = true;
        enemySpawner.RestartSpawning(); // Restart the spawning cycle
    }


    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
            pauseMenu.SetActive(false);
        }
        else
        {
            PauseGame();
            pauseMenu.SetActive(true);
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }
}
