using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestExperience : MonoBehaviour
{
    //Variables for the amount of Experience points needed for the player to level up to the next level
    public float currentExperiencePoints;
    public float experiencePointsGained;
    public float maxExperiencePoints;
    public Image experienceBar;
    public TextMeshProUGUI experienceText;
    public float enemyExp;

    public int currentLevel, totalExperience;
    public int previousLevelExperience, nextLevelsExperience;

    public TextMeshProUGUI levelText;
    
    //TurnSystem script reference to be called in conjunction with the Experience System
    TurnSystem turnSystem;

    //PlayerStats script reference to be called in conjunction with the Experience System
    //to level up the player and increase their stats upon level up
    PlayerStats playerStats;

    //EnemyStats script reference to be called in conjunction with the Experience System
    //to grant the player a specific amount of experience points upon being defeated
    EnemyBattleStats enemyBattleStats;


    //Variable for the Animation Curve that contains the Experience scaling for the player to level up
    [SerializeField] AnimationCurve experienceCurve;

    public void Awake()
    {
        //Finds the PlayerStats and TurnSystem scripts within the scene
        //playerStats = FindObjectOfType<PlayerStats>();
        //turnSystem = FindObjectOfType<TurnSystem>();
        //enemyBattleStats = FindObjectOfType<EnemyBattleStats>();

    }

    public void Start()
    {
        UpdateLevel();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Pressed the left click, gaining Debug Experience!");
            AddExperiencePoints(5);
        }
    }

    public void AddExperiencePoints(int amount)
    {
        totalExperience += amount;
        CheckForLevelUp();
        UpdateInterface();
    }

   

    public void ReceiveExperiencePoints()
    {
        //Contains the experience points gained after each battle
        //Experience points gained varies based on the strength of the enemy fought, how many enemies were fought
        //And the player's current level 
        Debug.Log("Player won the battle, the player will now gain experience points!");
        experiencePointsGained += enemyBattleStats.enemyExpPoints;
        currentExperiencePoints = experiencePointsGained;
        experienceBar.fillAmount += experiencePointsGained / 100 * 4;
        experienceText.text = currentExperiencePoints + " / " + maxExperiencePoints;
        if (experiencePointsGained >= maxExperiencePoints && playerStats.playerCurrentLevel != playerStats.playerNextLevel)
        {
            Debug.Log("The player has reached the requirements to go to the next level, the player will now level up!");
            experienceBar.fillAmount = 1;
        }
    }

    public void CheckForLevelUp()
    {
        Debug.Log("HAS THE PLAYER REACHED THE NEXT LEVEL?");
        if (totalExperience >= nextLevelsExperience)
        {
            currentLevel++;
            UpdateLevel();
        }
    }

    public void UpdateLevel()
    {
        previousLevelExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
        UpdateInterface();
    }

    public void UpdateInterface()
    {
        int startPoint = totalExperience - previousLevelExperience;
        int endPoint = nextLevelsExperience - previousLevelExperience;

        levelText.text = currentLevel.ToString();
        experienceText.text = startPoint + " exp / " + endPoint + " exp";
        experienceBar.fillAmount = (float)startPoint / (float)endPoint;
    }
}
