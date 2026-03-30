using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class EnemyBattleStats : MonoBehaviour
{
    [Header("Enemy UI")]
    [Tooltip("The enemy's health bar displayed during battle.")]
    public Image enemyHealthBar;

    [Tooltip("The enemy's mana bar displayed during battle.")]
    public Image enemyManaBar;

    [Tooltip("The enemy's health text displayed during battle.")]
    public TextMeshProUGUI enemyHealthText;

    [Tooltip("The enemy's mana text displayed during battle.")]
    public TextMeshProUGUI enemyManaText;

    [Tooltip("The visual indicator shown when it is this enemy's turn.")]
    public GameObject turnSignal;

    [Header("Enemy Stats")]
    [Tooltip("The enemy's current health value during battle.")]
    public float enemyHealthStat;

    [Tooltip("The enemy's maximum health value during battle.")]
    public float enemyMaxHealthStat;

    [Tooltip("The enemy's current mana value during battle.")]
    public float enemyManaStat;

    [Tooltip("The enemy's maximum mana value during battle.")]
    public float enemyMaxManaStat;

    [Tooltip("The enemy's physical attack value.")]
    public float enemyPhysicalAttackStat;

    [Tooltip("The enemy's magical attack value.")]
    public float enemyMagicalAttackStat;

    [Tooltip("The enemy's speed value.")]
    public float enemySpeedStat;

    [Tooltip("The amount of experience awarded when this enemy is defeated.")]
    public int enemyExpPoints;

    [Tooltip("The enemy's turn-order index used by the combat turn system.")]
    public int enemyIndex;

    [Tooltip("The cursor displayed when this enemy is selected as a target.")]
    public GameObject enemyCursor;

    [Header("Enemy Level Settings")]
    [Tooltip("The current level assigned to this enemy.")]
    public int enemyLevel;

    [Tooltip("Upper threshold for low-level enemy classification.")]
    public int lowLevelEnemyRange;

    [Tooltip("Upper threshold for mid-level enemy classification.")]
    public int midLevelEnemyRange;

    [Tooltip("Upper threshold for high-level enemy classification.")]
    public int highLevelEnemyRange;

    [Tooltip("Determines whether this enemy should use boss-specific stat behavior.")]
    public bool bossStatus;

    [Header("Enemy Level Flags")]
    [Tooltip("True when the enemy has been classified as low level.")]
    public bool isALowLevelEnemy;

    [Tooltip("True when the enemy has been classified as mid level.")]
    public bool isAMidLevelEnemy;

    [Tooltip("True when the enemy has been classified as high level.")]
    public bool isAHighLevelEnemy;

    [Header("Mana State")]
    [Tooltip("The amount of mana consumed when this enemy uses a magical attack.")]
    public float manaUsage = 10f;

    [Tooltip("Determines whether the enemy currently has enough mana to use mana-dependent actions.")]
    public bool hasMana;

    [Tooltip("Tracks whether this enemy has lost mana.")]
    public bool lostMana;

    // Shared combat system references used by all enemy types.
    protected TurnSystem turnSystem;
    protected DamageContainer damageContainer;
    protected PlayerStats playerStats;
    protected PlayerHealth playerHealth;
    protected PlayerMana playerMana;
    protected EnemyLevelTracker enemyLevelTracker;
    protected CountEnemies countEnemies;
    protected Animator animator;

    // Derived enemy scripts override these properties to define unique behavior.
    protected virtual bool IsBossEnemy => false;
    protected virtual string PhysicalAttackTrigger => string.Empty;
    protected virtual string MagicalAttackTrigger => PhysicalAttackTrigger;
    protected virtual float? OverridePhysicalAttackDamage => null;

    protected virtual void Awake()
    {
        // AI revision note:
        // The original combat system repeated the same FindObjectOfType setup
        // in every enemy script. That shared setup is now centralized here.
        CacheCommonReferences();
        DetermineEnemyStrength();
        bossStatus = IsBossEnemy;
    }

    protected virtual void Start()
    {
        Debug.Log("Check for the enemies' level at the start of battle");
        CheckEnemyLevel();
        hasMana = true;
        RefreshEnemyUI();
    }

    protected virtual void Update()
    {
        UpdateEnemyTurnSignal();
    }

    /// <summary>
    /// Caches the shared combat references used by all enemy combat scripts.
    /// </summary>
    protected void CacheCommonReferences()
    {
        turnSystem = FindObjectOfType<TurnSystem>();
        damageContainer = FindObjectOfType<DamageContainer>();
        playerStats = FindObjectOfType<PlayerStats>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMana = FindObjectOfType<PlayerMana>();
        enemyLevelTracker = FindObjectOfType<EnemyLevelTracker>();
        countEnemies = FindObjectOfType<CountEnemies>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Sets this enemy's strength based on the current tracked area level.
    /// </summary>
    public void DetermineEnemyStrength()
    {
        if (enemyLevelTracker == null)
        {
            Debug.LogWarning("EnemyLevelTracker not found. Enemy level will remain unchanged.");
            return;
        }

        Debug.Log("Determining Enemy Strength... " + enemyLevel);
        enemyLevel = enemyLevelTracker.areaLevel;
    }

    /// <summary>
    /// Updates the turn indicator based on whether this enemy should currently act.
    /// </summary>
    public void UpdateEnemyTurnSignal()
    {
        bool shouldShowTurnSignal = ShouldShowTurnSignal();
        turnSignal.SetActive(shouldShowTurnSignal);

        Debug.Log(shouldShowTurnSignal
            ? "It is now the enemy's turn!"
            : "It is not the enemy's turn or it is not the proper enemy's turn...");
    }

    /// <summary>
    /// Determines whether this specific enemy should show its turn indicator.
    /// Derived enemy scripts must define their own turn-order logic.
    /// </summary>
    protected abstract bool ShouldShowTurnSignal();

    /// <summary>
    /// Applies physical damage from the player to this enemy.
    /// </summary>
    public virtual void TakePhysicalDamage()
    {
        ApplyDamageToEnemy(damageContainer.playerPhysicalAttackDamage);
    }

    /// Applies magical damage from the player to this enemy.
    /// Also removes mana if the incoming attack includes a mana-drain effect.
    public virtual void TakeMagicalDamage()
    {
        ApplyDamageToEnemy(damageContainer.playerMagicalAttackDamage);

        if (damageContainer.isAttacking && damageContainer.isTakingMana && hasMana)
        {
            Debug.Log("Enemy was hit with a mana-draining attack.");
            ApplyManaDrain(playerMana.manaUsage);
        }
    }

    /// <summary>
    /// Applies vengeance damage from the player to this enemy.
    /// </summary>
    public virtual void TakeVengeanceDamage()
    {
        ApplyDamageToEnemy(damageContainer.playerVengeanceAttackDamage);
    }

    /// <summary>
    /// Performs the enemy's physical attack against the player.
    /// </summary>
    public virtual void PerformPhysicalAttack()
    {
        TriggerAnimation(PhysicalAttackTrigger);

        if (OverridePhysicalAttackDamage.HasValue)
        {
            damageContainer.enemyPhysicalAttackDamage = OverridePhysicalAttackDamage.Value;
        }

        if (damageContainer.isDefending)
        {
            Debug.Log("Player is defending the enemy's physical attack.");
            damageContainer.PlayerDefense();
            damageContainer.enemyPhysicalAttackDamage *= damageContainer.playerDefenseReduction;
            ApplyDirectDamageToPlayer(damageContainer.enemyPhysicalAttackDamage);
        }
        else
        {
            Debug.Log("Player is not defending. Enemy physical damage remains unchanged.");
            damageContainer.EnemyPhysicalAttack();
            playerHealth.TakePhysicalDamage();
            RefreshPlayerHealthUI();
        }
    }

    /// <summary>
    /// Performs the enemy's magical attack against the player and consumes mana.
    /// </summary>
    public virtual void PerformMagicalAttack()
    {
        TriggerAnimation(MagicalAttackTrigger);

        if (damageContainer.isDefending)
        {
            Debug.Log("Player is defending the enemy's magical attack.");
            damageContainer.PlayerDefense();
            damageContainer.enemyMagicalAttackDamage *= damageContainer.playerDefenseReduction;
            ApplyDirectDamageToPlayer(damageContainer.enemyMagicalAttackDamage);
        }
        else
        {
            Debug.Log("Player is not defending. Enemy magical damage remains unchanged.");
            damageContainer.EnemyMagicalAttack();
            playerHealth.TakeMagicalDamage();
            RefreshPlayerHealthUI();
        }

        SpendManaForMagicalAttack();
    }

    /// <summary>
    /// Triggers the specified enemy animation if a valid trigger name exists.
    /// </summary>
    protected void TriggerAnimation(string triggerName)
    {
        if (!string.IsNullOrEmpty(triggerName) && animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }

    /// <summary>
    /// Applies damage to the enemy, clamps health at zero, and refreshes the enemy health UI.
    /// </summary>
    /// <param name="damageAmount">The amount of damage dealt to the enemy.</param>
    protected void ApplyDamageToEnemy(float damageAmount)
    {
        // AI revision note:
        // This helper centralizes repeated enemy health calculations and UI updates that
        // originally appeared in every enemy-specific combat script.
        enemyHealthStat -= damageAmount;

        if (enemyHealthStat < 0)
        {
            enemyHealthStat = 0;
        }

        RefreshEnemyHealthUI();
    }

    /// <summary>
    /// Applies mana drain to the enemy, clamps mana at zero, and refreshes the enemy mana UI.
    /// </summary>
    /// <param name="manaAmount">The amount of mana removed.</param>
    protected void ApplyManaDrain(float manaAmount)
    {
        // AI revision note:
        // This helper centralizes mana subtraction and mana UI updates so every enemy
        // handles mana drain consistently.
        enemyManaStat -= manaAmount;

        if (enemyManaStat <= 0)
        {
            Debug.Log("Enemy mana fully depleted. Resetting to 0.");
            enemyManaStat = 0;
            hasMana = false;
        }

        RefreshEnemyManaUI();
    }

    /// <summary>
    /// Applies the standard mana cost for a magical attack.
    /// </summary>
    protected void SpendManaForMagicalAttack()
    {
        ApplyManaDrain(manaUsage);
    }

    /// <summary>
    /// Applies direct damage to the player and refreshes the player's health UI.
    /// </summary>
    /// <param name="damageAmount">The amount of damage dealt to the player.</param>
    protected void ApplyDirectDamageToPlayer(float damageAmount)
    {
        playerStats.healthStat -= damageAmount;

        if (playerStats.healthStat < 0)
        {
            playerStats.healthStat = 0;
        }

        RefreshPlayerHealthUI();
    }

    /// <summary>
    /// Refreshes both enemy health and mana UI.
    /// </summary>
    protected void RefreshEnemyUI()
    {
        RefreshEnemyHealthUI();
        RefreshEnemyManaUI();
    }

    /// <summary>
    /// Refreshes the enemy's health bar and health text.
    /// </summary>
    protected void RefreshEnemyHealthUI()
    {
        if (enemyHealthText != null)
        {
            enemyHealthText.text = enemyHealthStat + " / " + enemyMaxHealthStat;
        }

        if (enemyHealthBar != null)
        {
            enemyHealthBar.fillAmount = enemyMaxHealthStat > 0 ? enemyHealthStat / enemyMaxHealthStat : 0f;
        }
    }

    /// <summary>
    /// Refreshes the enemy's mana bar and mana text.
    /// </summary>
    protected void RefreshEnemyManaUI()
    {
        if (enemyManaText != null)
        {
            enemyManaText.text = enemyManaStat + " / " + enemyMaxManaStat;
        }

        if (enemyManaBar != null)
        {
            enemyManaBar.fillAmount = enemyMaxManaStat > 0 ? enemyManaStat / enemyMaxManaStat : 0f;
        }
    }

    /// <summary>
    /// Refreshes the player's health bar and health text after an enemy attack.
    /// </summary>
    protected void RefreshPlayerHealthUI()
    {
        if (playerHealth != null)
        {
            playerHealth.playerHealthBar.fillAmount = playerStats.maxHealthStat > 0
                ? playerStats.healthStat / playerStats.maxHealthStat
                : 0f;

            playerHealth.playerHealthText.text = playerStats.healthStat + " / " + playerStats.maxHealthStat;
        }
    }

    /// <summary>
    /// Determines which stat profile should be applied based on the enemy's level.
    /// </summary>
    public void CheckEnemyLevel()
    {
        if (enemyLevel < lowLevelEnemyRange)
        {
            Debug.Log("The enemy falls within the Low Level Enemy range...");
            LowLevelEnemyCheck();
        }
        else if (enemyLevel >= lowLevelEnemyRange && enemyLevel < midLevelEnemyRange)
        {
            Debug.Log("The enemy falls within the Mid Level Enemy range...");
            MidLevelEnemyCheck();
        }
        else if (enemyLevel >= midLevelEnemyRange && enemyLevel < highLevelEnemyRange)
        {
            Debug.Log("The enemy falls within the High Level Enemy range...");
            HighLevelEnemyCheck();
        }
        else
        {
            Debug.Log("Cannot determine enemy level...");
        }
    }

    /// <summary>
    /// Applies the stat profile for a low-level enemy.
    /// </summary>
    public void LowLevelEnemyCheck()
    {
        Debug.Log("This is a low level enemy, adjusting stats...");
        isALowLevelEnemy = true;

        if (isALowLevelEnemy && bossStatus != true)
        {
            enemyHealthStat = 50;
            enemyMaxHealthStat = 50;
            enemyManaStat = 50;
            enemyMaxManaStat = 50;
            enemyPhysicalAttackStat = 10;
            enemyMagicalAttackStat = 10;
            enemySpeedStat = 5;
            RefreshEnemyUI();
        }
    }

    /// <summary>
    /// Applies the stat profile for a mid-level enemy.
    /// </summary>
    public void MidLevelEnemyCheck()
    {
        Debug.Log("This is a mid level enemy, adjusting stats...");
        isAMidLevelEnemy = true;

        if (isAMidLevelEnemy && bossStatus == false)
        {
            enemyHealthStat = 75;
            enemyMaxHealthStat = 75;
            enemyManaStat = 75;
            enemyMaxManaStat = 75;
            enemyPhysicalAttackStat = 20;
            enemyMagicalAttackStat = 20;
            enemySpeedStat = 5;
            enemyExpPoints = 50;
            RefreshEnemyUI();
        }

        if (isAMidLevelEnemy && bossStatus == true)
        {
            enemyPhysicalAttackStat = 20;
            enemyMagicalAttackStat = 20;
            enemySpeedStat = 5;
        }
    }

    /// <summary>
    /// Applies the stat profile for a high-level enemy.
    /// </summary>
    public void HighLevelEnemyCheck()
    {
        Debug.Log("This is a high level enemy, adjusting stats...");
        isAHighLevelEnemy = true;

        if (isAHighLevelEnemy)
        {
            enemyPhysicalAttackStat = 30;
            enemyMagicalAttackStat = 30;
        }
    }
}