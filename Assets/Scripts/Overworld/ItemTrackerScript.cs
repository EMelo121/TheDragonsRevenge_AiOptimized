using System.IO;
using UnityEngine;

public class ItemTrackerScript : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The name of the file that stores collected item names.")]
    private string itemsCollectFile;

    // Cached full file path to the collected items file.
    private string path;

    // Reference to the item interaction prompt canvas.
    private Canvas itemText;

    // Stores the name of the item currently being collected.
    private string itemName;

    // Shared flag used by other scripts to detect when an item has just been collected.
    public static bool itemCollected;

    private void Awake()
    {
        // Build the file path once during initialization.
        path = PathMaker.SetPath(itemsCollectFile);

        // Reset the shared collection flag when this script initializes.
        itemCollected = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Display the item's interaction prompt while the player remains in range.
        itemText = collision.GetComponentInChildren<Canvas>();
        itemText.enabled = true;

        // Only process item collection for objects tagged as Item
        // and only if an item has not already been marked as collected.
        if (collision.gameObject.tag == "Item" && itemCollected == false)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Record the collected item in the save file.
                WriteItemNameToFile(collision);

                // Cache the item name and disable the collected item in the scene.
                itemName = collision.name;
                GameObject.Find(itemName).SetActive(false);
            }
        }
    }

    /// <summary>
    /// Writes the collected item's name to disk so it can be restored later.
    /// </summary>
    /// <param name="collision">The item collider the player is interacting with.</param>
    private void WriteItemNameToFile(Collider2D collision)
    {
        itemCollected = true;

        // Create a new item file if one does not already exist.
        if (!File.Exists(path))
        {
            Debug.LogWarning("Creating a new file to store Item names...");
            SaveSys.WriteListToJson(itemsCollectFile, collision.gameObject.name);
        }
        else
        {
            Debug.LogWarning("Item file found at destination. Appending to it...");
            SaveSys.AppendToJsonList(itemsCollectFile, collision.gameObject.name);
        }
    }
}