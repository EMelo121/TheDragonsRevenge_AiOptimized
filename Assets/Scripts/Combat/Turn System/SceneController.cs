using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadMainMenu()
    {
        //Loads the Main Menu upon being the button being pressed in a menu screen
        SceneManager.LoadScene("StartMenu");
        PlayerPrefs.DeleteAll(); //Jevon's Code - Used to reset the player's stats after completing the game and hitting the main menu
        PlayerStats.Instance.StartingLevel(); //Jevon's Code - Used to reset the player's level back to 1 and apply their basic stats accordingly
        PlayerStats.Instance.UpdateLevel();
        EnemyLevelTracker.Instance.enteredDeepForestLevel = false;
    }
}
