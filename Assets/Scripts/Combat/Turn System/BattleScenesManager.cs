using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleScenesManager : MonoBehaviour
{
    public Scene previousScene;
    public string previousSceneName;
    public int previousSceneNumber;
    public int levelIndex;

    public static BattleScenesManager Instance;

    private void OnEnable()
    {
        Debug.Log("Using OnEnabled");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Update()
    {
        CheckPlayerLocation();
        CheckLevelIndex();
    }

    private void Awake()
    {
        Debug.Log("Active Scene is " + SceneManager.GetActiveScene().name + ". ");
        //previousSceneNumber = SceneManager.GetActiveScene();
        //previousSceneName = previousSceneNumber.name;
        //previousScene = previousSceneNumber.buildIndex;
        //PlayerPrefs.SetString("Previous Level", previousSceneName);
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

    public void CheckPlayerLocation()
    {
        //Checks to see if the player is in the Forest or Deep Forest areas
        //When the player defeats the enemies in those areas, it will load them into the specific area
        previousScene = SceneManager.GetActiveScene();
        previousSceneName = previousScene.name;
        previousSceneNumber = previousScene.buildIndex;
        PlayerPrefs.SetString("Previous Level", previousSceneName);
        if (previousSceneName == "Forest")
        {
            Debug.Log("Player has entered the forest, retaining forest information");
            levelIndex = 1;
        }
        else if (previousSceneName == "DeepForest")
        {
            Debug.Log("Player has entered the deep forest, retaining deep forest information");
            levelIndex = 2;
        }
        else if (previousSceneName == "Village")
        {
            Debug.Log("Player has entered the deep forest, retaining deep forest information");
            levelIndex = 3;
        }
        else if (previousSceneName == "CastleTown")
        {
            Debug.Log("Player has entered the deep forest, retaining deep forest information");
            levelIndex = 4;
        }
        else
        {
            Debug.Log("Player is not in the forest or deep forest level, no need to retain any forest information");
            
            //previousSceneName = previousScene.name;
        }
    }

    public void CheckLevelIndex()
    {
        if (SceneManager.GetActiveScene().name == "SingleWolfCombat")
        {
            if (levelIndex == 1)
            {
                Debug.Log("Player is facing a wolf in the Forest Level!");
                previousSceneName = "Forest";
            }
            else if(levelIndex == 2)
            {
                Debug.Log("Player is facing a wolf in the Deep Forest Level!");
                previousSceneName = "DeepForest";
            }
        }
        if (SceneManager.GetActiveScene().name == "SingleHawkCombat")
        {
            if (levelIndex == 1)
            {
                Debug.Log("Player is facing a wolf in the Forest Level!");
                previousSceneName = "Forest";
            }
            else if (levelIndex == 2)
            {
                Debug.Log("Player is facing a wolf in the Deep Forest Level!");
                previousSceneName = "DeepForest";
            }
        }

        if (SceneManager.GetActiveScene().name == "MultipleWolfCombat")
        {
            if (levelIndex == 1)
            {
                Debug.Log("Player is facing a wolf in the Forest Level!");
                previousSceneName = "Forest";
            }
            else if (levelIndex == 2)
            {
                Debug.Log("Player is facing a wolf in the Deep Forest Level!");
                previousSceneName = "DeepForest";
            }
        }
        if (SceneManager.GetActiveScene().name == "MultipleHawkCombat")
        {
            if (levelIndex == 1)
            {
                Debug.Log("Player is facing a wolf in the Forest Level!");
                previousSceneName = "Forest";
            }
            else if (levelIndex == 2)
            {
                Debug.Log("Player is facing a wolf in the Deep Forest Level!");
                previousSceneName = "DeepForest";
            }
        }
        if (SceneManager.GetActiveScene().name == "WolfHawkCombat")
        {
            if (levelIndex == 1)
            {
                Debug.Log("Player is facing a wolf in the Forest Level!");
                previousSceneName = "Forest";
            }
            else if (levelIndex == 2)
            {
                Debug.Log("Player is facing a wolf in the Deep Forest Level!");
                previousSceneName = "DeepForest";
            }
        }
        if (SceneManager.GetActiveScene().name == "MultipleVillagerCombat")
        {
            if (levelIndex == 4)
            {
                Debug.Log("Player is facing Villagers in the Castle Town Level!");
                previousSceneName = "CastleTown";
            }
        }
        if (SceneManager.GetActiveScene().name == "SingleVillagerCombat")
        {
            if (levelIndex == 3)
            {
                Debug.Log("Player is facing the Villager Mini-Boss in the Village Level!");
                previousSceneName = "Village";
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("On Scene Loaded" + scene.name);
        Debug.Log("Load Mode " + mode);
    }

    public void Start()
    {
        
    }

    public void OnDisable()
    {
        Debug.Log("OnDisable activated...");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("TutorialCave");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
