using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public enum GameStates { Start, PlayerTurn, EnemyTurn }
public class TurnSystem : MonoBehaviour
{

    //Variable to contain the Enum which will be used to verify the player's turn
    public GameStates gameState;

    //variable for the GameObjects called to the scene upon entering battle from the Overworld
    public GameObject playerSprite;
    public GameObject enemySprites;
    public GameObject[] battleEnemies;

    //Variable  to activate so the player can see who's turn is it when playing
    public GameObject playerTurnSignal;
    //public GameObject enemyTurnSignal;

    //Variables for the buttons that the player
    //will press when it is their turn and they can press a button
    public Button[] combatButtons;              //Used to allow the player to click options during battle such as Attack, Magic, Item, and Defend 
    public Button vengeanceButton;              //Allows the player to use their Vengeance Attacks, powerful attack options that cost the Vengeance Resource which is gained by attacking and being attacked
    public Button[] enemyButton;                //When the player wants to attack an enemy, they will click on the enemy designated with the specific button
    public List<Button> enemyButtons = new List<Button>();
    public Button[] progressionButtons;         //Used in conjunction with the player's level to gradually increase their power with the player only having a few attacks based on their level

    //Variables containing all of the buttons that the player will press when selecting Attack, Magic, or Items
    public GameObject attackOptions;
    public GameObject magicOptions;
    //public GameObject itemOptions;

    //Variable for the panel that contains the buttons the player will use during battle
    public GameObject[] combatPanels;

    //Variables for the Vengeance Mode Options available to the player when they activate their Vengeance Mode during the game
    public GameObject vengeanceOptions;

    //Variable for the return button allowing the player to return back to the selection options 
    public GameObject returnButton;

    //Variables for the player's health and mana
    PlayerHealth playerHealth;
    PlayerMana playerMana;

    //Variable for the player's Vengeance pool allowing him to use enhanced abilities and his Giant Form
    PlayerVengeanceMeter playerVengeanceMeter;

    //Variable for the damage calculation scripts that contains the damage of the player's and enemies' attacks during battle.
    DamageContainer damageContainer;

    //Variable for the container of the current battle's index to be identified if the player dies
    BattleScenesManager battleScenesManager;

    //Variable for the enemy selection scripts that allows the player to choose an enemy to hit with an attack with
    SelectEnemies selectEnemies;

    //Variable for the player's attacks that calculate their damage and allow the player's turn to end
    AttackContainer attackContainer;

    //Variable for the information of the player's attacks so the player can know what each available attack does when performed
    OptionInfo optionInfo;

    //Variable for the chosen attack by the player that signifies what attack will be performed
    public int attackPicked;

    //Variable containing the stat system during combat
    StatSystem statSystem;

    //Variable containing the Experience System script to grant the player experience
    ExperienceSystem experienceSystem;

    //Variable containing the PlayerStats script to assess the player's stats during battle
    PlayerStats playerStats;

    //Variable to contain the scenes within the game during battles to return to the previous scene after a battle
    ReloadBattle reloadBattle;

    //Variable containing the total amount of enemies exist within the battle
    CountEnemies countEnemies;

    //Variable to determine if the first enemy has completed their turn
    public bool firstEnemyActive;
    public bool firstEnemyTurnCompleted;
    public bool secondEnemyActive;
    public bool secondEnemyTurnCompleted;
    public bool thirdEnemyActive;
    public bool thirdEnemyTurnCompleted;

    //Variable for the turn in which either a player or enemy is performing their attack
    //When the turn counter reaches a specific number, either the player or enemy will make their move
    public int turnCounter;

    public int enemyAttackValue;

    private void Awake()
    {
        //GameObject.FindGameObjectsWithTag("Enemy");
        damageContainer = FindObjectOfType<DamageContainer>();
        attackContainer = FindObjectOfType<AttackContainer>();
        experienceSystem = FindObjectOfType<ExperienceSystem>();
        //enemyHealth = FindObjectOfType<EnemyHealth>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMana = FindObjectOfType<PlayerMana>();
        //enemyMana = FindObjectOfType<EnemyMana>();
        playerVengeanceMeter = FindObjectOfType<PlayerVengeanceMeter>();
        battleScenesManager = FindObjectOfType<BattleScenesManager>();
        selectEnemies = FindObjectOfType<SelectEnemies>();
        optionInfo = FindObjectOfType<OptionInfo>();
        statSystem = FindObjectOfType<StatSystem>();
        playerStats = FindObjectOfType<PlayerStats>();
        reloadBattle = FindObjectOfType<ReloadBattle>();
        countEnemies = FindObjectOfType<CountEnemies>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //At the start of the battle, the initial game state is the starting state
        //The game then checks how much enemies are in the scene by finding all GameObjects tagged as "Enemy"
        //The battle will then begin, with the chosen attack being null and the player's Vengeance Meter being at 0
        gameState = GameStates.Start;
        countEnemies.CheckForEnemyAmount();
        CheckPlayerProgression();
        StartCoroutine(BeginBattle());
        playerVengeanceMeter.hasVengeance = false;
        attackPicked = 0;
        turnCounter = 0;
    }

   

    private IEnumerator BeginBattle()
    {
        //Begins the player's turn and activates the different panels containing the players' attacks
        //Sets the player's battle information to be the specific values according to their current stats and level
        //After 2 seconds passes, the game determines who goes first based on the player and the enemies' speed stats
        //If the player wins the speed clash, then it is their turn and they will be able to perform their attack.
        combatPanels[0].SetActive(false);
        combatPanels[1].SetActive(false);
        combatPanels[2].SetActive(false);
        playerTurnSignal.SetActive(false);
        //enemyTurnSignal.SetActive(false);
        //playerHealth.playerHealthText.text = playerStats.healthStat + " hp " + " / " + playerStats.maxHealthStat;
        //playerMana.playerManaText.text = playerStats.manaStat + " / " + playerStats.maxManaStat;
        playerHealth.playerCurrentHealth = playerStats.healthStat;
        playerHealth.playerHealthBar.fillAmount = playerStats.healthStat / 100;
        playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance;
        experienceSystem.experienceText.text = playerStats.playerCurrentExp + " exp / " + playerStats.playerNextLevelExp + " exp";
        experienceSystem.levelText.text = " Lv " + playerStats.playerCurrentLevel.ToString();
        experienceSystem.experienceBar.fillAmount = (float)playerStats.playerCurrentExp / (float)playerStats.playerNextLevelExp;
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("One Enemy detected, creating one enemy...");
            battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            countEnemies.enemyCombatSprites[0] = battleEnemies[0];
            //battleEnemies[0] = countEnemies.enemyCombatSprites[0];
            if (battleEnemies[0].name == "TutorialWolf" || battleEnemies[0].name == "EnemyWolf")
            {
                //battleEnemies[0].GetComponent<WolfEnemy>();
                Debug.Log("Getting Wolf Enemy information...");
                battleEnemies[0].GetComponent<WolfEnemy>().wolfHealthText.text = battleEnemies[0].GetComponent<WolfEnemy>().enemyHealthStat + " / " + battleEnemies[0].GetComponent<WolfEnemy>().enemyMaxHealthStat;
                battleEnemies[0].GetComponent<WolfEnemy>().wolfManaText.text = battleEnemies[0].GetComponent<WolfEnemy>().enemyManaStat + " / " + battleEnemies[0].GetComponent<WolfEnemy>().enemyMaxManaStat;
                battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
            else if (battleEnemies[0].name == "EnemyHawk")
            {
                //battleEnemies[0].GetComponent<WolfEnemy>();
                Debug.Log("Getting Hawk Enemy information...");
                battleEnemies[0].GetComponent<HawkEnemy>().hawkHealthText.text = battleEnemies[0].GetComponent<HawkEnemy>().enemyHealthStat + " / " + battleEnemies[0].GetComponent<HawkEnemy>().enemyMaxHealthStat;
                battleEnemies[0].GetComponent<HawkEnemy>().hawkManaText.text = battleEnemies[0].GetComponent<HawkEnemy>().enemyManaStat + " / " + battleEnemies[0].GetComponent<HawkEnemy>().enemyMaxManaStat;
                battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
            else if (battleEnemies[0].name == "EnemyVillager")
            {
                //battleEnemies[0].GetComponent<WolfEnemy>();
                Debug.Log("Getting Villager Boss Enemy information...");
                battleEnemies[0].GetComponent<VillagerEnemy>().villagerHealthText.text = battleEnemies[0].GetComponent<VillagerEnemy>().enemyHealthStat + " / " + battleEnemies[0].GetComponent<VillagerEnemy>().enemyMaxHealthStat;
                battleEnemies[0].GetComponent<VillagerEnemy>().villagerManaText.text = battleEnemies[0].GetComponent<VillagerEnemy>().enemyManaStat + " / " + battleEnemies[0].GetComponent<VillagerEnemy>().enemyMaxManaStat;
                battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
            else
            {
                Debug.Log("No Enemy Detected...");
            }
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Locating all two of the enemies in the scene...");
            battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            battleEnemies[0] = countEnemies.enemyCombatSprites[0];
            battleEnemies[1] = countEnemies.enemyCombatSprites[1];
            countEnemies.enemyCombatSprites.Reverse();
            if (battleEnemies[0].name == "EnemyWolf 2")
            {
                //battleEnemies[0].GetComponent<WolfEnemy>();
                Debug.Log("Getting Wolf Enemy information...");
                battleEnemies[0].GetComponent<WolfEnemy>().wolfHealthText.text = battleEnemies[0].GetComponent<WolfEnemy>().enemyHealthStat + " / " + battleEnemies[0].GetComponent<WolfEnemy>().enemyMaxHealthStat;
                battleEnemies[0].GetComponent<WolfEnemy>().wolfManaText.text = battleEnemies[0].GetComponent<WolfEnemy>().enemyManaStat + " / " + battleEnemies[0].GetComponent<WolfEnemy>().enemyMaxManaStat;
                battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
            else if (battleEnemies[0].name == "EnemyHawk 2" || battleEnemies[0].name == "EnemyHawk")
            {
                //battleEnemies[0].GetComponent<WolfEnemy>();
                Debug.Log("Getting Hawk Enemy information...");
                battleEnemies[0].GetComponent<HawkEnemy>().hawkHealthText.text = battleEnemies[0].GetComponent<HawkEnemy>().enemyHealthStat + " / " + battleEnemies[0].GetComponent<HawkEnemy>().enemyMaxHealthStat;
                battleEnemies[0].GetComponent<HawkEnemy>().hawkManaText.text = battleEnemies[0].GetComponent<HawkEnemy>().enemyManaStat + " / " + battleEnemies[0].GetComponent<HawkEnemy>().enemyMaxManaStat;
                battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
            if (battleEnemies[1].name == "EnemyWolf" || battleEnemies[1].name == "EnemyWolf 2")
            {
                //battleEnemies[0].GetComponent<WolfEnemy>();
                Debug.Log("Getting Wolf Enemy information...");
                battleEnemies[1].GetComponent<WolfEnemy>().wolfHealthText.text = battleEnemies[1].GetComponent<WolfEnemy>().enemyHealthStat + " / " + battleEnemies[1].GetComponent<WolfEnemy>().enemyMaxHealthStat;
                battleEnemies[1].GetComponent<WolfEnemy>().wolfManaText.text = battleEnemies[1].GetComponent<WolfEnemy>().enemyManaStat + " / " + battleEnemies[1].GetComponent<WolfEnemy>().enemyMaxManaStat;
                battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
            else if (battleEnemies[1].name == "EnemyHawk")
            {
                //battleEnemies[0].GetComponent<WolfEnemy>();
                Debug.Log("Getting Hawk Enemy information...");
                battleEnemies[1].GetComponent<HawkEnemy>().hawkHealthText.text = battleEnemies[1].GetComponent<HawkEnemy>().enemyHealthStat + " / " + battleEnemies[1].GetComponent<HawkEnemy>().enemyMaxHealthStat;
                battleEnemies[1].GetComponent<HawkEnemy>().hawkManaText.text = battleEnemies[1].GetComponent<HawkEnemy>().enemyManaStat + " / " + battleEnemies[1].GetComponent<HawkEnemy>().enemyMaxManaStat;
                battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
            if (battleEnemies[0].name == "EnemyVillager 2")
            {
                //battleEnemies[0].GetComponent<WolfEnemy>();
                Debug.Log("Getting Villager Boss Enemy information...");
                battleEnemies[0].GetComponent<VillagerEnemy>().villagerHealthText.text = battleEnemies[0].GetComponent<VillagerEnemy>().enemyHealthStat + " / " + battleEnemies[0].GetComponent<VillagerEnemy>().enemyMaxHealthStat;
                battleEnemies[0].GetComponent<VillagerEnemy>().villagerManaText.text = battleEnemies[0].GetComponent<VillagerEnemy>().enemyManaStat + " / " + battleEnemies[0].GetComponent<VillagerEnemy>().enemyMaxManaStat;
                battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
            if (battleEnemies[1].name == "EnemyVillager")
            {
                //battleEnemies[0].GetComponent<WolfEnemy>();
                Debug.Log("Getting Villager Boss Enemy information...");
                battleEnemies[1].GetComponent<VillagerEnemy>().villagerHealthText.text = battleEnemies[1].GetComponent<VillagerEnemy>().enemyHealthStat + " / " + battleEnemies[0].GetComponent<VillagerEnemy>().enemyMaxHealthStat;
                battleEnemies[1].GetComponent<VillagerEnemy>().villagerManaText.text = battleEnemies[1].GetComponent<VillagerEnemy>().enemyManaStat + " / " + battleEnemies[0].GetComponent<VillagerEnemy>().enemyMaxManaStat;
                battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
        }
        if (countEnemies.enemyAmount == 3)
        {
            Debug.Log("Locating all three of the enemies in the scene...");
            battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            battleEnemies[0] = countEnemies.enemyCombatSprites[0];
            battleEnemies[1] = countEnemies.enemyCombatSprites[1];
            battleEnemies[2] = countEnemies.enemyCombatSprites[2];
            countEnemies.enemyCombatSprites.Reverse();
            if (countEnemies.enemyCombatSprites[0].name == "Fist Hero")
            {
                Debug.Log("Getting Fist Hero Enemy information...");
                countEnemies.enemyCombatSprites[0].GetComponent<FistHeroEnemy>().fistHeroHealthText.text = countEnemies.enemyCombatSprites[0].GetComponent<FistHeroEnemy>().enemyHealthStat + " / " + countEnemies.enemyCombatSprites[0].GetComponent<FistHeroEnemy>().enemyMaxHealthStat;
                countEnemies.enemyCombatSprites[0].GetComponent<FistHeroEnemy>().fistHeroManaText.text = countEnemies.enemyCombatSprites[0].GetComponent<FistHeroEnemy>().enemyManaStat + " / " + countEnemies.enemyCombatSprites[0].GetComponent<FistHeroEnemy>().enemyMaxManaStat;
                countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
            if (countEnemies.enemyCombatSprites[1].name == "Mage Hero")
            {
                Debug.Log("Getting Spear Hero Enemy information...");
                countEnemies.enemyCombatSprites[1].GetComponent<MageHeroEnemy>().mageHeroHealthText.text = countEnemies.enemyCombatSprites[1].GetComponent<MageHeroEnemy>().enemyHealthStat + " / " + countEnemies.enemyCombatSprites[1].GetComponent<MageHeroEnemy>().enemyMaxHealthStat;
                countEnemies.enemyCombatSprites[1].GetComponent<MageHeroEnemy>().mageHeroManaText.text = countEnemies.enemyCombatSprites[1].GetComponent<MageHeroEnemy>().enemyManaStat + " / " + countEnemies.enemyCombatSprites[1].GetComponent<MageHeroEnemy>().enemyMaxManaStat;
                countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
            if (countEnemies.enemyCombatSprites[2].name == "Spear Hero")
            {
                Debug.Log("Getting Mage Hero Enemy information...");
                countEnemies.enemyCombatSprites[2].GetComponent<SpearHeroEnemy>().spearHeroHealthText.text = countEnemies.enemyCombatSprites[2].GetComponent<SpearHeroEnemy>().enemyHealthStat + " / " + countEnemies.enemyCombatSprites[2].GetComponent<SpearHeroEnemy>().enemyMaxHealthStat;
                countEnemies.enemyCombatSprites[2].GetComponent<SpearHeroEnemy>().spearHeroManaText.text = countEnemies.enemyCombatSprites[2].GetComponent<SpearHeroEnemy>().enemyManaStat + " / " + countEnemies.enemyCombatSprites[2].GetComponent<SpearHeroEnemy>().enemyMaxManaStat;
                countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
        }
        yield return new WaitForSeconds(2f);
        statSystem.DetermineSpeed();
        if (gameState == GameStates.PlayerTurn)
        {
            Debug.Log("Beginning Player's Turn...");
            turnCounter = 1;
            PlayerTurn();
        }
    }

    public void CheckPlayerProgression()
    {
        //Determines which attacks the player will have access to based on their current level
        //The player will get access to Fire Breath after defeating the Villager mini boss
        //The player will get access to Ice Breath after getting passed the Deep Forest Level
        //The player will only have access to Claw Strike, Tail Swipe, Healing Breath, and Terror Breath for a few levels
        if (playerStats.playerCurrentLevel <= 2)
        {
            progressionButtons[0].gameObject.SetActive(false);
            if (playerStats.playerCurrentLevel >= 2)
            {
                progressionButtons[0].gameObject.SetActive(true);
            }
        }
        if (playerStats.playerCurrentLevel <= 4)
        {
            progressionButtons[1].gameObject.SetActive(false);
            progressionButtons[2].gameObject.SetActive(false);
            if (playerStats.playerCurrentLevel >= 4)
            {
                progressionButtons[1].gameObject.SetActive(true);
                progressionButtons[2].gameObject.SetActive(true);
            }
        }
    }

    public void PlayerTurn()
    {
        //On the player's turn, the player's options will be activated, allowing them to choose an option during battle
        //The player's health and mana amount will be displayed, letting the player know how much health and mana they have remaining
        //During the player's turn, the player will always regenerate 10 points of mana if their mana runs out
        //In addition, depending on how much of the Vengeance Meter is maintained during battle, the player will have access to their Vengeance Move options
        //When reaching the designated Vengeance Meter Threshold
        if (gameState == GameStates.PlayerTurn)
        {
            combatPanels[0].SetActive(true);
            combatPanels[1].SetActive(true);
            combatPanels[2].SetActive(true);
            attackOptions.SetActive(false);
            magicOptions.SetActive(false);
            if (countEnemies.enemyAmount >= 1)
            {
                Debug.Log("Only one enemy in battle, turning their button off...");
                //enemyButton[0].gameObject.SetActive(false);
                enemyButtons[0].gameObject.SetActive(false);
                
            }
            if (countEnemies.enemyAmount >= 2)
            {
                Debug.Log("Two enemies in battle, turning their button off...");
                //enemyButton[0].gameObject.SetActive(false);
                //enemyButton[1].gameObject.SetActive(false);
                enemyButtons[0].gameObject.SetActive(false);
                enemyButtons[1].gameObject.SetActive(false);
            }
            if (countEnemies.enemyAmount == 3)
            {
                Debug.Log("Three enemies in battle, turning their button off...");
                //enemyButton[0].gameObject.SetActive(false);
                //enemyButton[1].gameObject.SetActive(false);
                //enemyButton[2].gameObject.SetActive(false);
                enemyButtons[0].gameObject.SetActive(false);
                enemyButtons[1].gameObject.SetActive(false);
                enemyButtons[2].gameObject.SetActive(false);
            }
            ShowCharacterCircles();
            //playerHealth.playerCurrentHealth = playerStats.healthStat;
            //playerHealth.playerHealthBar.fillAmount = playerStats.healthStat / 100;
            playerMana.playerManaBar.fillAmount = playerStats.manaStat / 100;
            experienceSystem.experienceBar.fillAmount = (float)playerStats.playerCurrentExp / (float)playerStats.playerNextLevelExp;
        }
        if (playerMana.playerCurrentMana >= playerMana.playerMaxMana)
        {
            playerMana.playerCurrentMana = playerMana.playerMaxMana;
            Debug.Log("Mana Pool full!");
        }
        else
        {
            Debug.Log("Regenerating a bit of Mana!");
            playerStats.manaStat += 10.0f;
            if (playerMana.playerCurrentMana >= playerMana.playerMaxMana)
            {
                Debug.Log("Regenerated enough mana to be full, the Mana Pool is now full once again!");
                playerMana.playerCurrentMana = playerMana.playerMaxMana;
            }
            playerMana.playerManaBar.fillAmount = playerStats.manaStat / 100;
            playerMana.playerManaText.text = playerMana.playerCurrentMana + " MP" + " / " + playerStats.maxManaStat;
        }
        if (playerStats.healthStat <= 0)
        {
            Debug.Log("Player at 0 HP, Player has lost the battle!");
            DisablePlayerButtons();
            PlayerDeath();
        }
        if (playerVengeanceMeter.hasVengeance == false)
        {
            Debug.Log("Vengeance Meter not at full yet...");
            vengeanceButton.gameObject.SetActive(false);
            vengeanceOptions.SetActive(false);
        }
        if (playerVengeanceMeter.playerCurrentVengeance >= 50.0f)
        {
            Debug.Log("Player can use Vengeance Attacks!");
            playerVengeanceMeter.hasVengeance = true;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                Debug.Log("Player has reached max Vengeance!");
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
            }
        }
        if (damageContainer.isDefending == false)
        {
            Debug.Log("Player has not performed any defensive actions this turn, enemy damage will remain the same or reset accordingly");
            damageContainer.PlayerDefense();
            
        }
        
    }

    public void AttackButton()
    {
        //If the player clicks the Attack Button during battle, the player' attack options will be available
        //If the player wants to pick a different move, the return button will also be available for them to choose a different option
        if (gameState == GameStates.PlayerTurn)
        {
            attackOptions.SetActive(true);
            magicOptions.SetActive(false);
            returnButton.SetActive(true);
            if (playerVengeanceMeter.hasVengeance == true)
            {
                vengeanceOptions.SetActive(false);
            }
        }
        if (playerVengeanceMeter.hasVengeance == true)
        {
            Debug.Log("Vengeance Meter full!");
            vengeanceButton.gameObject.SetActive(true);
        }

    }

    public void ClawAttackSelection()
    {
        //Before performing the Claw Attack, the player will need to choose which enemy to hit by clicking a button
        attackContainer.isPerformingPhysicalAttack = true;
        optionInfo.attackInfoText.SetActive(false);
        attackPicked = 1;
        if (attackContainer.isPerformingPhysicalAttack && countEnemies.enemyAmount >= 2)
        {
            Debug.Log("More than one enemy in battle, choosing a target for this single target attack!");
            ChooseTarget();
        }
        else if (attackContainer.isPerformingPhysicalAttack && countEnemies.enemyAmount == 1)
        {
            Debug.Log("Only one enemy in battle, attacking enemy!");
            attackContainer.enemyAttacked = 1;
            StartCoroutine(attackContainer.PerformClawAttack());
        }
        
    }

    public void TailAttackSelection()
    {
        //Before performing the Tail Attack, the player will need to choose which enemy to hit by clicking a button
        attackContainer.isPerformingPhysicalAttack = true;
        optionInfo.attackInfoText.SetActive(false);
        attackPicked = 2;
        if (attackContainer.isPerformingPhysicalAttack)
        {
            StartCoroutine(attackContainer.PerformTailAttack());
            //ChooseTarget();
        }

    }

    public void MagicButton()
    {
        if (gameState == GameStates.PlayerTurn)
        {
            attackOptions.SetActive(false);
            magicOptions.SetActive(true);
            returnButton.SetActive(true);
            if (playerVengeanceMeter.hasVengeance == true)
            {
                vengeanceOptions.SetActive(false);
            }
        }
        if (playerVengeanceMeter.hasVengeance == true)
        {
            Debug.Log("Vengeance Meter full!");
            vengeanceButton.gameObject.SetActive(true);
        }

    }

    public void FireBreathAttackSelection()
    {
        //Before performing the Fire Breath Attack, the player will need to choose which enemy to hit by clicking a button and assess if they have enough mana available to perform the attack
        attackContainer.isPerformingMagicalAttack = true;
        optionInfo.attackInfoText.SetActive(false);
        attackPicked = 3;
        if (playerMana.hasMana == true)
        {
            playerMana.CheckManaPool();
            if (countEnemies.enemyAmount >= 2)
            {
                Debug.Log("More than one enemy in battle, choosing a target for this single target attack!");
                ChooseTarget();
            }
            else if (countEnemies.enemyAmount == 1)
            {
                Debug.Log("Only one enemy in battle, attacking enemy!");
                attackContainer.enemyAttacked = 1;
                StartCoroutine(attackContainer.PerformFireBreath());
            }
        }
        else
        {
            Debug.Log("Does not have mana, cannot use Mana Attack");
            playerMana.CheckManaPool();
            playerMana.hasMana = false;
            return;
        }

    }

    public void IceBreathAttackSelection()
    {
        //Before performing the Ice Breath Attack, the player will need to choose which enemy to hit by clicking a button and assess if they have enough mana available to perform the attack
        attackContainer.isPerformingMagicalAttack = true;
        optionInfo.attackInfoText.SetActive(false);
        attackPicked = 4;
        if (playerMana.hasMana == true)
        {
            playerMana.CheckManaPool();
            StartCoroutine(attackContainer.PerformIceBreath());
            //ChooseTarget();
        }
        else
        {
            Debug.Log("Does not have mana, cannot use Mana Attack");
            playerMana.CheckManaPool();
            playerMana.hasMana = false;
            return;
        }
    }

    public void TerrorBreathAttackSelection()
    {
        //Before performing the Terror Breath Attack, the player will need to choose which enemy to hit by clicking a button and assess if they have enough mana available to perform the attack
        attackContainer.isPerformingMagicalAttack = true;
        optionInfo.attackInfoText.SetActive(false);
        attackPicked = 5;
        if (playerMana.hasMana == true)
        {
            playerMana.CheckManaPool();
            if (attackContainer.isPerformingMagicalAttack && countEnemies.enemyAmount >= 2)
            {
                Debug.Log("More than one enemy in battle, choosing a target for this single target attack!");
                ChooseTarget();
            }
            else if (attackContainer.isPerformingMagicalAttack && countEnemies.enemyAmount == 1)
            {
                Debug.Log("Only one enemy in battle, attacking enemy!");
                attackContainer.enemyAttacked = 1;
                StartCoroutine(attackContainer.PerformTerrorBreath());
            }
        }
        else
        {
            Debug.Log("Does not have mana, cannot use Mana Attack");
            playerMana.CheckManaPool();
            playerMana.hasMana = false;
            return;
        }
    }

    public void HealingMagicSelection()
    {
        //When the player clicks the Healing Magic Attack, if the player has the required mana
        //They will instantly perform the action and gain some of their health back
        optionInfo.attackInfoText.SetActive(false);
        if (playerMana.hasMana == true)
        {
            damageContainer.isHealing = true;
            playerMana.CheckManaPool();
            StartCoroutine(attackContainer.PerformHealingBreath()); 
        }
        else
        {
            Debug.Log("Does not have mana, cannot use Mana Attack");
            playerMana.CheckManaPool();
            //optionInfo.attackInfoText.SetActive(false);
            playerMana.hasMana = false;
            return;
        }
    }

    public void DefendSelection()
    {
        //When the player clicks the Defend Option
        //They will instantly perform the action and reduce all damage taken by half/50% for a single turn
        if (gameState == GameStates.PlayerTurn)
        {
            
            damageContainer.isDefending = true;
            DisablePlayerButtons();
            StartCoroutine(PerformDefense());
        }
    }

    public IEnumerator PerformDefense()
    {
        //After the player clicks the Defend Option
        //They will be "Defending" against the enemies attacks and move to the enemy's turn
        if (gameState == GameStates.PlayerTurn)
        {
            if (damageContainer.isDefending)
            {
                Debug.Log("Player is Defending!");
                DisablePlayerButtons();
                damageContainer.PlayerDefense();
            }
            else
            {
                Debug.Log("Player is not Defending!");
                damageContainer.EnemyMagicalAttack();
                damageContainer.EnemyPhysicalAttack();
            }
            yield return new WaitForSeconds(1f);
            ShowCharacterCircles();
            turnCounter = 2;
            CheckTurnIndicator();
        }
    }

    public void ChooseTarget()
    {
        //The player will be able to choose an enemy to attack during battle
        //Depending on the amount of enemies counted at the start of battle, the player will be able to choose between 1-3 enemies at a time
        if (gameState == GameStates.PlayerTurn)
        {
            if (countEnemies.enemyAmount == 1)
            {
                Debug.Log("Only one enemy in battle, targeting only one enemy");
                //enemyButton[0].gameObject.SetActive(true);
                enemyButtons[0].gameObject.SetActive(true);
                battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(true);
            }
            if (countEnemies.enemyAmount >= 2)
            {
                Debug.Log("Two enemies in battle, Allow the player to target either enemy");
                //enemyButton[0].gameObject.SetActive(true);
                //enemyButton[1].gameObject.SetActive(true);
                enemyButtons[0].gameObject.SetActive(true);
                enemyButtons[1].gameObject.SetActive(true);
                battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(true);
                battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(true);
            }
            if (countEnemies.enemyAmount == 3)
            {
                Debug.Log("Two enemies in battle, Allow the player to target either enemy");
                //enemyButton[0].gameObject.SetActive(true);
                //enemyButton[1].gameObject.SetActive(true);
                //enemyButton[2].gameObject.SetActive(true);
                enemyButtons[0].gameObject.SetActive(true);
                enemyButtons[1].gameObject.SetActive(true);
                enemyButtons[2].gameObject.SetActive(true);
                battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(true);
                battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(true);
                battleEnemies[2].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(true);
            }
        }
    }

    public void EnemySelected()
    {
        //When an enemy has been selected to be attacked, the game will then decide which attack was performed
        //Based on the value of the attack picked before choosing the enemy to attack 
        int attackChosen = attackPicked;
        switch (attackChosen)
        {
            case 1:
                Debug.Log("Enemy Hit with Claw Strike!");
                attackChosen = 1;
                attackContainer.playerAttackDelegate = attackContainer.ActivateClawAttack;
                attackContainer.playerAttackDelegate();
                break;
            case 2:
                Debug.Log("Enemy Hit with Tail Swipe!");
                attackChosen = 2;
                attackContainer.playerAttackDelegate = attackContainer.ActivateTailAttack;
                attackContainer.playerAttackDelegate();
                break;
            case 3:
                Debug.Log("Enemy Hit with Fire Breath!");
                attackChosen = 3;
                attackContainer.playerAttackDelegate = attackContainer.ActivateFireBreathAttack;
                attackContainer.playerAttackDelegate();
                break;
            case 4:
                Debug.Log("Enemy Hit with Ice Breath!");
                attackChosen = 4;
                attackContainer.playerAttackDelegate = attackContainer.ActivateIceBreathAttack;
                attackContainer.playerAttackDelegate();
                break;
            case 5:
                Debug.Log("Enemy Hit with Terror Breath!");
                attackChosen = 5;
                attackContainer.playerAttackDelegate = attackContainer.ActivateTerrorBreathAttack;
                attackContainer.playerAttackDelegate();
                break;
            case 6:
                Debug.Log("Enemy Hit with Vengeance Rush!");
                attackChosen = 6;
                attackContainer.playerAttackDelegate = attackContainer.ActivateVengeanceRushAttack;
                attackContainer.playerAttackDelegate();
                break;
            case 7:
                Debug.Log("Enemy Hit with Vengeance Leap!");
                attackChosen = 7;
                attackContainer.playerAttackDelegate = attackContainer.ActivateVengeanceBreathAttack;
                attackContainer.playerAttackDelegate();
                break;
        }
    }

   

    public void ActivateVengeance()
    {
        //If it is the Player's Turn and the player has reached the designated Vengeance Meter threshold, 
        //Then the player's Vengeance has been Activated and can now use get ready to use their Vengeance Options
        if (gameState == GameStates.PlayerTurn && playerVengeanceMeter.hasVengeance == true)
        {
            Debug.Log("The Player has Vengeance!");
            VengeanceMode();
        }
        else
        {
            Debug.Log("You do not have Vengeance, this mode cannot be activated...");
            return;
        }
    }

    public void VengeanceMode()
    {
        //If it is the Player's Turn and the player has reached the designated Vengeance Meter threshold, 
        //Then the player's Vengeance has been Activated and can click the button to gain access to their Vengeance Attacks
        if (gameState == GameStates.PlayerTurn && playerVengeanceMeter.hasVengeance == true)
        {
            PlayerCombatAnimation.transformation = true;
            PlayerCombatAnimation.isBig = true;
            attackOptions.SetActive(false);
            magicOptions.SetActive(false);
            vengeanceButton.gameObject.SetActive(false);
            vengeanceOptions.SetActive(true);
        }
    }

    public void VengeanceRushSelection()
    {
        //Before performing the Vengeance Rush Attack, the player will need to choose which enemy to hit by clicking a button
        optionInfo.attackInfoText.SetActive(false);
        attackContainer.isPerformingVengeanceAttack = true;
        if (gameState == GameStates.PlayerTurn)
        {
            if (playerVengeanceMeter.hasVengeance == true)
            {
                if (attackContainer.isPerformingVengeanceAttack && countEnemies.enemyAmount >= 2)
                {
                    Debug.Log("More than one enemy in battle, choosing a target for this single target attack!");
                    attackPicked = 6;
                    ChooseTarget();
                }
                else if (attackContainer.isPerformingVengeanceAttack && countEnemies.enemyAmount == 1)
                {
                    Debug.Log("Only one enemy in battle, attacking enemy!");
                    attackContainer.enemyAttacked = 1;
                    StartCoroutine(attackContainer.PerformVengeanceRush());
                }
            }
            
        }
    }

    public void VengeanceBreathSelection()
    {
        //Before performing the Vengeance Rush Attack, the player will need to choose which enemy to hit by clicking a button
        optionInfo.attackInfoText.SetActive(false);
        attackContainer.isPerformingVengeanceAttack = true;
        if (gameState == GameStates.PlayerTurn)
        {
            if (playerVengeanceMeter.hasVengeance == true)
            {
                attackPicked = 7;
                StartCoroutine(attackContainer.PerformVengeanceBreath());
            }
        }
    }

    public void BackSelection()
    {
        //When the "Back" Button is pressed, all options and buttons are turned off and the combat buttons are activated
        //Any action the player was going to initiate will be turned off
        if (gameState == GameStates.PlayerTurn)
        {
            Debug.Log("The Player has backed out of their action!");
            attackOptions.SetActive(false);
            magicOptions.SetActive(false);
            returnButton.SetActive(false);
            if (countEnemies.enemyAmount == 1)
            {
                Debug.Log("Resetting enemy button...");
                //enemyButton[0].gameObject.SetActive(false);
                enemyButtons[0].gameObject.SetActive(false);
                foreach (var enemybattleButton in enemyButton)
                {
                    Debug.Log("Checking enemies in battle, enemy amount is equal to " + enemybattleButton);
                    enemybattleButton.gameObject.SetActive(false);
                }
            }
            if (countEnemies.enemyAmount == 2)
            {
                Debug.Log("Resetting additional enemy buttons...");
                //enemyButton[0].gameObject.SetActive(false);
                //enemyButton[1].gameObject.SetActive(false);
                enemyButtons[0].gameObject.SetActive(false);
                enemyButtons[1].gameObject.SetActive(false);
                foreach (var enemybattleButton in enemyButton)
                {
                    Debug.Log("Checking enemies in battle, enemy amount is equal to " + enemybattleButton);
                    enemybattleButton.gameObject.SetActive(false);
                }
            }
            if (countEnemies.enemyAmount == 3)
            {
                Debug.Log("Resetting additional enemy buttons...");
                //enemyButton[0].gameObject.SetActive(false);
                //enemyButton[1].gameObject.SetActive(false);
                //enemyButton[2].gameObject.SetActive(false);
                enemyButtons[0].gameObject.SetActive(false);
                enemyButtons[1].gameObject.SetActive(false);
                enemyButtons[2].gameObject.SetActive(false);
            }
            if (countEnemies.enemyAmount == 1)
            {
                Debug.Log("Resetting enemy target cursor...");
                battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
            if (countEnemies.enemyAmount == 2)
            {
                Debug.Log("Resetting enemy target cursors...");
                battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
                battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
            if (countEnemies.enemyAmount == 3)
            {
                Debug.Log("Resetting enemy target cursors...");
                battleEnemies[0].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
                battleEnemies[1].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
                battleEnemies[2].GetComponent<EnemyBattleStats>().enemyCursor.SetActive(false);
            }
            //Emily's code. Makes the dragon return to normal size if the player is backing out of the vengance menu
            if (PlayerCombatAnimation.isBig == true)
            {
                PlayerCombatAnimation.isNotBig = true;
                PlayerCombatAnimation.isBig = false;
            }
            vengeanceButton.gameObject.SetActive(false);
            vengeanceOptions.SetActive(false);
            combatButtons[0].interactable = true;
            combatButtons[1].interactable = true;
            combatButtons[2].interactable = true;
            attackContainer.isPerformingMagicalAttack = false;
            attackContainer.isPerformingPhysicalAttack = false;
        }
        else
        {
            Debug.Log("The Player has not backed out of their action...");
            return;
        }
    }

    
    public void EnemyTurn()
    {
        //When the enemy begins their turn, the game checks to see how many enemies are in the battle
        //Then after getting the count of the enemies, determines which enemies go first
        if (gameState == GameStates.EnemyTurn)
        {
            Debug.Log("Enemy is making their move!");
            battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (countEnemies.enemyAmount == 1)
            {
                Debug.Log("Only one enemy, the enemy will be making their move!");
                turnCounter = 2;
                firstEnemyActive = true;
                //countEnemies.enemyCombatSprites[0] = battleEnemies[0];
                if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyIndex == 2 || countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyIndex == 1 || countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyIndex == 3)
                {
                    Debug.Log("The enemy is deciding their action!");
                    ShowCharacterCircles();
                    DisablePlayerButtons();
                    EnemyTurnOrder();
                }
            }
            if (countEnemies.enemyAmount == 2)
            {
                Debug.Log("Two enemies in battle, the first enemy will be making their move!");
                battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
                if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyIndex == 1 && turnCounter == 2)
                {
                    Debug.Log("The first enemy is deciding their action!");
                    DisablePlayerButtons();
                    firstEnemyActive = true;
                    StartCoroutine(DetermineEnemyAction());
                }
                else if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyIndex == 2 && turnCounter == 2)
                {
                    Debug.Log("The first enemy is deciding their action!");
                    DisablePlayerButtons();
                    firstEnemyActive = true;
                    StartCoroutine(DetermineEnemyAction());
                }
                else if (countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyIndex == 2 && turnCounter == 3)
                {
                    Debug.Log("The second enemy is deciding their action!");
                    DisablePlayerButtons();
                    secondEnemyActive = true;
                    StartCoroutine(DetermineEnemyAction());
                }
            }
            if (countEnemies.enemyAmount == 3)
            {
                Debug.Log("Three enemies in battle, the first enemy will be making their move!");
                battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
                if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyIndex == 1 && turnCounter == 2)
                {
                    Debug.Log("The first enemy is deciding their action!");
                    DisablePlayerButtons();
                    firstEnemyActive = true;
                    StartCoroutine(DetermineEnemyAction());
                }
                else if (countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyIndex == 2 && turnCounter == 3)
                {
                    Debug.Log("The second enemy is deciding their action!");
                    DisablePlayerButtons();
                    secondEnemyActive = true;
                    StartCoroutine(DetermineEnemyAction());
                }
                else if (countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyIndex == 3 && turnCounter == 4)
                {
                    Debug.Log("The second enemy is deciding their action!");
                    DisablePlayerButtons();
                    thirdEnemyActive = true;
                    StartCoroutine(DetermineEnemyAction());
                }
            }
        }
    }

    public IEnumerator DetermineEnemyAction()
    {
        DisablePlayerButtons();
        DecideAttackOption();
        if (enemyAttackValue == 0 || enemyAttackValue == 2 || enemyAttackValue == 4 || enemyAttackValue == 6)
        {
            Debug.Log("Enemy is using a Physical Attack!");
            yield return new WaitForSeconds(2f);
            StartCoroutine(EnemyAttack());
            yield return new WaitForSeconds(1f);
            TurnCheck();
        }
        else if (enemyAttackValue == 1 || enemyAttackValue == 3 || enemyAttackValue == 5 || enemyAttackValue == 7)
        {
            Debug.Log("Enemy is using a Magic Attack!");
            yield return new WaitForSeconds(2f);
            StartCoroutine(EnemyMagicAttack());
            yield return new WaitForSeconds(1f);
            //CheckTurnIndicator();
        }
    }

    public void DecideAttackOption()
    {
        Debug.Log("Determining Enemy Action " + enemyAttackValue);
        enemyAttackValue = Random.Range(0, 7);
    }

    private IEnumerator EnemyAttack()
    {
        //Enemy will perform attack against the player and end their turn
        Debug.Log("The Enemy has hit you with a Physical Attack!");
        damageContainer.performingAttack = true;
        DisablePlayerButtons();
        if (firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "EnemyWolf" || firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "EnemyWolf 2")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            countEnemies.enemyCombatSprites[0].GetComponent<WolfEnemy>().PerformPhysicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 10.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        if (secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "EnemyWolf 2" || secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "EnemyWolf")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            countEnemies.enemyCombatSprites[1].GetComponent<WolfEnemy>().PerformPhysicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 10.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        else
        {
            Debug.Log("No Wolf is attacking...");
            yield return null;
        }
        if (firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "EnemyHawk" || firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "EnemyHawk 2")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            countEnemies.enemyCombatSprites[0].GetComponent<HawkEnemy>().PerformPhysicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 10.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        if (secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "EnemyHawk" || secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "EnemyHawk 2")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            countEnemies.enemyCombatSprites[1].GetComponent<HawkEnemy>().PerformPhysicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 10.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        else
        {
            Debug.Log("No Hawk is attacking...");
            yield return null;
        }
        if (firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "EnemyVillager" || firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "EnemyVillager 2")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            countEnemies.enemyCombatSprites[0].GetComponent<VillagerEnemy>().PerformPhysicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 15.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        if (secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "EnemyVillager 2")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            countEnemies.enemyCombatSprites[1].GetComponent<VillagerEnemy>().PerformPhysicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 15.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        else
        {
            Debug.Log("No Villager is attacking...");
            yield return null;
        }
        if (firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "Fist Hero" || secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "Fist Hero")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            //countEnemies.enemyCombatSprites[0].GetComponent<FistHeroEnemy>().PerformPhysicalAttack();
            GameObject.Find("Fist Hero").GetComponent<FistHeroEnemy>().PerformPhysicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 15.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        else if (secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "Mage Hero" || firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "Mage Hero")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            //countEnemies.enemyCombatSprites[1].GetComponent<MageHeroEnemy>().PerformPhysicalAttack();
            GameObject.Find("Mage Hero").GetComponent<MageHeroEnemy>().PerformPhysicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 15.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        else if (thirdEnemyActive && countEnemies.enemyCombatSprites[2].name == "Spear Hero" || secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "Spear Hero" || firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "Spear Hero")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().PerformPhysicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 15.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        else
        {
            Debug.Log("No Heroes is attacking...");
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        damageContainer.performingAttack = false;
        damageContainer.isDefending = false;
        if (damageContainer.isDefending == false)
        {
            Debug.Log("Player is not defending anymore or has not opted to defend themselves, resetting enemy damage");
            damageContainer.EnemyMagicalAttack();
            damageContainer.EnemyPhysicalAttack();
        }
        if (turnCounter == 2 && firstEnemyActive == true)
        {
            Debug.Log("The first enemy has completed their actions");
            firstEnemyTurnCompleted = true;
        }
        if (turnCounter == 3)
        {
            Debug.Log("The second enemy has completed their actions");
            secondEnemyTurnCompleted = true;
        }
        if (turnCounter == 4)
        {
            Debug.Log("The second enemy has completed their actions");
            thirdEnemyTurnCompleted = true;
        }
        TurnCheck();
        ShowCharacterCircles();
        if (turnCounter != 1)
        {
            Debug.Log("Still not the player's turn, disable player buttons and panels");
            DisablePlayerButtons();
        }
        else
        {
            Debug.Log("It is the player's turn now, enable player buttons and panels");
            ActivatePlayerButtons();
        }
    }

    private IEnumerator EnemyMagicAttack()
    {
        //Enemy will perform attack against the player and end their turn
        Debug.Log("The Enemy has hit you with a Magical Attack!");
        damageContainer.performingAttack = true;
        DisablePlayerButtons();
        if (firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "EnemyWolf" || firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "EnemyWolf 2")
        {
            Debug.Log("The Enemy used their magical strength to hurt the player");
            countEnemies.enemyCombatSprites[0].GetComponent<WolfEnemy>().PerformMagicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 15.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        if (secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "EnemyWolf 2" || secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "EnemyWolf")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            countEnemies.enemyCombatSprites[1].GetComponent<WolfEnemy>().PerformMagicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 10.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        else
        {
            Debug.Log("No Wolf is attacking...");
            yield return null;
        }
        if (firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "EnemyHawk" || firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "EnemyHawk 2")
        {
            Debug.Log("The Hawk Enemy used their physical strength to hurt the player");
            countEnemies.enemyCombatSprites[0].GetComponent<HawkEnemy>().PerformMagicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 10.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        if (secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "EnemyHawk" || secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "EnemyHawk 2")
        {
            Debug.Log("The Hawk Enemy used their magical strength to hurt the player");
            countEnemies.enemyCombatSprites[1].GetComponent<HawkEnemy>().PerformMagicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 10.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        else
        {
            Debug.Log("No Hawk is attacking...");
            yield return null;
        }
        if (firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "EnemyVillager" || firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "EnemyVillager 2")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            countEnemies.enemyCombatSprites[0].GetComponent<VillagerEnemy>().PerformMagicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 15.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        if (secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "EnemyVillager 2")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            countEnemies.enemyCombatSprites[1].GetComponent<VillagerEnemy>().PerformMagicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 15.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        else
        {
            Debug.Log("No Villager is attacking...");
            yield return null;
        }
        if (firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "Fist Hero" || secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "Fist Hero")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            //countEnemies.enemyCombatSprites[0].GetComponent<FistHeroEnemy>().PerformMagicalAttack();
            GameObject.Find("Fist Hero").GetComponent<FistHeroEnemy>().PerformMagicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 15.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        else if (secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "Mage Hero" || firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "Mage Hero")
        {
            Debug.Log("The Enemy used their magical strength to hurt the player");
            //countEnemies.enemyCombatSprites[1].GetComponent<MageHeroEnemy>().PerformMagicalAttack();
            GameObject.Find("Mage Hero").GetComponent<MageHeroEnemy>().PerformMagicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 15.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        else if (thirdEnemyActive && countEnemies.enemyCombatSprites[2].name == "Spear Hero" || secondEnemyActive && countEnemies.enemyCombatSprites[1].name == "Spear Hero" || firstEnemyActive && countEnemies.enemyCombatSprites[0].name == "Spear Hero")
        {
            Debug.Log("The Enemy used their physical strength to hurt the player");
            //countEnemies.enemyCombatSprites[2].GetComponent<SpearHeroEnemy>().PerformMagicalAttack();
            GameObject.Find("Spear Hero").GetComponent<SpearHeroEnemy>().PerformMagicalAttack();
            playerVengeanceMeter.playerCurrentVengeance += 15.0f;
            playerVengeanceMeter.playerVengeanceBar.fillAmount = playerVengeanceMeter.playerCurrentVengeance / 100;
            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
                playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
            }
            playerVengeanceMeter.playerVengeanceText.text = playerVengeanceMeter.playerCurrentVengeance + " VP " + " / " + playerVengeanceMeter.playerMaxVengeance;
        }
        else
        {
            Debug.Log("No Heroes is attacking...");
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        damageContainer.performingAttack = false;
        damageContainer.isDefending = false;
        if (damageContainer.isDefending == false)
        {
            Debug.Log("Player is not defending anymore or has not opted to defend themselves, resetting enemy damage");
            damageContainer.EnemyMagicalAttack();
            damageContainer.EnemyPhysicalAttack();
        }
        if (turnCounter == 2)
        {
            Debug.Log("The first enemy has completed their actions");
            firstEnemyTurnCompleted = true;
        }
        if (turnCounter == 3)
        {
            Debug.Log("The second enemy has completed their actions");
            secondEnemyTurnCompleted = true;
        }
        if (turnCounter == 4)
        {
            Debug.Log("The second enemy has completed their actions");
            thirdEnemyTurnCompleted = true;
        }
        TurnCheck();
        ShowCharacterCircles();
        if (turnCounter != 1)
        {
            Debug.Log("Still not the player's turn, disable player buttons and panels");
            DisablePlayerButtons();
        }
        else
        {
            Debug.Log("It is the player's turn now, enable player buttons and panels");
            ActivatePlayerButtons();
        }
        
    }

    public void TurnCheck()
    {
        //Checks to see if it is the player's turn or the enemy's turn.
        //If multiple enemies are in a scene, then the enemy's turn will repeat but for the next enemy's turn
        //The enemies will have a index/indicator number ranging between 1-3 for each enemy
        //If the first enemy is executing their turn, their index will be 1, etc.
        Debug.Log("Checking to see who's turn it currently is during the battle...");
        if (countEnemies.enemyAmount == 1 && firstEnemyTurnCompleted == true)
        {
            if (turnCounter == 2)
            {
                Debug.Log("First enemy completed their turn, the player will now go next");
                turnCounter = 1;
                CheckTurnIndicator();
            }

        }
        if (countEnemies.enemyAmount == 2 && firstEnemyTurnCompleted == true)
        {
            if (turnCounter == 2)
            {
                Debug.Log("First enemy completed their turn, the second enemy will go next");
                firstEnemyActive = false;
                turnCounter = 3;
                CheckTurnIndicator();
            }
            if (turnCounter == 3 && secondEnemyTurnCompleted == true)
            {
                Debug.Log("Second enemy completed their turn, the player will go next");
                secondEnemyActive = false;
                firstEnemyTurnCompleted = false;
                secondEnemyTurnCompleted = false;
                turnCounter = 1;
                CheckTurnIndicator();
            }
        }
        if (countEnemies.enemyAmount == 3 && firstEnemyTurnCompleted == true)
        {
            if (turnCounter == 2)
            {
                Debug.Log("First enemy completed their turn, the second enemy will go next");
                firstEnemyActive = false;
                turnCounter = 3;
                CheckTurnIndicator();
            }
            if (turnCounter == 3 && secondEnemyTurnCompleted == true)
            {
                Debug.Log("Second enemy completed their turn, the third enemy will go next");
                secondEnemyActive = false;
                turnCounter = 4;
                CheckTurnIndicator();
            }
            if (turnCounter == 4 && thirdEnemyTurnCompleted == true)
            {
                Debug.Log("Third enemy completed their turn, the player will go next");
                thirdEnemyActive = false;
                secondEnemyActive = false;
                firstEnemyActive = false;
                thirdEnemyTurnCompleted = false;
                secondEnemyTurnCompleted = false;
                firstEnemyTurnCompleted = false;
                turnCounter = 1;
                CheckTurnIndicator();
            }

        }
    }

    public void EnemyTurnOrder()
    {
        if (firstEnemyActive == true && turnCounter == 2)
        {
            Debug.Log("First Enemy indicated, the first enemy will begin their attack!");
            damageContainer.EnemyPhysicalAttack();
            damageContainer.EnemyMagicalAttack();
            StartCoroutine(DetermineEnemyAction());
        }
        else if (secondEnemyActive == true && turnCounter == 3)
        {
            Debug.Log("Second Enemy indicated, the second enemy will begin their attack!");
            damageContainer.EnemyPhysicalAttack();
            damageContainer.EnemyMagicalAttack();
            StartCoroutine(DetermineEnemyAction());
        }
        else if (thirdEnemyActive == true && turnCounter == 4)
        {
            Debug.Log("Second Enemy indicated, the second enemy will begin their attack!");
            damageContainer.EnemyPhysicalAttack();
            damageContainer.EnemyMagicalAttack();
            StartCoroutine(DetermineEnemyAction());
        }
    }

    public void CheckTurnIndicator()
    {
        //Determines what the turn counter is and switches turns between the player and the enemy
        //If the turn counter is 1, it is the player's turn;
        //If the turn counter is 2 and only one enemy is currently active in battle, then it is the first enemy's turn
        //If the turn counter is 2 and more than one enemy is active in battle, then it will be the first enemy's turn
        //If the turn counter is 3, more than one enemy is active in battle, and the first enemy completed their turn, then it is the next enemy's turn
        //If the turn counter is 4, more than two enemy is active in battle, and the first and second enemy completed their turns, then it is the next enemy's turn
        //Once the enemy has completed their turn(s), then it goes back to 1 which is the player's turn
        Debug.Log("Checking who's making their actions during battle");
        if (turnCounter == 1)
        {
            Debug.Log("The Turn Counter is one, it is now the player's turn!");
            gameState = GameStates.PlayerTurn;
            PlayerTurn();
        }
        if (turnCounter == 2)
        {
            Debug.Log("The Turn Counter is two, it is now the first/single enemy's turn!");
            firstEnemyActive = true;
            EnemyTurnOrder();
            gameState = GameStates.EnemyTurn;
            
        }
        if (turnCounter == 3)
        {
            Debug.Log("The Turn Counter is three, it is now the second enemy's turn!");
            secondEnemyActive = true;
            EnemyTurnOrder();
            gameState = GameStates.EnemyTurn;
        }
        if (turnCounter == 4)
        {
            Debug.Log("The Turn Counter is three, it is now the second enemy's turn!");
            thirdEnemyActive = true;
            EnemyTurnOrder();
            gameState = GameStates.EnemyTurn;
        }
    }

    public void TurnOffButtons()
    {
        returnButton.SetActive(false);
        attackOptions.SetActive(false);
        magicOptions.SetActive(false);
        optionInfo.attackInfoText.SetActive(false);
        if (countEnemies.enemyAmount == 1)
        {
            Debug.Log("Resetting enemy button...");
            //enemyButton[0].gameObject.SetActive(false);
            enemyButtons[0].gameObject.SetActive(false);
        }
        if (countEnemies.enemyAmount == 2)
        {
            Debug.Log("Resetting additional enemy buttons...");
            //enemyButton[0].gameObject.SetActive(false);
            //enemyButton[1].gameObject.SetActive(false);
            enemyButtons[0].gameObject.SetActive(false);
            enemyButtons[1].gameObject.SetActive(false);
        }
        vengeanceButton.gameObject.SetActive(false);
        vengeanceOptions.SetActive(false);
    }

    public void DisablePlayerButtons()
    {
        //Turns off the player's buttons when performing an action during their turn or the enemy's turn.
        Debug.Log("Turning off Player Buttons and Panels...");
        combatPanels[0].SetActive(false);
        combatPanels[1].SetActive(false);
        combatPanels[2].SetActive(false);
        combatButtons[0].interactable = false;
        combatButtons[1].interactable = false;
        combatButtons[2].interactable = false;
    }

    public void ActivatePlayerButtons()
    {
        //Turns off the player's buttons when performing an action during their turn or the enemy's turn.
        Debug.Log("Turning on Player Buttons and Panels!");
        combatPanels[0].SetActive(true);
        combatPanels[1].SetActive(true);
        combatPanels[2].SetActive(true);
        combatButtons[0].interactable = true;
        combatButtons[1].interactable = true;
        combatButtons[2].interactable = true;
    }

    public void ShowCharacterCircles()
    {
        if (gameState == GameStates.PlayerTurn)
        {
            Debug.Log("It is now the player's turn!");
            playerTurnSignal.SetActive(true);
        }
        else
        {
            Debug.Log("It is not the player's turn anymore...");
            playerTurnSignal.SetActive(false);
        }
        if (gameState == GameStates.EnemyTurn)
        {
            Debug.Log("It is now the enemy's turn!");
        }
        else
        {
            Debug.Log("It is not the enemy's turn anymore...");
        }
    }

    public void PlayerDeath()
    {
        //When player loses all of their HP, they will die and the Game Over Panel or Scene will load
        if (playerStats.healthStat <= 0)
        {
            
            Destroy(GameObject.Find("Player"), 0.4f);
            StartCoroutine(PlayerDefeated());
        }
    }

    public IEnumerator PlayerDefeated()
    {
        yield return new WaitForSeconds(2f);
        reloadBattle.LoadPreviousBattle();
    }

    public void EnemyDeath()
    {
        //When enemies lose all of their HP, they will die and their sprite will be deactivated and a Battle End Panel will load
        if (countEnemies.enemyAmount == 1)
        {
            if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("First Enemy have been defeated");
                Destroy(battleEnemies[0], 0.4f);
                StartCoroutine(BattleEnded());
            }
            else if (countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Second Enemy have been defeated");
                Destroy(battleEnemies[1], 0.4f);
                StartCoroutine(BattleEnded());
            }
        }
        if (countEnemies.enemyAmount == 2)
        {
            if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Enemies 1 have been defeated");
                //Destroy(battleEnemies[0], 0.4f);
                //countEnemies.enemyCombatSprites[0].SetActive(false);
                Destroy(countEnemies.enemyCombatSprites[0], 0.4f);
                countEnemies.enemyCombatSprites.RemoveAt(0);
                countEnemies.enemyAmount--;
                //enemyButton[0].gameObject.SetActive(false);
                enemyButtons[0].gameObject.SetActive(false);
                if (turnCounter == 2)
                {
                    Debug.Log("Enemy 1 has been defeated, it is now Enemy 2's turn");
                    turnCounter = 3;
                    TurnCheck();
                }
            }
            else if (countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Enemies 2 have been defeated");
                //Destroy(battleEnemies[1], 0.4f);
                Destroy(countEnemies.enemyCombatSprites[1], 0.4f);
                //battleEnemies[0].SetActive(false);
                countEnemies.enemyCombatSprites.RemoveAt(1);
                countEnemies.enemyAmount--;
                //enemyButton[1].gameObject.SetActive(false);
                enemyButtons[1].gameObject.SetActive(false);
            }
            if (countEnemies.enemyAmount == 0)
            {
                Debug.Log("All enemies removed, ending battle");
                Destroy(countEnemies.enemyCombatSprites[0]);
                Destroy(countEnemies.enemyCombatSprites[1]);
                //countEnemies.enemyCombatSprites[0].SetActive(false);
                //countEnemies.enemyCombatSprites[1].SetActive(false);
                countEnemies.enemyCombatSprites.RemoveAt(0);
                StartCoroutine(BattleEnded());
            }
        }
        if (countEnemies.enemyAmount == 3)
        {
            if (countEnemies.enemyCombatSprites[0].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Enemies 1 have been defeated");
                //Destroy(battleEnemies[0], 0.4f);
                //countEnemies.enemyCombatSprites[0].SetActive(false);
                Destroy(countEnemies.enemyCombatSprites[0], 0.4f);
                countEnemies.enemyCombatSprites.RemoveAt(0);
                countEnemies.enemyAmount--;
                //enemyButton[0].gameObject.SetActive(false);
                enemyButtons[0].gameObject.SetActive(false);
                enemyButtons.RemoveAt(0);
                if (turnCounter == 2)
                {
                    Debug.Log("Enemy 1 has been defeated, it is now Enemy 2's turn");
                    turnCounter = 3;
                    TurnCheck();
                }
            }
            else if (countEnemies.enemyCombatSprites[1].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Enemies 2 have been defeated");
                //Destroy(battleEnemies[1], 0.4f);
                Destroy(countEnemies.enemyCombatSprites[1], 0.4f);
                //battleEnemies[0].SetActive(false);
                countEnemies.enemyCombatSprites.RemoveAt(1);
                countEnemies.enemyAmount--;
                //enemyButton[1].gameObject.SetActive(false);
                enemyButtons[1].gameObject.SetActive(false);
                enemyButtons.RemoveAt(1);
                //Destroy(enemyButton[1], 0.4f);
            }
            else if (countEnemies.enemyCombatSprites[2].GetComponent<EnemyBattleStats>().enemyHealthStat <= 0)
            {
                Debug.Log("Enemies 2 have been defeated");
                //Destroy(battleEnemies[1], 0.4f);
                Destroy(countEnemies.enemyCombatSprites[2], 0.4f);
                //battleEnemies[0].SetActive(false);
                countEnemies.enemyCombatSprites.RemoveAt(2);
                countEnemies.enemyAmount--;
                //enemyButton[2].gameObject.SetActive(false);
                enemyButtons[2].gameObject.SetActive(false);
                enemyButtons.RemoveAt(2);
                //Destroy(enemyButton[2], 0.4f);
            }
            if (countEnemies.enemyAmount == 0)
            {
                Debug.Log("All enemies removed, ending battle");
                Destroy(countEnemies.enemyCombatSprites[0]);
                Destroy(countEnemies.enemyCombatSprites[1]);
                Destroy(countEnemies.enemyCombatSprites[2]);
                //countEnemies.enemyCombatSprites[0].SetActive(false);
                //countEnemies.enemyCombatSprites[1].SetActive(false);
                countEnemies.enemyCombatSprites.RemoveAt(0);
                StartCoroutine(BattleEnded());
            }
        }
    }

    public void MultipleEnemyDeath()
    {
        //If both enemies die at the same time, then both enemies are destroyed
        //And the battle ends automatically
        if (countEnemies.enemyAmount >= 1)
        {
            Debug.Log("Destroying all enemies");
            battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in battleEnemies)
            {
                Debug.Log("Both enemies defeated at once, destroying both enemies");
                Destroy(enemy, 0.1f);
                //enemy.SetActive(false);
                countEnemies.enemyAmount--;
                countEnemies.enemyCombatSprites.RemoveAt(0);
                countEnemies.enemyCombatSprites.RemoveAt(1);
                if (countEnemies.enemyAmount == 0)
                {
                    Debug.Log("Both enemies are destroyed, ending the battle...");
                    turnCounter = 1;
                    CheckTurnIndicator();
                    StartCoroutine(BattleEnded());
                }

            }
        }
    }

    public void MultiEnemyDeathCopy()
    {
        if (countEnemies.enemyAmount == 2)
        {
            battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in battleEnemies)
            {
                Debug.Log("Both enemies defeated at once, destroying both enemies");
                enemy.SetActive(false);
                countEnemies.enemyAmount--;
                if (countEnemies.enemyAmount == 0)
                {
                    Debug.Log("Both enemies are destroyed, ending the battle...");
                    turnCounter = 1;
                    CheckTurnIndicator();
                    StartCoroutine(BattleEnded());
                }

            }
        }
    }

    public IEnumerator BattleEnded()
    {
        yield return new WaitForSeconds(2f);
        experienceSystem.ReceiveExperiencePoints();
        yield return new WaitForSeconds(3f);
        if (SceneManager.GetActiveScene().name == "TutorialCombat")
        {
            SceneManager.LoadScene("TutorialCave");
        }
        if (SceneManager.GetActiveScene().name == "SingleWolfCombat")
        {
            SceneManager.LoadScene(battleScenesManager.previousSceneName);
        }
        if (SceneManager.GetActiveScene().name == "SingleHawkCombat")
        {
            SceneManager.LoadScene(battleScenesManager.previousSceneName);
        }
        if (SceneManager.GetActiveScene().name == "MultipleWolfCombat")
        {
            SceneManager.LoadScene(battleScenesManager.previousSceneName);
        }
        if (SceneManager.GetActiveScene().name == "MultipleHawkCombat")
        {
            SceneManager.LoadScene(battleScenesManager.previousSceneName);
        }
        if (SceneManager.GetActiveScene().name == "WolfHawkCombat")
        {
            SceneManager.LoadScene(battleScenesManager.previousSceneName);
        }
        if (SceneManager.GetActiveScene().name == "SingleVillagerCombat")
        {
            SceneManager.LoadScene("Village");
        }
        if (SceneManager.GetActiveScene().name == "MultipleVillagerCombat")
        {
            SceneManager.LoadScene("CastleTown");
        }
        if (SceneManager.GetActiveScene().name == "TownVillagerCombat")
        {
            SceneManager.LoadScene("CastleTown");
        }
        if (SceneManager.GetActiveScene().name == "FinalBattleCombat")
        {
            SceneManager.LoadScene("EndScreen");
        }
    }
}
