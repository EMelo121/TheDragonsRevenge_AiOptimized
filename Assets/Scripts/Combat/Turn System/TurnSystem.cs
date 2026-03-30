using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum GameStates { Start, PlayerTurn, EnemyTurn }

public class TurnSystem : MonoBehaviour
{
    [Header("Battle State")]
    [Tooltip("Tracks the current overall battle state.")]
    public GameStates gameState;

    [Header("Battle Participants")]
    [Tooltip("The player's battle sprite.")]
    public GameObject playerSprite;

    [Tooltip("Container or parent object holding enemy battle sprites.")]
    public GameObject enemySprites;

    [Tooltip("The active enemies in the current battle.")]
    public GameObject[] battleEnemies;

    [Header("Turn Indicators")]
    [Tooltip("Visual indicator shown when it is the player's turn.")]
    public GameObject playerTurnSignal;

    [Header("Combat Buttons")]
    [Tooltip("Primary combat buttons such as Attack, Magic, and Defend.")]
    public Button[] combatButtons;

    [Tooltip("Button used to activate Vengeance Mode.")]
    public Button vengeanceButton;

    [Tooltip("Legacy array of enemy target buttons.")]
    public Button[] enemyButton;

    [Tooltip("List of active enemy target buttons used during targeting.")]
    public List<Button> enemyButtons = new List<Button>();

    [Tooltip("Buttons unlocked progressively as the player levels.")]
    public Button[] progressionButtons;

    [Header("Combat Menus")]
    [Tooltip("Panel containing attack options.")]
    public GameObject attackOptions;

    [Tooltip("Panel containing magic options.")]
    public GameObject magicOptions;

    [Tooltip("Main combat panels shown during battle.")]
    public GameObject[] combatPanels;

    [Tooltip("Panel containing Vengeance Mode options.")]
    public GameObject vengeanceOptions;

    [Tooltip("Return button used to back out of sub-menus.")]
    public GameObject returnButton;

    [Header("Combat Flow")]
    [Tooltip("The attack selected by the player.")]
    public int attackPicked;

    [Tooltip("Tracks the current turn stage. 1 = Player, 2+ = Enemy turns.")]
    public int turnCounter;

    [Tooltip("Randomized value used to choose an enemy attack type.")]
    public int enemyAttackValue;

    // Core combat references.
    private PlayerHealth playerHealth;
    private PlayerMana playerMana;
    private PlayerVengeanceMeter playerVengeanceMeter;
    private DamageContainer damageContainer;
    private BattleScenesManager battleScenesManager;
    private SelectEnemies selectEnemies;
    private AttackContainer attackContainer;
    private OptionInfo optionInfo;
    private StatSystem statSystem;
    private ExperienceSystem experienceSystem;
    private PlayerStats playerStats;
    private ReloadBattle reloadBattle;
    private CountEnemies countEnemies;

    // Enemy turn-state flags used by existing enemy turn signal logic.
    public bool firstEnemyActive;
    public bool firstEnemyTurnCompleted;
    public bool secondEnemyActive;
    public bool secondEnemyTurnCompleted;
    public bool thirdEnemyActive;
    public bool thirdEnemyTurnCompleted;

    private void Awake()
    {
        damageContainer = FindObjectOfType<DamageContainer>();
        attackContainer = FindObjectOfType<AttackContainer>();
        experienceSystem = FindObjectOfType<ExperienceSystem>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMana = FindObjectOfType<PlayerMana>();
        playerVengeanceMeter = FindObjectOfType<PlayerVengeanceMeter>();
        battleScenesManager = FindObjectOfType<BattleScenesManager>();
        selectEnemies = FindObjectOfType<SelectEnemies>();
        optionInfo = FindObjectOfType<OptionInfo>();
        statSystem = FindObjectOfType<StatSystem>();
        playerStats = FindObjectOfType<PlayerStats>();
        reloadBattle = FindObjectOfType<ReloadBattle>();
        countEnemies = FindObjectOfType<CountEnemies>();
    }

    private void Start()
    {
        // AI revision note:
        // The original script mixed battle setup, UI refresh, enemy-specific initialization,
        // and turn preparation inside one long startup path. This version preserves the
        // same sequence but organizes setup into focused helper methods.
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
        InitializeBattleUI();
        InitializePlayerBattleValues();
        InitializeBattleEnemies();

        yield return new WaitForSeconds(2f);

        statSystem.DetermineSpeed();

        if (gameState == GameStates.PlayerTurn)
        {
            Debug.Log("Beginning Player's Turn...");
            turnCounter = 1;
            PlayerTurn();
        }
    }

    /// <summary>
    /// Initializes the base combat UI state when the battle begins.
    /// </summary>
    private void InitializeBattleUI()
    {
        combatPanels[0].SetActive(false);
        combatPanels[1].SetActive(false);
        combatPanels[2].SetActive(false);
        playerTurnSignal.SetActive(false);
        attackOptions.SetActive(false);
        magicOptions.SetActive(false);
        vengeanceOptions.SetActive(false);
        returnButton.SetActive(false);
    }

    /// <summary>
    /// Initializes the player's battle-facing UI values at combat start.
    /// </summary>
    private void InitializePlayerBattleValues()
    {
        playerHealth.playerCurrentHealth = playerStats.healthStat;
        playerHealth.playerHealthBar.fillAmount = playerStats.maxHealthStat > 0
            ? playerStats.healthStat / playerStats.maxHealthStat
            : 0f;
        playerHealth.playerHealthText.text = playerStats.healthStat + " / " + playerStats.maxHealthStat;

        playerMana.playerCurrentMana = playerStats.manaStat;
        playerMana.playerManaBar.fillAmount = playerStats.maxManaStat > 0
            ? playerStats.manaStat / playerStats.maxManaStat
            : 0f;
        playerMana.playerManaText.text = playerStats.manaStat + " MP / " + playerStats.maxManaStat;

        playerVengeanceMeter.playerVengeanceText.text =
            playerVengeanceMeter.playerCurrentVengeance + " VP / " + playerVengeanceMeter.playerMaxVengeance;
        playerVengeanceMeter.playerVengeanceBar.fillAmount =
            playerVengeanceMeter.playerCurrentVengeance / 100f;

        experienceSystem.experienceText.text =
            playerStats.playerCurrentExp + " Exp / " + playerStats.playerNextLevelExp;
        experienceSystem.levelText.text = " Lv " + playerStats.playerCurrentLevel;
        experienceSystem.experienceBar.fillAmount = playerStats.playerNextLevelExp > 0
            ? (float)playerStats.playerCurrentExp / playerStats.playerNextLevelExp
            : 0f;
    }

    /// <summary>
    /// Finds and initializes all active enemies in the battle using the shared EnemyBattleStats base class.
    /// </summary>
    private void InitializeBattleEnemies()
    {
        battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (countEnemies.enemyCombatSprites != null && countEnemies.enemyCombatSprites.Count > 0)
        {
            battleEnemies = countEnemies.enemyCombatSprites.ToArray();
        }

        foreach (GameObject enemyObject in battleEnemies)
        {
            if (enemyObject == null)
            {
                continue;
            }

            EnemyBattleStats enemyStats = enemyObject.GetComponent<EnemyBattleStats>();

            if (enemyStats == null)
            {
                continue;
            }

            InitializeEnemyUI(enemyStats);

            if (enemyStats.enemyCursor != null)
            {
                enemyStats.enemyCursor.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Refreshes the visible health and mana UI for a specific enemy.
    /// </summary>
    /// <param name="enemyStats">The enemy whose UI should be initialized.</param>
    private void InitializeEnemyUI(EnemyBattleStats enemyStats)
    {
        if (enemyStats.enemyHealthText != null)
        {
            enemyStats.enemyHealthText.text = enemyStats.enemyHealthStat + " / " + enemyStats.enemyMaxHealthStat;
        }

        if (enemyStats.enemyManaText != null)
        {
            enemyStats.enemyManaText.text = enemyStats.enemyManaStat + " / " + enemyStats.enemyMaxManaStat;
        }

        if (enemyStats.enemyHealthBar != null)
        {
            enemyStats.enemyHealthBar.fillAmount = enemyStats.enemyMaxHealthStat > 0
                ? enemyStats.enemyHealthStat / enemyStats.enemyMaxHealthStat
                : 0f;
        }

        if (enemyStats.enemyManaBar != null)
        {
            enemyStats.enemyManaBar.fillAmount = enemyStats.enemyMaxManaStat > 0
                ? enemyStats.enemyManaStat / enemyStats.enemyMaxManaStat
                : 0f;
        }
    }

    public void CheckPlayerProgression()
    {
        // Determines which attacks the player can use based on current level.
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
        if (gameState == GameStates.PlayerTurn)
        {
            combatPanels[0].SetActive(true);
            combatPanels[1].SetActive(true);
            combatPanels[2].SetActive(true);

            attackOptions.SetActive(false);
            magicOptions.SetActive(false);
            vengeanceOptions.SetActive(false);

            SetAllEnemyButtons(false);
            SetAllEnemyCursors(false);

            ShowCharacterCircles();

            playerMana.playerManaBar.fillAmount = playerStats.maxManaStat > 0
                ? playerStats.manaStat / playerStats.maxManaStat
                : 0f;

            experienceSystem.experienceBar.fillAmount = playerStats.playerNextLevelExp > 0
                ? (float)playerStats.playerCurrentExp / playerStats.playerNextLevelExp
                : 0f;
        }

        RegeneratePlayerMana();

        if (playerStats.healthStat <= 0)
        {
            Debug.Log("Player at 0 HP, Player has lost the battle!");
            DisablePlayerButtons();
            PlayerDeath();
        }

        if (playerVengeanceMeter.hasVengeance == false)
        {
            vengeanceButton.gameObject.SetActive(false);
            vengeanceOptions.SetActive(false);
        }

        if (playerVengeanceMeter.playerCurrentVengeance >= 50.0f)
        {
            Debug.Log("Player can use Vengeance Attacks!");
            playerVengeanceMeter.hasVengeance = true;

            if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
            {
                playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
            }
        }

        if (damageContainer.isDefending == false)
        {
            damageContainer.PlayerDefense();
        }
    }

    /// <summary>
    /// Regenerates player mana at the beginning of the player's turn.
    /// </summary>
    private void RegeneratePlayerMana()
    {
        if (playerMana.playerCurrentMana >= playerMana.playerMaxMana)
        {
            playerMana.playerCurrentMana = playerMana.playerMaxMana;
            Debug.Log("Mana Pool full!");
        }
        else
        {
            Debug.Log("Regenerating a bit of Mana!");
            playerStats.manaStat += 10.0f;

            if (playerStats.manaStat >= playerMana.playerMaxMana)
            {
                playerStats.manaStat = playerMana.playerMaxMana;
            }

            playerMana.playerCurrentMana = playerStats.manaStat;
            playerMana.playerManaBar.fillAmount = playerStats.maxManaStat > 0
                ? playerStats.manaStat / playerStats.maxManaStat
                : 0f;
            playerMana.playerManaText.text = playerMana.playerCurrentMana + " MP / " + playerStats.maxManaStat;
        }
    }

    public void AttackButton()
    {
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
        attackContainer.isPerformingPhysicalAttack = true;
        optionInfo.attackInfoText.SetActive(false);
        attackPicked = 2;

        if (attackContainer.isPerformingPhysicalAttack)
        {
            StartCoroutine(attackContainer.PerformTailAttack());
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
        }
    }

    public void IceBreathAttackSelection()
    {
        attackContainer.isPerformingMagicalAttack = true;
        optionInfo.attackInfoText.SetActive(false);
        attackPicked = 4;

        if (playerMana.hasMana == true)
        {
            playerMana.CheckManaPool();
            StartCoroutine(attackContainer.PerformIceBreath());
        }
        else
        {
            Debug.Log("Does not have mana, cannot use Mana Attack");
            playerMana.CheckManaPool();
            playerMana.hasMana = false;
        }
    }

    public void TerrorBreathAttackSelection()
    {
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
        }
    }

    public void HealingMagicSelection()
    {
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
            playerMana.hasMana = false;
        }
    }

    public void DefendSelection()
    {
        if (gameState == GameStates.PlayerTurn)
        {
            damageContainer.isDefending = true;
            DisablePlayerButtons();
            StartCoroutine(PerformDefense());
        }
    }

    public IEnumerator PerformDefense()
    {
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
        if (gameState == GameStates.PlayerTurn)
        {
            int activeEnemyCount = Mathf.Min(countEnemies.enemyAmount, enemyButtons.Count, battleEnemies.Length);

            for (int i = 0; i < activeEnemyCount; i++)
            {
                enemyButtons[i].gameObject.SetActive(true);

                EnemyBattleStats enemyStats = battleEnemies[i].GetComponent<EnemyBattleStats>();
                if (enemyStats != null && enemyStats.enemyCursor != null)
                {
                    enemyStats.enemyCursor.SetActive(true);
                }
            }
        }
    }

    public void EnemySelected()
    {
        int attackChosen = attackPicked;

        switch (attackChosen)
        {
            case 1:
                Debug.Log("Enemy Hit with Claw Strike!");
                attackContainer.playerAttackDelegate = attackContainer.ActivateClawAttack;
                attackContainer.playerAttackDelegate();
                break;
            case 2:
                Debug.Log("Enemy Hit with Tail Swipe!");
                attackContainer.playerAttackDelegate = attackContainer.ActivateTailAttack;
                attackContainer.playerAttackDelegate();
                break;
            case 3:
                Debug.Log("Enemy Hit with Fire Breath!");
                attackContainer.playerAttackDelegate = attackContainer.ActivateFireBreathAttack;
                attackContainer.playerAttackDelegate();
                break;
            case 4:
                Debug.Log("Enemy Hit with Ice Breath!");
                attackContainer.playerAttackDelegate = attackContainer.ActivateIceBreathAttack;
                attackContainer.playerAttackDelegate();
                break;
            case 5:
                Debug.Log("Enemy Hit with Terror Breath!");
                attackContainer.playerAttackDelegate = attackContainer.ActivateTerrorBreathAttack;
                attackContainer.playerAttackDelegate();
                break;
            case 6:
                Debug.Log("Enemy Hit with Vengeance Rush!");
                attackContainer.playerAttackDelegate = attackContainer.ActivateVengeanceRushAttack;
                attackContainer.playerAttackDelegate();
                break;
            case 7:
                Debug.Log("Enemy Hit with Vengeance Leap!");
                attackContainer.playerAttackDelegate = attackContainer.ActivateVengeanceBreathAttack;
                attackContainer.playerAttackDelegate();
                break;
        }
    }

    public void ActivateVengeance()
    {
        if (gameState == GameStates.PlayerTurn && playerVengeanceMeter.hasVengeance == true)
        {
            Debug.Log("The Player has Vengeance!");
            VengeanceMode();
        }
        else
        {
            Debug.Log("You do not have Vengeance, this mode cannot be activated...");
        }
    }

    public void VengeanceMode()
    {
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
        optionInfo.attackInfoText.SetActive(false);
        attackContainer.isPerformingVengeanceAttack = true;

        if (gameState == GameStates.PlayerTurn && playerVengeanceMeter.hasVengeance == true)
        {
            if (countEnemies.enemyAmount >= 2)
            {
                Debug.Log("More than one enemy in battle, choosing a target for this single target attack!");
                attackPicked = 6;
                ChooseTarget();
            }
            else if (countEnemies.enemyAmount == 1)
            {
                Debug.Log("Only one enemy in battle, attacking enemy!");
                attackContainer.enemyAttacked = 1;
                StartCoroutine(attackContainer.PerformVengeanceRush());
            }
        }
    }

    public void VengeanceBreathSelection()
    {
        optionInfo.attackInfoText.SetActive(false);
        attackContainer.isPerformingVengeanceAttack = true;

        if (gameState == GameStates.PlayerTurn && playerVengeanceMeter.hasVengeance == true)
        {
            attackPicked = 7;
            StartCoroutine(attackContainer.PerformVengeanceBreath());
        }
    }

    public void BackSelection()
    {
        if (gameState == GameStates.PlayerTurn)
        {
            Debug.Log("The Player has backed out of their action!");

            attackOptions.SetActive(false);
            magicOptions.SetActive(false);
            returnButton.SetActive(false);
            vengeanceButton.gameObject.SetActive(false);
            vengeanceOptions.SetActive(false);

            SetAllEnemyButtons(false);
            SetAllEnemyCursors(false);

            if (PlayerCombatAnimation.isBig == true)
            {
                PlayerCombatAnimation.isNotBig = true;
                PlayerCombatAnimation.isBig = false;
            }

            combatButtons[0].interactable = true;
            combatButtons[1].interactable = true;
            combatButtons[2].interactable = true;

            attackContainer.isPerformingMagicalAttack = false;
            attackContainer.isPerformingPhysicalAttack = false;
        }
        else
        {
            Debug.Log("The Player has not backed out of their action...");
        }
    }

    public void EnemyTurn()
    {
        if (gameState != GameStates.EnemyTurn)
        {
            return;
        }

        Debug.Log("Enemy is making their move!");
        battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        SetEnemyActiveFlagsByTurnCounter();

        EnemyBattleStats activeEnemy = GetCurrentActiveEnemy();

        if (activeEnemy != null)
        {
            DisablePlayerButtons();
            StartCoroutine(DetermineEnemyAction());
        }
    }

    /// <summary>
    /// Sets which enemy slot is currently acting based on the turn counter.
    /// </summary>
    private void SetEnemyActiveFlagsByTurnCounter()
    {
        firstEnemyActive = turnCounter == 2;
        secondEnemyActive = turnCounter == 3;
        thirdEnemyActive = turnCounter == 4;
    }

    public IEnumerator DetermineEnemyAction()
    {
        DisablePlayerButtons();
        DecideAttackOption();

        if (enemyAttackValue == 0 || enemyAttackValue == 2 || enemyAttackValue == 4 || enemyAttackValue == 6)
        {
            Debug.Log("Enemy is using a Physical Attack!");
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(EnemyAttack());
            yield return new WaitForSeconds(1f);
            TurnCheck();
        }
        else
        {
            Debug.Log("Enemy is using a Magic Attack!");
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(EnemyMagicAttack());
            yield return new WaitForSeconds(1f);
        }
    }

    public void DecideAttackOption()
    {
        Debug.Log("Determining Enemy Action " + enemyAttackValue);
        enemyAttackValue = Random.Range(0, 7);
    }

    private IEnumerator EnemyAttack()
    {
        Debug.Log("The Enemy has hit you with a Physical Attack!");
        damageContainer.performingAttack = true;
        DisablePlayerButtons();

        EnemyBattleStats activeEnemy = GetCurrentActiveEnemy();

        if (activeEnemy != null)
        {
            activeEnemy.PerformPhysicalAttack();
            AddVengeanceFromEnemyAttack(activeEnemy);
        }
        else
        {
            Debug.Log("No enemy available to perform a physical attack.");
        }

        yield return new WaitForSeconds(2f);

        EndEnemyAttackCleanup();
    }

    private IEnumerator EnemyMagicAttack()
    {
        Debug.Log("The Enemy has hit you with a Magical Attack!");
        damageContainer.performingAttack = true;
        DisablePlayerButtons();

        EnemyBattleStats activeEnemy = GetCurrentActiveEnemy();

        if (activeEnemy != null)
        {
            activeEnemy.PerformMagicalAttack();
            AddVengeanceFromEnemyAttack(activeEnemy);
        }
        else
        {
            Debug.Log("No enemy available to perform a magical attack.");
        }

        yield return new WaitForSeconds(2f);

        EndEnemyAttackCleanup();
    }

    /// <summary>
    /// Returns the enemy currently acting based on the active turn flags.
    /// </summary>
    private EnemyBattleStats GetCurrentActiveEnemy()
    {
        if (firstEnemyActive)
        {
            return GetEnemyAtIndex(0);
        }

        if (secondEnemyActive)
        {
            return GetEnemyAtIndex(1);
        }

        if (thirdEnemyActive)
        {
            return GetEnemyAtIndex(2);
        }

        return null;
    }

    /// <summary>
    /// Safely gets the enemy at the requested slot.
    /// </summary>
    private EnemyBattleStats GetEnemyAtIndex(int index)
    {
        if (countEnemies.enemyCombatSprites == null || index < 0 || index >= countEnemies.enemyCombatSprites.Count)
        {
            return null;
        }

        GameObject enemyObject = countEnemies.enemyCombatSprites[index];

        if (enemyObject == null)
        {
            return null;
        }

        return enemyObject.GetComponent<EnemyBattleStats>();
    }

    /// <summary>
    /// Adds vengeance points when the player is struck by an enemy attack.
    /// </summary>
    private void AddVengeanceFromEnemyAttack(EnemyBattleStats enemy)
    {
        float vengeanceGain = enemy.bossStatus ? 15.0f : 10.0f;

        playerVengeanceMeter.playerCurrentVengeance += vengeanceGain;

        if (playerVengeanceMeter.playerCurrentVengeance >= playerVengeanceMeter.playerMaxVengeance)
        {
            playerVengeanceMeter.playerCurrentVengeance = playerVengeanceMeter.playerMaxVengeance;
        }

        playerVengeanceMeter.playerVengeanceBar.fillAmount =
            playerVengeanceMeter.playerCurrentVengeance / 100f;

        playerVengeanceMeter.playerVengeanceText.text =
            playerVengeanceMeter.playerCurrentVengeance + " VP / " + playerVengeanceMeter.playerMaxVengeance;
    }

    /// <summary>
    /// Resets state after an enemy attack and advances turn flow.
    /// </summary>
    private void EndEnemyAttackCleanup()
    {
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
        else if (turnCounter == 3)
        {
            Debug.Log("The second enemy has completed their actions");
            secondEnemyTurnCompleted = true;
        }
        else if (turnCounter == 4)
        {
            Debug.Log("The third enemy has completed their actions");
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
        Debug.Log("Checking to see whose turn it currently is during the battle...");

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
        if (turnCounter >= 2 && turnCounter <= 4)
        {
            Debug.Log("Enemy turn indicated. Beginning enemy action sequence.");
            damageContainer.EnemyPhysicalAttack();
            damageContainer.EnemyMagicalAttack();
            StartCoroutine(DetermineEnemyAction());
        }
    }

    public void CheckTurnIndicator()
    {
        Debug.Log("Checking who is making their actions during battle");

        if (turnCounter == 1)
        {
            Debug.Log("The Turn Counter is one, it is now the player's turn!");
            gameState = GameStates.PlayerTurn;
            PlayerTurn();
        }
        else if (turnCounter == 2)
        {
            Debug.Log("The Turn Counter is two, it is now the first/single enemy's turn!");
            gameState = GameStates.EnemyTurn;
            firstEnemyActive = true;
            EnemyTurnOrder();
        }
        else if (turnCounter == 3)
        {
            Debug.Log("The Turn Counter is three, it is now the second enemy's turn!");
            gameState = GameStates.EnemyTurn;
            secondEnemyActive = true;
            EnemyTurnOrder();
        }
        else if (turnCounter == 4)
        {
            Debug.Log("The Turn Counter is four, it is now the third enemy's turn!");
            gameState = GameStates.EnemyTurn;
            thirdEnemyActive = true;
            EnemyTurnOrder();
        }
    }

    public void TurnOffButtons()
    {
        returnButton.SetActive(false);
        attackOptions.SetActive(false);
        magicOptions.SetActive(false);
        optionInfo.attackInfoText.SetActive(false);
        vengeanceButton.gameObject.SetActive(false);
        vengeanceOptions.SetActive(false);
        SetAllEnemyButtons(false);
    }

    public void DisablePlayerButtons()
    {
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
    }

    public void PlayerDeath()
    {
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
        // AI revision note:
        // The original script handled enemy death with multiple hard-coded branches for
        // 1, 2, and 3 enemies. This version removes defeated enemies generically so it
        // matches the refactored EnemyBattleStats system.
        bool anyEnemyRemoved = false;

        for (int i = countEnemies.enemyCombatSprites.Count - 1; i >= 0; i--)
        {
            GameObject enemyObject = countEnemies.enemyCombatSprites[i];

            if (enemyObject == null)
            {
                countEnemies.enemyCombatSprites.RemoveAt(i);
                continue;
            }

            EnemyBattleStats enemyStats = enemyObject.GetComponent<EnemyBattleStats>();

            if (enemyStats != null && enemyStats.enemyHealthStat <= 0)
            {
                Debug.Log(enemyObject.name + " has been defeated.");

                if (i < enemyButtons.Count && enemyButtons[i] != null)
                {
                    enemyButtons[i].gameObject.SetActive(false);
                    enemyButtons.RemoveAt(i);
                }

                Destroy(enemyObject, 0.4f);
                countEnemies.enemyCombatSprites.RemoveAt(i);
                countEnemies.enemyAmount--;
                anyEnemyRemoved = true;
            }
        }

        if (anyEnemyRemoved)
        {
            battleEnemies = countEnemies.enemyCombatSprites.ToArray();
        }

        if (countEnemies.enemyAmount <= 0)
        {
            Debug.Log("All enemies removed, ending battle.");
            StartCoroutine(BattleEnded());
        }
    }

    public void MultipleEnemyDeath()
    {
        if (countEnemies.enemyAmount >= 1)
        {
            Debug.Log("Destroying all enemies");
            battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in battleEnemies)
            {
                Debug.Log("Enemies defeated at once, destroying enemy");
                Destroy(enemy, 0.1f);
            }

            countEnemies.enemyCombatSprites.Clear();
            countEnemies.enemyAmount = 0;
            turnCounter = 1;
            CheckTurnIndicator();
            StartCoroutine(BattleEnded());
        }
    }

    public void MultiEnemyDeathCopy()
    {
        if (countEnemies.enemyAmount == 2)
        {
            battleEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in battleEnemies)
            {
                Debug.Log("Both enemies defeated at once, destroying both enemies");
                enemy.SetActive(false);
            }

            countEnemies.enemyAmount = 0;
            turnCounter = 1;
            CheckTurnIndicator();
            StartCoroutine(BattleEnded());
        }
    }

    public IEnumerator BattleEnded()
    {
        yield return new WaitForSeconds(2f);
        experienceSystem.ReceiveExperiencePoints();
        yield return new WaitForSeconds(3f);

        string currentSceneName = SceneManager.GetActiveScene().name;

        switch (currentSceneName)
        {
            case "TutorialCombat":
                SceneManager.LoadScene("TutorialCave");
                break;
            case "SingleWolfCombat":
            case "SingleHawkCombat":
            case "MultipleWolfCombat":
            case "MultipleHawkCombat":
            case "WolfHawkCombat":
                SceneManager.LoadScene(battleScenesManager.previousSceneName);
                break;
            case "SingleVillagerCombat":
                SceneManager.LoadScene("Village");
                break;
            case "MultipleVillagerCombat":
            case "TownVillagerCombat":
                SceneManager.LoadScene("CastleTown");
                break;
            case "FinalBattleCombat":
                SceneManager.LoadScene("EndScreen");
                break;
        }
    }

    /// <summary>
    /// Turns all enemy target buttons on or off safely.
    /// </summary>
    private void SetAllEnemyButtons(bool isActive)
    {
        for (int i = 0; i < enemyButtons.Count; i++)
        {
            if (enemyButtons[i] != null)
            {
                enemyButtons[i].gameObject.SetActive(isActive);
            }
        }
    }

    /// <summary>
    /// Turns all enemy target cursors on or off safely.
    /// </summary>
    private void SetAllEnemyCursors(bool isActive)
    {
        for (int i = 0; i < battleEnemies.Length; i++)
        {
            if (battleEnemies[i] == null)
            {
                continue;
            }

            EnemyBattleStats enemyStats = battleEnemies[i].GetComponent<EnemyBattleStats>();

            if (enemyStats != null && enemyStats.enemyCursor != null)
            {
                enemyStats.enemyCursor.SetActive(isActive);
            }
        }
    }
}