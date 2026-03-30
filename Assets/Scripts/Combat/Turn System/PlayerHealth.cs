using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{

    //Player Health and additional variables
    public Image playerHealthBar;
    public TextMeshProUGUI playerHealthText;
    public float playerCurrentHealth;
    public float playerMaxHealth;

    //Additional variables for improved health bar visual effects
    //Variables include a "back" health bar hidden behind the initial green "player health bar
    public Image backHealthBar;
    public float chipSpeed = 2f;
    private float lerpTimer;

    //Variable for the Player Stats script that will be referenced
    PlayerStats playerStats;

    //variable for the DamageContainer script that will work in conjunction with the player's health
    DamageContainer damageContainer;

    private void Awake()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        damageContainer = FindObjectOfType<DamageContainer>();
    }

    private void Start()
    {
        //playerCurrentHealth = playerStats.maxHealthStat;
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerHealth();
        UpdatePlayerHealthUI();
    }

    public void CheckPlayerHealth()
    {
        //Checks the player's current health during the game and matches it with the player's stats
        //Checks for the player's max health during the game and matches it with the player's stats
        //Displays the text of the player's current health during battle
        //playerStats.maxHealthStat = playerMaxHealth;
        Debug.Log("Analyzing and acquiring player's health stat and value...");
        playerCurrentHealth = playerStats.healthStat;
        playerHealthText.text = playerCurrentHealth + " HP " +  " / " + playerStats.maxHealthStat;
    }

    public void UpdatePlayerHealthUI()
    {
        //Keeps tracks of the Player's Health UI and updates it accordingly
        Debug.Log(" Player's health is " + playerCurrentHealth);
        float playerFrontHealthBarFill = playerHealthBar.fillAmount;
        float playerBackHealthBarFill = backHealthBar.fillAmount;
        float healthFraction = playerStats.healthStat / playerStats.maxHealthStat;
        if (playerBackHealthBarFill >= healthFraction)
        {
            Debug.Log("The player likely took damage, analyzing player health UI!");
            playerHealthBar.fillAmount = healthFraction;
            backHealthBar.color = Color.black;
            lerpTimer += Time.deltaTime;
            float percentageComplete = lerpTimer / chipSpeed;
            percentageComplete = percentageComplete * percentageComplete;
            backHealthBar.fillAmount = Mathf.Lerp(playerBackHealthBarFill, healthFraction, percentageComplete);
        }
    }

    public void TakePhysicalDamage()
    {
        //When the player takes damage, the health bars for the player will decrease accordingly
        //The player's "original" health will decrease and the back health bar will try to catch
        //Up to the player's "original" health bar's fill amount to improve visual clarity for the player
        //Upon taking damage from the enemies' attacks
        playerHealthBar.fillAmount -= damageContainer.enemyPhysicalAttackDamage / 100;
        playerStats.healthStat -= damageContainer.enemyPhysicalAttackDamage;
        lerpTimer = 0.0f;
    }

    public void TakeMagicalDamage()
    {
        //When the player takes damage, the health bars for the player will decrease accordingly
        //The player's "original" health will decrease and the back health bar will try to catch
        //Up to the player's "original" health bar's fill amount to improve visual clarity for the player
        //Upon taking damage from the enemies' attacks
        playerHealthBar.fillAmount -= damageContainer.enemyMagicalAttackDamage / 100;
        playerStats.healthStat -= damageContainer.enemyMagicalAttackDamage;
        lerpTimer = 0.0f;
    }

    public void IncreasePlayerHealth()
    {
        //When the player recovers their health, their back health bar will try to reach the front 
        //And the back health bar changes to a green color
        playerHealthBar.fillAmount += damageContainer.playerMagicalHealthRecovery / 100;
        playerStats.healthStat += damageContainer.playerMagicalHealthRecovery;
        lerpTimer = 0.0f;
    }
}
