using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadBattle : MonoBehaviour
{
    public int lastBattle;
    //public Scene previousSceneNumber;
    //public int previousScene;

    public static ReloadBattle Instance;

    private void OnEnable()
    {
        Debug.Log("Using OnEnabled");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Awake()
    {
        Debug.Log("Active Scene is " + SceneManager.GetActiveScene().name + ". ");
        //previousSceneNumber = SceneManager.GetActiveScene();
        //previousScene = previousSceneNumber.buildIndex;
        //PlayerPrefs.SetString("Previous Level", previousSceneNumber.name);
        if (Instance != null && Instance != this)
        {
            //If more than one instance of the PlayerStats Game Object exist in a scene
            //Then that object will be destroyed and the current PlayerStats Game Object will remain in the scene
            Debug.Log("Additional BattleScenesManager Object found, destroying any remaining PlayerStats Objects...");
            Destroy(gameObject);
            return;
        }
        else
        {
            //If only one instance of the PlayerStats Game Object exist within the game, then 
            //That instance of the PlayerStats Game Object remains throughout the game
            Debug.Log("A single BattleSceneManager Object exist in the game...");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("On Scene Loaded" + scene.name);
        Debug.Log("Load Mode " + mode);
    }

    public void OnDisable()
    {
        Debug.Log("OnDisable activated...");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadPreviousBattle()
    {
        lastBattle = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("Lose Screen");
        
    }

    public void RetryBattle()
    {
        SceneManager.LoadScene(lastBattle);
    }
}
