using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("The player's overworld movement speed.")]
    public float moveSpeed = 3f;

    // Reference to the player's Animator component.
    private Animator animator;

    [Header("Player Spawner")]
    // Name of the file used to store the player's previous position.
    private string previousPositionFile;

    // Stores the coordinates loaded from the saved position file.
    private float[] previousCoords;

    // Cached full path to the saved position file.
    private string path;

    // Reference to the exit trigger that defines the next spawn point.
    public LevelSwitchScript exitLevel;

    [Header("Enemy and Vines Tracking")]
    [SerializeField]
    [Tooltip("The name of the file that tracks defeated enemies or removed overworld obstacles.")]
    private string defeatedEnemyNamesFile;

    [Header("Burning Vines")]
    [Tooltip("The fire effect shown when using the upward-facing burn action.")]
    public GameObject fireUp;

    [Tooltip("The fire effect shown when using the downward-facing burn action.")]
    public GameObject fireDown;

    // Prevents the fire interaction from being used repeatedly while an effect is already active.
    private bool usedFire;

    private void Awake()
    {
        // Build the path to the player's saved position file.
        path = PathMaker.SetPath(previousPositionFile);

        // Cache commonly used component references.
        animator = GetComponent<Animator>();

        // Ensure fire effects start disabled.
        fireUp.SetActive(false);
        fireDown.SetActive(false);
    }

    private void Start()
    {
        // If a previous position file exists, restore the player to that location.
        if (File.Exists(path))
        {
            previousCoords = LoadSys.ReadVector3FromJson(previousPositionFile);
            transform.position = new Vector3(previousCoords[0], previousCoords[1], previousCoords[2]);

            DeleteSystem.DeleteData(previousPositionFile);
            Debug.LogWarning("Deleting Player spawnpoint from file...");
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleAnimations();
        HandlePause();
    }

    /// <summary>
    /// Processes the player's overworld movement input.
    /// </summary>
    private void HandleMovement()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            transform.Translate(
                new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) *
                moveSpeed *
                Time.deltaTime
            );
        }
    }

    /// <summary>
    /// Opens the pause state when the player presses Escape.
    /// </summary>
    private void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && LevelManager.isPaused == false)
        {
            LevelManager.isPaused = true;
        }
    }

    /// <summary>
    /// Triggers directional movement animations based on keyboard input.
    /// </summary>
    private void HandleAnimations()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            animator.SetTrigger("flyRight");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.SetTrigger("flyLeft");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetTrigger("flyForward");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetTrigger("flyBack");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Save the player's current position before entering combat.
        if (collision.gameObject.CompareTag("Enemy"))
        {
            WritePlayerPositionToFile();
        }

        // Save the designated spawn point when transitioning through an exit.
        if (collision.gameObject.CompareTag("Exit"))
        {
            exitLevel = collision.GetComponent<LevelSwitchScript>();

            DeleteSystem.DeleteData(previousPositionFile);
            Debug.LogWarning("Deleting Player spawnpoint from file...");

            Vector3 spawnPoint = exitLevel.spawnPoint;
            SaveSys.WriteVector3ToJson(previousPositionFile, spawnPoint);

            Debug.LogWarning("Writing Player spawnpoint to file...");
        }
    }

    /// <summary>
    /// Writes the player's current position to disk so it can be restored later.
    /// </summary>
    private void WritePlayerPositionToFile()
    {
        Vector3 spawnPoint = transform.position;
        SaveSys.WriteVector3ToJson(previousPositionFile, spawnPoint);
        Debug.LogWarning("Writing Player spawnpoint to file...");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Burn upward-facing vines when the player has unlocked fire and presses E.
        if (collision.gameObject.CompareTag("Vines") && LevelManager.unlockedFire == true)
        {
            if (Input.GetKeyDown(KeyCode.E) && usedFire == false)
            {
                StartCoroutine(UseFireAndDestroyVines(collision.gameObject, true));
            }
        }

        // Burn downward-facing vines when the player has unlocked fire and presses E.
        if (collision.gameObject.CompareTag("Vines2") && LevelManager.unlockedFire == true)
        {
            if (Input.GetKeyDown(KeyCode.E) && usedFire == false)
            {
                StartCoroutine(UseFireAndDestroyVines(collision.gameObject, false));
            }
        }
    }

    /// <summary>
    /// Plays the appropriate fire effect, records the destroyed vines, and removes them from the scene.
    /// </summary>
    /// <param name="targetObject">The vine object being destroyed.</param>
    /// <param name="useUpwardFire">True for the upward fire effect, false for the downward fire effect.</param>
    private IEnumerator UseFireAndDestroyVines(GameObject targetObject, bool useUpwardFire)
    {
        usedFire = true;

        if (useUpwardFire)
        {
            animator.SetTrigger("flyBack");
            fireUp.SetActive(true);
        }
        else
        {
            animator.SetTrigger("flyForward");
            fireDown.SetActive(true);
        }

        Debug.LogWarning("Activating Fire...");
        Debug.LogWarning("Writing Vines To File...");
        SaveSys.WriteListToJson(defeatedEnemyNamesFile, targetObject.name);

        Destroy(targetObject);

        yield return new WaitForSeconds(1f);

        if (useUpwardFire)
        {
            fireUp.SetActive(false);
        }
        else
        {
            fireDown.SetActive(false);
        }

        Debug.LogWarning("Deactivating Fire...");
        usedFire = false;
    }
}