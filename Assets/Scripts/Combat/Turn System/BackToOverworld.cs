using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToOverworld : MonoBehaviour
{
    [Tooltip("Stores the previously loaded battle scene index so the player can return to it.")]
    private static int previousScene;

    // Cached previous battle scene index from the ReloadBattle system.
    private int storedPreviousScene;

    // References to supporting systems.
    private ReloadBattle reloadBattle;
    private PlayerStats playerStats;

    private void Awake()
    {
        // Cache references used when returning to the overworld or prior battle scene.
        reloadBattle = FindObjectOfType<ReloadBattle>();
        playerStats = FindObjectOfType<PlayerStats>();
    }

    private void Start()
    {
        // Store the most recent battle scene index from the reload system.
        storedPreviousScene = reloadBattle.lastBattle;
        previousScene = storedPreviousScene;
    }

    /// <summary>
    /// Loads the previously stored battle scene, clears saved PlayerPrefs data,
    /// and reapplies player stats for the current level.
    /// </summary>
    public void LoadLastBattle()
    {
        SceneManager.LoadScene(previousScene);
        PlayerPrefs.DeleteAll();

        // AI revision note:
        // The original script called CheckPlayerLevel(), but the refactored combat system
        // now uses ApplyStatsForCurrentLevel() in PlayerStats.
        playerStats.ApplyStatsForCurrentLevel();
    }
}