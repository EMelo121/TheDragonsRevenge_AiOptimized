using System.IO;
using UnityEngine;

public class EnemyTrackerScript : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The name of the file that stores the names of Enemy objects the player has encountered in the overworld.")]
    private string defeatedEnemyNamesFile;

    // Cached full path to the file where encountered enemy names are stored.
    private string path;

    private void Awake()
    {
        // Build and cache the file path once during initialization.
        path = PathMaker.SetPath(defeatedEnemyNamesFile);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle enemy encounters that occur through trigger-based collisions.
        HandleEnemyEncounter(collision.gameObject);

        // Clear stored enemy data when the player reaches an exit.
        if (collision.gameObject.CompareTag("Exit"))
        {
            DeleteSystem.DeleteData(defeatedEnemyNamesFile);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle enemy encounters that occur through physics-based collisions.
        HandleEnemyEncounter(collision.gameObject);

        // Clear stored enemy data when the player reaches an exit.
        if (collision.gameObject.CompareTag("Exit"))
        {
            DeleteSystem.DeleteData(defeatedEnemyNamesFile);
        }
    }

    /// <summary>
    /// Records an enemy encounter and starts that enemy's combat scene.
    /// </summary>
    /// <param name="collidedObject">The GameObject the player collided with.</param>
    private void HandleEnemyEncounter(GameObject collidedObject)
    {
        // Only process objects tagged as enemies.
        if (!collidedObject.CompareTag("Enemy"))
        {
            return;
        }

        // Retrieve the enemy's overworld script so the correct combat scene can be loaded.
        OWEnemyScript enemyScript = collidedObject.GetComponent<OWEnemyScript>();

        // If the tracking file does not exist yet, create it and write the first enemy name.
        if (!File.Exists(path))
        {
            Debug.LogWarning("Creating a new file to store Enemy names...");
            SaveSys.WriteListToJson(defeatedEnemyNamesFile, collidedObject.name);
        }
        else
        {
            Debug.LogWarning("File found at destination. Appending to it...");
            SaveSys.AppendToJsonList(defeatedEnemyNamesFile, collidedObject.name);
        }

        // Delegate combat scene loading to the enemy script.
        enemyScript.LoadCombatScene();
    }
}