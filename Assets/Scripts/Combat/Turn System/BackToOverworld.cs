using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToOverworld : MonoBehaviour
{
    public int sceneNumber;
    private static int previousScene;
    private int oldPreviousScene;

    ReloadBattle reloadBattle;
    PlayerStats playerStats;
    

    private void Awake()
    {
        reloadBattle = FindObjectOfType<ReloadBattle>();
        playerStats = FindObjectOfType<PlayerStats>();

    }
    // Start is called before the first frame update
    void Start()
    {
        oldPreviousScene = reloadBattle.lastBattle;
        previousScene = oldPreviousScene;
    }

    public void LoadLastBattle()
    {
        SceneManager.LoadScene(previousScene);
        PlayerPrefs.DeleteAll();
        playerStats.CheckPlayerLevel();
    }
}
