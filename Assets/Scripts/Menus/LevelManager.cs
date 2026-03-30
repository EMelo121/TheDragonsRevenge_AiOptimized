using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //variables track the game state
    //public static bool isGameOver;
    public static bool isPaused;
    //for testing the vines/fire
    public bool fireTest;
    //Has the player beaten the village boss
    public static bool unlockedFire;

    [Tooltip("UI Pause panel")]
    public GameObject PauseScreen;

    [SerializeField]
    [Tooltip("The name of the file that has the stored names of the Enemy objects the player has collided with previously.")]
    string defeatedEnemyNamesFile;
    string previousPositionFile;
    [SerializeField]
    string itemsCollectFile;
    string path;

    private void Awake()
    {
        //isGameOver = false;
        //deathScreen.SetActive(false);
        isPaused = false;
        if (PauseScreen == null)
        {
            Debug.Log("Unable to Pause");
        }
        PauseScreen.SetActive(false);
        //if(deathScreen == null)
        // {
        // Debug.Log("No death screen assigned");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if(fireTest == true)
        {
            unlockedFire = true;
        }
        //activates the pause panel when the game is paused
        if (isPaused)
        {
            if(PauseScreen == null)
            {
                Debug.Log("Unable to Pause");
                isPaused = false;
                return;
            }
            else
            {
                PauseScreen.SetActive(true);
                Time.timeScale = 0;
            }
            
        }
        //activates the game over screen when the player dies
        //if (isGameOver)
        //{
           // deathScreen.SetActive(true);
        //}
    }
    public void StartMenu()
    {
        isPaused = false;
        Time.timeScale = 1;
        DeleteSystem.DeleteData(defeatedEnemyNamesFile);
        DeleteSystem.DeleteData(itemsCollectFile);
        SceneManager.LoadScene("StartMenu");
        PlayerPrefs.DeleteAll(); //Jevon's Code - Used to reset the player's stats after completing the game and hitting the main menu
        PlayerStats.Instance.StartingLevel(); //Jevon's Code - Used to reset the player's level back to 1 and apply their basic stats accordingly
        EnemyLevelTracker.Instance.enteredDeepForestLevel = false;
    }
    public void QuitGame()
    {
        //closes the build
        Application.Quit();
    }
    public void Continue()
    {
        PauseScreen.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
    }
    public void StartGame()
    {
        path = PathMaker.SetPath(previousPositionFile);
        unlockedFire = false;
        SceneManager.LoadScene("Intro");
        DeleteSystem.DeleteData(defeatedEnemyNamesFile);
        DeleteSystem.DeleteData(previousPositionFile);
        DeleteSystem.DeleteData(itemsCollectFile);
        PlayerPrefs.DeleteAll(); //Deletes the player's stats once the game has reloaded from the End Screen Scene - Jevon
        PlayerStats.Instance.UpdateLevel();
    }

}
