using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base class for all combat enemies.
/// 
/// This class centralizes shared enemy functionality including:
/// - Health and mana management
/// - Damage handling (physical, magical, vengeance)
/// - UI updates (health bars, mana bars, text)
/// - Animation triggering
/// - Level-based stat scaling
/// - Battle taunt metadata
/// 
/// Individual enemy scripts inherit from this class and only define:
/// - Unique animation triggers
/// - Turn-order behavior
/// - Optional attack overrides
/// - Boss status
/// </summary>
public abstract class EnemyBattleStats : MonoBehaviour
{
    #region UI References

    [Header("Enemy UI")]
    [Tooltip("Health bar UI element for this enemy.")]
    public Image enemyHealthBar;

    [Tooltip("Mana bar UI element for this enemy.")]
    public Image enemyManaBar;

    [Tooltip("Text displaying current and maximum health.")]
    public TextMeshProUGUI enemyHealthText;

    [Tooltip("Text displaying current and maximum mana.")]
    public TextMeshProUGUI enemyManaText;

    [Tooltip("Visual indicator used to show when it is this enemy's turn.")]
    public GameObject turnSignal;

    #endregion

    #region Core Stats

    [Header("Enemy Stats")]
    [Tooltip("Current health value.")]
    public float enemyHealthStat;

    [Tooltip("Maximum health value.")]
    public float enemyMaxHealthStat;

    [Tooltip("Current mana value.")]
    public float enemyManaStat;

    [Tooltip("Maximum mana value.")]
    public float enemyMaxManaStat;

    [Tooltip("Physical attack strength.")]
    public float enemyPhysicalAttackStat;

    [Tooltip("Magical attack strength.")]
    public float enemyMagicalAttackStat;

    [Tooltip("Speed stat used for turn order.")]
    public float enemySpeedStat;

    [Tooltip("Experience points awarded when defeated.")]
    public int enemyExpPoints;

    [Tooltip("Index used for determining turn order.")]
    public int enemyIndex;

    [Tooltip("Cursor shown when selecting this enemy.")]
    public GameObject enemyCursor;

    #endregion

    #region Level System

    [Header("Enemy Level Settings")]
    [Tooltip("Current level of the enemy.")]
    public int enemyLevel;

    [Tooltip("Upper bound for low-level classification.")]
    public int lowLevelEnemyRange;

    [Tooltip("Upper bound for mid-level classification.")]
    public int midLevelEnemyRange;

    [Tooltip("Upper bound for high-level classification.")]
    public int highLevelEnemyRange;

    [Tooltip("Indicates if this enemy is a boss.")]
    public bool bossStatus;

    [Header("Level Flags")]
    [Tooltip("True when the enemy is classified as low level.")]
    public bool isALowLevelEnemy;

    [Tooltip("True when the enemy is classified as mid level.")]
    public bool isAMidLevelEnemy;

    [Tooltip("True when the enemy is classified as high level.")]
    public bool isAHighLevelEnemy;

    #endregion

    #region Mana System

    [Header("Mana Settings")]
    [Tooltip("Amount of mana consumed per magical attack.")]
    public float manaUsage = 10f;

    [Tooltip("True if the enemy currently has enough mana to cast.")]
    public bool hasMana;

    [Tooltip("Tracks whether mana has been depleted.")]
    public bool lostMana;

    #endregion

    #region Battle Taunts

    [Header("Battle Taunts")]
    [Tooltip("Name used by the battle taunt system for this enemy type.")]
    [SerializeField]
    private string tauntEnemyName = "Enemy";

    [Tooltip("Optional personality/style hint for battle taunts.")]
    [SerializeField]
    private string tauntStyle = "hostile and concise";

    /// <summary>
    /// Public accessor used by the taunt system.
    /// Falls back to the GameObject name if no custom taunt name is set.
    /// </summary>
    public string TauntEnemyName => string.IsNullOrWhiteSpace(tauntEnemyName) ? gameObject.name : tauntEnemyName;

    /// <summary>
    /// Public accessor used by the taunt system to describe tone/personality.
    /// </summary>
    public string TauntStyle => tauntStyle;

    #endregion

    #region Shared System References

    // Shared combat system references used by all enemies.
    protected TurnSystem turnSystem;
    protected DamageContainer damageContainer;
    protected PlayerStats playerStats;
    protected PlayerHealth playerHealth;
    protected PlayerMana playerMana;
    protected EnemyLevelTracker enemyLevelTracker;
    protected CountEnemies countEnemies;
    protected Animator animator;

    #endregion

    #region Customization Points

    /// <summary>
    /// Derived enemy scripts override this to identify boss enemies.
    /// </summary>
    protected virtual bool IsBossEnemy => false;

    /// <summary>
    /// Derived enemy scripts override this to set the physical attack animation trigger.
    /// </summary>
    protected virtual string PhysicalAttackTrigger => string.Empty;

    /// <summary>
    /// Derived enemy scripts override this to set the magical attack animation trigger.
    /// Defaults to the physical trigger if not overridden.
    /// </summary>
    protected virtual string MagicalAttackTrigger => PhysicalAttackTrigger;

    /// <summary>
    /// Optional override used by specific enemies that need fixed physical damage.
    /// </summary>
    protected virtual float? OverridePhysicalAttackDamage => null;

    #endregion

    #region Unity Lifecycle

    protected virtual void Awake()
    {
        // Cache all shared combat references used by every enemy.
        CacheReferences();

        // Determine enemy strength based on the current area.
        DetermineEnemyStrength();

        // Set boss status from the derived enemy definition.
        bossStatus = IsBossEnemy;
    }

    protected virtual void Start()
    {
        Debug.Log("Initializing enemy stats...");

        // Apply level-based stat scaling at battle start.
        CheckEnemyLevel();

        // Enemies begin battle with mana available by default.
        hasMana = true;

        // Initialize the enemy's UI to reflect current stats.
        RefreshEnemyUI();
    }

    protected virtual void Update()
    {
        // Keep the turn indicator in sync with turn flow.
        UpdateTurnIndicator();
    }

    #endregion

    #region Setup

    /// <summary>
    /// Finds and caches all required combat system references.
    /// </summary>
    protected void CacheReferences()
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
    /// Sets enemy level based on the currently tracked area.
    /// </summary>
    public void DetermineEnemyStrength()
    {
        if (enemyLevelTracker == null)
        {
            Debug.LogWarning("EnemyLevelTracker not found. Enemy level will remain unchanged.");
            return;
        }

        enemyLevel = enemyLevelTracker.areaLevel;
    }

    #endregion

    #region Turn Indicator

    /// <summary>
    /// Updates the visual turn indicator using enemy-specific turn logic.
    /// </summary>
    public void UpdateTurnIndicator()
    {
        if (turnSignal != null)
        {
            turnSignal.SetActive(ShouldShowTurnSignal());
        }
    }

    /// <summary>
    /// Implemented by child classes to determine when this enemy should show its turn indicator.
    /// </summary>
    protected abstract bool ShouldShowTurnSignal();

    #endregion

    #region Damage Handling

    /// <summary>
    /// Applies physical damage received from the player.
    /// </summary>
    public virtual void TakePhysicalDamage()
    {
        ApplyDamage(damageContainer.playerPhysicalAttackDamage);
    }

    /// <summary>
    /// Applies magical damage received from the player.
    /// Also applies mana drain when appropriate.
    /// </summary>
    public virtual void TakeMagicalDamage()
    {
        ApplyDamage(damageContainer.playerMagicalAttackDamage);

        if (damageContainer.isAttacking && damageContainer.isTakingMana && hasMana)
        {
            ApplyManaDrain(playerMana.manaUsage);
        }
    }

    /// <summary>
    /// Applies vengeance damage received from the player.
    /// </summary>
    public virtual void TakeVengeanceDamage()
    {
        ApplyDamage(damageContainer.playerVengeanceAttackDamage);
    }

    /// <summary>
    /// Applies damage to the enemy, clamps health at zero, and updates the health UI.
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    protected void ApplyDamage(float damage)
    {
        enemyHealthStat = Mathf.Max(0, enemyHealthStat - damage);
        RefreshEnemyHealthUI();
    }

    /// <summary>
    /// Applies mana drain to the enemy, clamps mana at zero, and updates the mana UI.
    /// </summary>
    /// <param name="amount">The amount of mana to remove.</param>
    protected void ApplyManaDrain(float amount)
    {
        enemyManaStat -= amount;

        if (enemyManaStat <= 0)
        {
            enemyManaStat = 0;
            hasMana = false;
        }

        RefreshEnemyManaUI();
    }

    #endregion

    #region Attacks

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
            damageContainer.PlayerDefense();
            damageContainer.enemyPhysicalAttackDamage *= damageContainer.playerDefenseReduction;
            ApplyDirectDamageToPlayer(damageContainer.enemyPhysicalAttackDamage);
        }
        else
        {
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
            damageContainer.PlayerDefense();
            damageContainer.enemyMagicalAttackDamage *= damageContainer.playerDefenseReduction;
            ApplyDirectDamageToPlayer(damageContainer.enemyMagicalAttackDamage);
        }
        else
        {
            damageContainer.EnemyMagicalAttack();
            playerHealth.TakeMagicalDamage();
            RefreshPlayerHealthUI();
        }

        ApplyManaDrain(manaUsage);
    }

    /// <summary>
    /// Triggers the requested animation if a valid trigger is defined.
    /// </summary>
    /// <param name="trigger">Animator trigger name.</param>
    protected void TriggerAnimation(string trigger)
    {
        if (!string.IsNullOrEmpty(trigger) && animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }

    /// <summary>
    /// Applies direct damage to the player and refreshes the player's health UI.
    /// </summary>
    /// <param name="damageAmount">Damage dealt to the player.</param>
    protected void ApplyDirectDamageToPlayer(float damageAmount)
    {
        playerStats.healthStat -= damageAmount;

        if (playerStats.healthStat < 0)
        {
            playerStats.healthStat = 0;
        }

        RefreshPlayerHealthUI();
    }

    #endregion

    #region UI Updates

    /// <summary>
    /// Refreshes all enemy UI values.
    /// </summary>
    protected void RefreshEnemyUI()
    {
        RefreshEnemyHealthUI();
        RefreshEnemyManaUI();
    }

    /// <summary>
    /// Refreshes enemy health bar and text.
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
    /// Refreshes enemy mana bar and text.
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
    /// Refreshes the player's health display after enemy attacks.
    /// </summary>
    protected void RefreshPlayerHealthUI()
    {
        if (playerHealth != null)
        {
            playerHealth.playerHealthBar.fillAmount =
                playerStats.maxHealthStat > 0 ? playerStats.healthStat / playerStats.maxHealthStat : 0f;

            playerHealth.playerHealthText.text =
                playerStats.healthStat + " / " + playerStats.maxHealthStat;
        }
    }

    #endregion

    #region Level Scaling

    /// <summary>
    /// Applies the appropriate enemy stat profile based on level range.
    /// </summary>
    public void CheckEnemyLevel()
    {
        if (enemyLevel < lowLevelEnemyRange)
        {
            LowLevelEnemyCheck();
        }
        else if (enemyLevel >= lowLevelEnemyRange && enemyLevel < midLevelEnemyRange)
        {
            MidLevelEnemyCheck();
        }
        else if (enemyLevel >= midLevelEnemyRange && enemyLevel < highLevelEnemyRange)
        {
            HighLevelEnemyCheck();
        }
    }

    /// <summary>
    /// Applies low-level enemy stats.
    /// </summary>
    public void LowLevelEnemyCheck()
    {
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
            enemyExpPoints = 25;

            RefreshEnemyUI();
        }
    }

    /// <summary>
    /// Applies mid-level enemy stats.
    /// </summary>
    public void MidLevelEnemyCheck()
    {
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
            enemyExpPoints = 100;
        }
    }

    /// <summary>
    /// Applies high-level enemy stats.
    /// </summary>
    public void HighLevelEnemyCheck()
    {
        isAHighLevelEnemy = true;

        if (isAHighLevelEnemy)
        {
            enemyPhysicalAttackStat = 30;
            enemyMagicalAttackStat = 30;
            enemyExpPoints = 75;
        }
    }

    #endregion

    // AI revision note:
    // This script was refactored to centralize all shared enemy combat behavior into one base class.
    // Previously, each enemy script duplicated:
    // - Damage handling
    // - Mana handling
    // - UI refresh logic
    // - Player damage logic
    //
    // This new structure preserves original functionality while reducing duplication,
    // improving maintainability, and making it much easier to add new enemy types.
    //
    // Taunt metadata was also added so the battle taunt system can identify enemies
    // cleanly without hard-coding enemy names inside TurnSystem.
}