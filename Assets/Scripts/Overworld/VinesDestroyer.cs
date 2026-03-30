using UnityEngine;

public class VinesDestroyer : MonoBehaviour
{
    // Reference to the prompt shown when the player can interact with the vines.
    private Canvas promptText;

    private void Awake()
    {
        // Cache the child Canvas used for the interaction prompt.
        promptText = GetComponentInChildren<Canvas>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Show the prompt only when the player is in range
        // and the required fire ability has been unlocked.
        if (collision.gameObject.CompareTag("Player") && LevelManager.unlockedFire == true)
        {
            promptText.enabled = true;
        }
        else
        {
            promptText.enabled = false;
        }
    }
}