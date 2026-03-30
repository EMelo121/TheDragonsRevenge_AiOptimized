using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HoardUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The name of the file containing collected item names.")]
    private string itemsCollectFile;

    // Stores the list of collected item names loaded from disk.
    private List<string> items;

    // Cached full file path for the collected items file.
    private string path;

    // Reference to the UI image controlled by this script.
    private Image image;

    private void Awake()
    {
        // Cache the Image component attached to this UI object.
        image = GetComponent<Image>();
    }

    private void Start()
    {
        // Build the file path once when the script starts.
        path = PathMaker.SetPath(itemsCollectFile);

        // If no item file exists yet, there is no UI state to restore.
        if (!File.Exists(path))
        {
            Debug.LogWarning("Items file not found...");
            return;
        }

        Debug.LogWarning("Reading Items File...");
        items = LoadSys.ReadListFromJson<string>(itemsCollectFile);

        // Enable this UI image if its name appears in the collected items file.
        if (items.Contains(name))
        {
            image.enabled = true;
        }
    }

    private void Update()
    {
        // When an item is collected, reload the saved list and update the UI if needed.
        if (ItemTrackerScript.itemCollected == true)
        {
            Debug.LogWarning("Updating hoard...");
            items = LoadSys.ReadListFromJson<string>(itemsCollectFile);

            if (items.Contains(name))
            {
                image.enabled = true;
                ItemTrackerScript.itemCollected = false;
            }
        }
    }
}