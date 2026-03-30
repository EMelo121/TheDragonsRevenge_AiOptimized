using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DisableEnemies : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The name of the file containing the names of overworld enemies that should be disabled.")]
    private string defeatedEnemiesFile;

    // Stores the names of enemies that were previously recorded as defeated or encountered.
    private List<string> defeatedEnemyNames;

    // Cached full file path built from the configured file name.
    private string path;

    private void Awake()
    {
        // Build the full path once during initialization so it can be reused later.
        path = PathMaker.SetPath(defeatedEnemiesFile);
    }

    private void Start()
    {
        // If the save file does not exist, there is nothing to disable.
        if (!File.Exists(path))
        {
            return;
        }

        // Load the saved enemy names from disk.
        defeatedEnemyNames = LoadSys.ReadListFromJson<string>(defeatedEnemiesFile);
        Debug.LogWarning("Reading Enemies File...");

        // Disable each matching enemy GameObject in the current scene.
        foreach (string enemyName in defeatedEnemyNames)
        {
            GameObject.Find(enemyName).SetActive(false);
        }
    }
}