using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyLevelTracker : MonoBehaviour
{
    [Header("Area Level Tracking")]
    [Tooltip("The enemy level assigned to the current area.")]
    public int areaLevel;

    // Cached reference to the active scene.
    private Scene currentArea;

    // Cached name of the active scene.
    private string areaName;

    [Header("Area Flags")]
    [Tooltip("True when the player is currently in the Forest area.")]
    public bool enteredForestLevel;

    [Tooltip("True when the player is currently in the Deep Forest area.")]
    public bool enteredDeepForestLevel;

    public static EnemyLevelTracker Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // AI revision note:
            // The original script used PlayerStats wording in its singleton log messages.
            // Those messages were updated to correctly refer to EnemyLevelTracker.
            Debug.Log("Additional EnemyLevelTracker object found, destroying duplicate...");
            Destroy(gameObject);
            return;
        }

        Debug.Log("A single EnemyLevelTracker object exists in the game...");
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Calculate the area level when the object first initializes.
        CalculateAreaLevel();
    }

    private void OnEnable()
    {
        // Recalculate area level whenever a new scene finishes loading.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CalculateAreaLevel();
    }

    /// <summary>
    /// Determines the enemy level associated with the currently loaded area.
    /// </summary>
    public void CalculateAreaLevel()
    {
        // AI revision note:
        // The original version recalculated this every frame and repeatedly reset areaLevel.
        // This version updates only when needed and assigns the result once.
        currentArea = SceneManager.GetActiveScene();
        areaName = currentArea.name;

        enteredForestLevel = areaName == "Forest";
        enteredDeepForestLevel = areaName == "DeepForest";

        if (enteredForestLevel)
        {
            Debug.Log("Player is in Forest Level, all enemies are Level 2.");
            areaLevel = 2;
        }
        else if (enteredDeepForestLevel)
        {
            Debug.Log("Player is in Deep Forest Level, all enemies are Level 4.");
            areaLevel = 4;
        }
        else
        {
            Debug.Log("Player is in a default area, enemies are Level 1.");
            areaLevel = 1;
        }
    }
}