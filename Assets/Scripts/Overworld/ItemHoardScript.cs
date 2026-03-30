using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemHoardScript : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The name of the file containing collected overworld item names.")]
    private string itemsCollectFile;

    // Stores the names of items that have already been collected.
    private List<string> items;

    // Cached full file path to the collected items file.
    private string path;

    private void Awake()
    {
        // Build the full path once during initialization.
        path = PathMaker.SetPath(itemsCollectFile);
    }

    private void Start()
    {
        // If the collected items file exists, disable all matching overworld item objects.
        if (File.Exists(path))
        {
            Debug.LogWarning("Reading Items File...");
            items = LoadSys.ReadListFromJson<string>(itemsCollectFile);

            foreach (string itemName in items)
            {
                GameObject.Find(itemName).SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Items file not found...");
            return;
        }
    }
}