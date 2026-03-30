using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSwitchScript : MonoBehaviour
{
    [Tooltip("The name of the level scene to load when the player enters this trigger.")]
    public string levelSwitchScene;

    [Tooltip("The position where the player should spawn in the next level.")]
    public Vector3 spawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Load the configured level only when the player enters this trigger.
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Loading " + levelSwitchScene);
            SceneManager.LoadScene(levelSwitchScene);
        }
    }
}